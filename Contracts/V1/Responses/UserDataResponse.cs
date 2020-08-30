using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Responses
{
    public class UserDataResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string ProfilePicUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Bank { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public bool IsGoogleRegistered { get; set; }
        public bool IsFacebookRegistered { get; set; }
        public bool IsUpdateComplete { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? DateRegistered { get; set; }
    }
}
