using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Responses
{
    public class DeliverymanResponse
    {
        public Guid Id { get; set; }
        public string RiderId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string ProfilePicUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneTypeUsed { get; set; }
        public bool InternetAccess { get; set; }
        public string MeansOfTransport { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Bank { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string PassportUrl { get; set; }
        public string GovernmentIssuedIDFront { get; set; }
        public string GovernmentIssuedIDBack { get; set; }
        public string UtilityBillUrl { get; set; }
        public bool IsCompanyDriver { get; set; }
        public Guid? CompanyId { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsEnabled { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public bool IsGoogleRegistered { get; set; } = false;
        public bool IsFacebookRegistered { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }
        public DateTime? DateRegistered { get; set; }
    }
}
