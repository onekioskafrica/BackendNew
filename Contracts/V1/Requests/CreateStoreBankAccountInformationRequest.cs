using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class CreateStoreBankAccountInformationRequest
    {
        [Required]
        public Guid StoreOwnerId { get; set; }
        [Required]
        public Guid StoreId { get; set; }

        [Required]
        public string Bank { get; set; }

        public string BankCode { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        public string BvnNumber { get; set; }
    }
}
