using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class CreateStoreBankAccountInformationRequest
    {
        [Required]
        [JsonPropertyName("store_id")]
        public Guid StoreId { get; set; }

        [Required]
        [JsonPropertyName("bank")]
        public string Bank { get; set; }

        [JsonPropertyName("bank_code")]
        public string BankCode { get; set; }

        [Required]
        [JsonPropertyName("bank_account_name")]
        public string AccountName { get; set; }

        [Required]
        [JsonPropertyName("bank_account_number")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("bvn_number")]
        public string BvnNumber { get; set; }
    }
}
