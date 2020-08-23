using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Order
    {
        /*
         * The table is to manage each store orders
         */
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public string SessionId { get; set; }
        public string Token { get; set; }
        public string Status { get; set; } // Can be New, Checkout, Paid, Failed, Shipped, Delivered, Returned, Completed

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Tax { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Shipping { get; set; } //The Shipping charges of the Order Items

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; } // The total price of the Order including tax and shipping. It excludes the Item Discount
        public string Promo { get; set; } // The promo code of the Order

        [Column(TypeName = "decimal(18, 2)")]
        public decimal StoreOwnerShippingDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]

        public decimal StoreOwnerPriceDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]

        public decimal OnekioskShippingDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]

        public decimal OnekioskPriceDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PriceDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Discount { get; set; } // The total discount of the Order based on the promo code or store discount.

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GrandTotal { get; set; } // The grand total of the order to be paid by the buyer
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Line1 { get; set; } //The first line to store address
        public string Line2 { get; set; } //The second line to store address
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public Customer Customer { get; set; } // The Customer that made this order
        
    }
}
