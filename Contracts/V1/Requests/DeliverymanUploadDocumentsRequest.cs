using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class DeliverymanUploadDocumentsRequest
    {
        [Required]
        public Guid DeliverymanId { get; set; }

        [Required]
        public IFormFile PassportPhoto { get; set; }

        [Required]
        public IFormFile GovernmentIssuedIdFront { get; set; }

        [Required]
        public IFormFile GovernmentIssuedIdBack { get; set; }

        [Required]
        public IFormFile UtilityBill { get; set; }
    }
}
