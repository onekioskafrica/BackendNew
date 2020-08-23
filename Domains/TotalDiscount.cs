using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Domains
{
    public class TotalDiscount
    {
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalShippingDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPriceDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalStoreOwnerShippingDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalStoreOwnerPriceDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalOnekioskShippingDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalOnekioskPriceDiscount { get; set; }
    }
}
