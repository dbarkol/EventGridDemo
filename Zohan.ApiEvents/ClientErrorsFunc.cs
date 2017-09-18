using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Zohan.ApiEvents
{
    public static class ClientErrorsFunc
    {
        [FunctionName("ClientErrorsFunc")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info(string.Format("ClientErrorsFunc - {0}", DateTime.Now.ToLongTimeString()));

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
