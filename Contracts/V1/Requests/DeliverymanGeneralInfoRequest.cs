using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class DeliverymanGeneralInfoRequest
    {
        [Required]
        public Guid DeliverymanId { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string PhoneTypeUsed { get; set; }

        [Required]
        public bool InternetAccess { get; set; }

        [Required]
        public string MeansOfTransport { get; set; }

        public string Bank { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        [Required]
        public bool IsCompanyDriver { get; set; }
    }
}
