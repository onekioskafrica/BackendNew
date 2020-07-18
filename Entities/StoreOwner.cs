using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class StoreOwner
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ProfilePicUrl { get; set; }
        public string EmailAddress { get; set; }
        public string ReferredBy { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsOneKioskContractAccepted { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsFacebookRegistered { get; set; } = false;
        public bool IsGoogleRegistered { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }
        public DateTime? DateRegistered { get; set; }
    }
}
