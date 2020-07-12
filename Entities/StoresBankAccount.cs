using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class StoresBankAccount
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Store")]
        public Guid StoreId { get; set; }
        public string Bank { get; set; }
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BvnNumber { get; set; }

        public Store Store { get; set; } // The Store that owns this StoreBankAccount
    }
}
