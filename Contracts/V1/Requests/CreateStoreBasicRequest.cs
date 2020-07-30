using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class CreateStoreBasicRequest
    {
        [Required]
        [JsonPropertyName("store_owner_id")]
        public Guid StoreOwnerId { get; set; }

        [Required]
        [JsonPropertyName("store_name")]
        public string StoreName { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(16, ErrorMessage = "Minimum length of characters should be 11 and maximum of 16 characters.", MinimumLength = 11)]
        [JsonPropertyName("phone_number")]
        public string StorePhoneNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [JsonPropertyName("store_email")]
        public string StoreEmailAddress { get; set; }

        [JsonPropertyName("logo")]
        public IFormFile Logo { get; set; }

        [Required]
        [JsonPropertyName("store_intro")]
        public string StoreIntro { get; set; }

        [JsonPropertyName("store_creation_reason")]
        public string StoreCreationReason { get; set; }
    }
}
