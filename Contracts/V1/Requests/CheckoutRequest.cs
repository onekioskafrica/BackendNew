using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class CheckoutRequest
    {
        [Required]
        public int CartId { get; set; }

        [Required]
        public string SessionId { get; set; }

        public string CouponCode { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public string Line1 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }
    }
}
