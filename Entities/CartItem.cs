using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        [ForeignKey("Store")]
        public Guid StoreId { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public string SKU { get; set; } // The SKU of the product while purchasing it.

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } // The Price of the product while purchasing it

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }
        public bool IsActive { get; set; } //To show if the item was added and removed later.
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public Cart Cart { get; set; } //The Cart this CartItem belongs to
        public Product Product { get; set; } //The Product that identifies this CartItem

        public Store Store { get; set; }
    }
}
