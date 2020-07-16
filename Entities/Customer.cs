using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { set; get; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ProfilePicUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }


        public ICollection<Cart> Carts { get; set; } //All the carts belonging to a Customer
        public ICollection<Order> Orders { get; set; } // All the orders belonging to the Customer
    }
}
