using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zohan.EventGrid;

namespace Zohan.EventGridTest
{
    class Program
    {
        private const string TopicEndpoint = "[place endpoint here]";
        private const string TopicKey = "[place key here]";

        static void Main(string[] args)
        {
            SendApiErrors().Wait();
            Console.WriteLine("Press <enter> to exit.");
            Console.ReadLine();
        }

        private static async Task SendApiErrors()
        {
            var errors = new List<GridEvent<ApiError>>
            {
                new GridEvent<ApiError>()
                {
                    Data =
                        new ApiError()
                        {
                            MessageType = "Error",
                            Method = "GET",
                            StatusCode = 429,
                            UrlHost = "hostaddress1",
                            UrlPath = "hostpath1",
                            StatusReason = "reason1",
                            UserEmail = "test@email.com"
                        },
                    Subject = "manufacturing/orders",
                    EventType = "apiInfo",
                    EventTime = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString()
                }
            };

            await EventGridUtils.SendEvent(TopicEndpoint, TopicKey, errors);
        }

    }

    public class ApiError
    {
        /// <summary>
        /// The type of message
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// The HTTP method used
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The status code returned
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The HTTP status reason
        /// </summary>
        public string StatusReason { get; set; }

        /// <summary>
        /// User email associated with the request
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// URL path
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// URL host
        /// </summary>
        public string UrlHost { get; set; }
    }
}
