using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Domains
{
    public class StoreDiscountDetail
    {
        public int CartItemId { get; set; }
        public Guid StoreId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountOnShipping { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountOnPrice { get; set; }
    }
}
