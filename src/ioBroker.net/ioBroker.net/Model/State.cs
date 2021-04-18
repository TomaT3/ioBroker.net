using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ioBroker.net.Model
{
    internal class State
    {
        /// <summary>
        /// the actual value - can be any type that is JSON-"encodable"
        /// </summary>
        [JsonPropertyName("val")]
        public object Val { get; set; }

        /// <summary>
        /// a boolean flag indicating if the target system has acknowledged the value
        /// </summary>
        [JsonPropertyName("ack")]
        public bool Ack { get; set; }

        /// <summary>
        /// adapter instance that did the "setState"
        /// </summary>
        [JsonPropertyName("from")]
        public string From { get; set; }

        /// <summary>
        /// the username, that set the value
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; set; }
    }
}
