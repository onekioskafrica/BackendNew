using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Responses
{
    public class CheckoutResponse
    {
        public Guid CustomerId { get; set; }
        public string SessionId { get; set; }
        public string Token { get; set; } 
        public string Status { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; } //Total Price of goods bought

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; } //Subtotal + Shipping

        public string Promo { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PriceDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Discount { get; set; } //Total Discount

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GrandTotal { get; set; }

        public string Line1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public Customer Customer { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
