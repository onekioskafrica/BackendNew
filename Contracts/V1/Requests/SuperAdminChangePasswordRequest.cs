using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class SuperAdminChangePasswordRequest
    {
        [Required]
        [JsonPropertyName("super_admin_id")]
        public int SuperAdminId { get; set; }

        [Required]
        [JsonPropertyName("old_password")]
        public string OldPassword { get; set; }

        [Required]
        [JsonPropertyName("new_password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must meet minimum complexity requirements")]
        [StringLength(250, ErrorMessage = "Password must be a minimum of 8 characters in length", MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}
