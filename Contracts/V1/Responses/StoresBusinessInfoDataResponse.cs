using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Responses
{
    public class StoresBusinessInfoDataResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("store_id")]
        public Guid StoreId { get; set; }

        [JsonPropertyName("line_1")]
        public string Line1 { get; set; }

        [JsonPropertyName("line_2")]
        public string Line2 { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("tax_identification_number")]
        public string TaxIdentificationNumber { get; set; }

        [JsonPropertyName("person_in_charge")]
        public string PersonInCharge { get; set; }

        [JsonPropertyName("business_registration_number")]
        public string BusinessRegistrationNumber { get; set; }

        [JsonPropertyName("vat_information_file_url")]
        public string VatInformationFileUrl { get; set; }

        [JsonPropertyName("vat_registered")]
        public bool VatRegistered { get; set; }

        [JsonPropertyName("seller_vat")]
        public string SellerVat { get; set; }

        [JsonPropertyName("company_legal_name")]
        public string CompanyLegalName { get; set; }
    }
}
