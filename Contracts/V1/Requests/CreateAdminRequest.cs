using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class CreateAdminRequest
    {
        [Required]
        [JsonPropertyName("caller_id")]
        public Guid CallerId { get; set; }

        [Required]
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("middle_name")]
        public string MiddleName { get; set; }

        [Required]
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(16, ErrorMessage = "Minimum length of characters should be 11 and maximum of 16 characters.", MinimumLength = 11)]
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must meet minimum complexity requirements")]
        [StringLength(250, ErrorMessage = "Password must be a minimum of 8 characters in length", MinimumLength = 8)]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Required]
        [JsonPropertyName("privileges")]
        public string Privileges { get; set; }
    }
}
