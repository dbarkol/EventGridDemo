using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Zohan.EventGrid
{
    /// <summary>
    /// Template for an Event Grid topic
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridEvent<T> where T : class
    {
        #region Properties

        /// <summary>
        /// Identifier for the event grid topic
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Subject for the event grid topic
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Event type
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Data for the topic
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Date and time of the event
        /// </summary>
        public DateTime EventTime { get; set; }

        #endregion
    }
}
