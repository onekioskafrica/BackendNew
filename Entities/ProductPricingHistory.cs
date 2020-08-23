using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class ProductPricingHistory
    {
        [Key]
        public int Id { get; set; }

        public Guid StoreOwnerId { get; set; }
        public Guid ProductId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OldPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OldSalePrice { get; set; }

        public DateTime? OldSaleStartDate { get; set; }
        public DateTime? OldSaleEndDate { get; set; }
        public DateTime? DateOfAction { get; set; }
    }
}
