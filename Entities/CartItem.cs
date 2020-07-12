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

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public string SKU { get; set; } // The SKU of the product while purchasing it.
        public decimal Price { get; set; } // The Price of the product while purchasing it
        public decimal Discount { get; set; } // The Discount of the product while purchasing it
        public int Quantity { get; set; }
        public bool IsActive { get; set; } //To show if the item is active in the cart to prevent it from being added again
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public Cart Cart { get; set; } //The Cart this CartItem belongs to
        public Product Product { get; set; } //The Product that identifies this CartItem
    }
}
