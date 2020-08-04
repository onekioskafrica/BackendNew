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
        public Guid StoreOwnerId { get; set; }

        [Required]
        public string StoreName { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(16, ErrorMessage = "Minimum length of characters should be 11 and maximum of 16 characters.", MinimumLength = 11)]
        public string StorePhoneNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string StoreEmailAddress { get; set; }

        public IFormFile Logo { get; set; }

        [Required]
        public string StoreIntro { get; set; }

        public string StoreCreationReason { get; set; }
    }
}
