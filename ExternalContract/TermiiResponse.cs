using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.ExternalContract
{
    public class TermiiResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }
    }
}
