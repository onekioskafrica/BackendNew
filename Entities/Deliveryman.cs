using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Deliveryman
    {
        public Guid Id { get; set; }
        public string RiderId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsEnabled { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public bool IsGoogleRegistered { get; set; } = false;
        public bool IsFacebookRegistered { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }
        public DateTime? DateRegistered { get; set; }
    }
}
