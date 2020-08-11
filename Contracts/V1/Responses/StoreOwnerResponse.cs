using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Responses
{
    public class StoreOwnerResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ProfilePicUrl { get; set; }
        public string Email { get; set; }
        public string ReferredBy { get; set; }
        public bool IsOneKioskContractAccepted { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsFacebookRegistered { get; set; } = false;
        public bool IsGoogleRegistered { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }
        public DateTime? DateRegistered { get; set; }
    }
}
