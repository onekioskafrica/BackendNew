using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Store
    {
        [Key]
        public Guid Id { get; set; }
        public string StoreId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string StoreName { get; set; }
        public string LogoUrl { get; set; }
        public string StoreIntro { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string ReferredBy { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsOneKioskContractAccepted { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActivated { get; set; }

    }
}
