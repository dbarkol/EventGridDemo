using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Zohan.ApiEvents.Models;
using Zohan.EventGrid;

namespace Zohan.ApiEvents
{
    public static class ApiErrorsFunc
    {
        [FunctionName("ApiErrorsFunc")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function,  "post", Route = null)]HttpRequestMessage req, 
            TraceWriter log)
        {
            log.Info(string.Format("ApiErrorsFunc triggered. {0}", DateTime.Now.ToLongTimeString()));

            // Retrieve the body from the request. If there isn't any 
            // content then return a 400 with the appropriate message.
            var apiError = await req.Content.ReadAsAsync<ApiError>();
            if (apiError == null)
                return req.CreateResponse(HttpStatusCode.BadRequest, "Missing body content.");

            // Diagnostics
            log.Info(string.Format("Method: {0}", apiError.Method));
            log.Info(string.Format("Status Code: {0}", apiError.StatusCode));
            log.Info(string.Format("Status Reason: {0}", apiError.StatusReason));
            log.Info(string.Format("URL Path: {0}", apiError.UrlPath));
            log.Info(string.Format("URL Host: {0}", apiError.UrlHost));
            log.Info(string.Format("Email: {0}", apiError.UserEmail));

            // Let's rock - send it!
            await PublishGridEvent(apiError);

            return req.CreateResponse(HttpStatusCode.OK,
                string.Format("ApiErrorsFunc - {0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
        }

        private static async Task PublishGridEvent(ApiError error)
        {
            // Set the event type to 'server' or 'client' based 
            // on the status code.
            var eventType = error.StatusCode >= 500 ? "server" : String.IsInterned("client");

            // Retrieve the event grid endpoint and key so that we can 
            // publish events.
            var topicEndpoint = System.Environment.GetEnvironmentVariable("EventGridTopicEndpoint");
            var topicKey = System.Environment.GetEnvironmentVariable("EventGridTopicKey");

            // Events are sent to event grid in an array
            var errors = new List<EventGridTopic<ApiError>>
            {
                new EventGridTopic<ApiError>()
                {
                    Data =
                        new ApiError()
                        {
                            Method = error.Method,
                            StatusCode = error.StatusCode,
                            UrlHost = error.UrlHost,
                            UrlPath = error.UrlPath,
                            StatusReason = error.StatusReason,
                            UserEmail = error.UserEmail
                        },
                    Subject = "applicationerror",
                    EventType = eventType,
                    EventTime = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString()
                }
            };

            await EventGrid.EventGridUtils.SendEvent(topicEndpoint, topicKey, errors);
        }

    }
}
