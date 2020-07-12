using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class StoresBusinessInformation
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Store")]
        public Guid StoreId { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string PersonInCharge { get; set; }
        public string BusinessRegistrationNumber { get; set; }
        public string VatInformationFileUrl { get; set; }
        public bool VatRegistered { get; set; }
        public string SellerVat { get; set; }
        public string CompanyLegalName { get; set; }


        public Store Store { get; set; } // The Store that owns this StoresBusinessInformation
    }
}
