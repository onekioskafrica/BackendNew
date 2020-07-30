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
        [JsonPropertyName("store_id")]
        public Guid StoreId { get; set; }

        [Required]
        [JsonPropertyName("address_line_1")]
        public string Line1 { get; set; }

        [JsonPropertyName("address_line_2")]
        public string Line2 { get; set; }

        [Required]
        [JsonPropertyName("city")]
        public string City { get; set; }

        [Required]
        [JsonPropertyName("state")]
        public string State { get; set; }

        [Required]
        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("tax_identification_number")]
        public string TaxIdentificationNumber { get; set; }

        [Required]
        [JsonPropertyName("person_in_charge")]
        public string PersonInCharge { get; set; }

        [JsonPropertyName("business_registration_number")]
        public string BusinessRegistrationNumber { get; set; }

        [JsonPropertyName("vat_information_file")]
        public IFormFile VatInformationFile { get; set; }

        [Required]
        [JsonPropertyName("vat_registered")]
        public bool VatRegistered { get; set; }

        [JsonPropertyName("seller_vat")]
        public string SellerVat { get; set; }

        [JsonPropertyName("company_legal_name")]
        public string CompanyLegalName { get; set; }
    }
}
