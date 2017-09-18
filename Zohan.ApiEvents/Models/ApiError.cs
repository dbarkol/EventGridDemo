using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zohan.ApiEvents.Models
{
    /// <summary>
    /// API error details
    /// </summary>
    public class ApiError
    {
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
