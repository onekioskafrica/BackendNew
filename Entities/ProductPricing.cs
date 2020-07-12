using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class ProductPricing
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Store")]
        public Guid? StoreId { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public string SellerSku { get; set; }
        public string Variation { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }


        public Store Store { get; set; } // The store where this Product Pricing belongs to
        public Product Product { get; set; } // The product which this Product Pricing describes
    }
}
