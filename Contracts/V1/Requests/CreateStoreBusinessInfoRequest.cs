using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class CreateStoreBusinessInfoRequest
    {
        [Required]
        public Guid StoreOwnerId { get; set; }

        [Required]
        public Guid StoreId { get; set; }

        [Required]
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        public string TaxIdentificationNumber { get; set; }

        [Required]
        public string PersonInCharge { get; set; }

        public string BusinessRegistrationNumber { get; set; }

        public IFormFile VatInformationFile { get; set; }

        [Required]
        public bool VatRegistered { get; set; }

        public string SellerVat { get; set; }

        public string CompanyLegalName { get; set; }
    }
}
