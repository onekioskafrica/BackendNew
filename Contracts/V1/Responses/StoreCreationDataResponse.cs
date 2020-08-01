using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Responses
{
    public class StoreCreationDataResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("store_owner_id")]
        public Guid StoreOwnerId { get; set; }

        [JsonPropertyName("store_id")]
        public string StoreId { get; set; }

        [JsonPropertyName("store_name")]
        public string StoreName { get; set; }

        [JsonPropertyName("store_phone_number")]
        public string StorePhoneNumber { get; set; }

        [JsonPropertyName("store_email_address")]
        public string StoreEmailAddress { get; set; }

        [JsonPropertyName("logo_url")]
        public string LogoUrl { get; set; }

        [JsonPropertyName("store_intro")]
        public string StoreIntro { get; set; }

        [JsonPropertyName("store_creation_reason")]
        public string StoreCreationReason { get; set; }
    }
}
