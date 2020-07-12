using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public string SessionId { get; set; }
        public string Token { get; set; } //The unique token associated with the cart to identify the cart over multiple sessions. The same token can also be passed to the Payment Gateway if required.
        public string Status { get; set; } //Can be New, Checkout, Paid, Complete, Abandoned
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Customer Customer { get; set; } //The owner of the cart
        public ICollection<CartItem> CartItems {get; set;} //All the CartItems in the Cart

    }
}
