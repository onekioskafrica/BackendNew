using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class EnableUserCreationRequest
    {
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string OTP { get; set; }
    }
}
