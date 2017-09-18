using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Zohan.ApiEvents.Models;
using Zohan.EventGrid;

namespace Zohan.ApiEvents
{
    public static class ClientErrorsFunc
    {
        [FunctionName("ClientErrorsFunc")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info(string.Format("ClientErrorsFunc - {0}", DateTime.Now.ToLongTimeString()));
            var errors = await req.Content.ReadAsAsync<IEnumerable<EventGridTopic<ApiError>>>();
            foreach (var e in errors)
            {
                var message = string.Format("Received: {0} - {1} at {2}",
                    e.Data.StatusCode, e.Data.StatusReason, DateTime.Now.ToLongTimeString());
                log.Info(message);
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
