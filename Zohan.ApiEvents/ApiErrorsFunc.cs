using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
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

            await PublishGridEvent(apiError);

            return req.CreateResponse(HttpStatusCode.OK,
                string.Format("ApiErrorsFunc - {0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
        }

        private static async Task PublishGridEvent(ApiError error)
        {
            // Set the event subject to 'server' or 'client' based 
            // on the status code.            
            string eventSubject;
            if (error.StatusCode >= 500)
            {
                eventSubject = "server";
            }
            else
            {
                eventSubject = "client";

                // If the status code is 429 then append
                // additional content to the subject for 
                // more filter options. 
                if (error.StatusCode == 429)
                    eventSubject += "/too-many-requests";
            }

            // Retrieve the event grid endpoint and key so that we can 
            // publish events.
            var topicEndpoint = System.Environment.GetEnvironmentVariable("EventGridTopicEndpoint");
            var topicKey = System.Environment.GetEnvironmentVariable("EventGridTopicKey");

            // Events are sent to event grid in an array
            var errors = new List<GridEvent<ApiError>>
            {
                new GridEvent<ApiError>()
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
                    Subject = eventSubject,
                    EventType = "applicationError",
                    EventTime = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString()
                }
            };

            await EventGrid.EventGridUtils.SendEvent(topicEndpoint, topicKey, errors);
        }

    }
}
