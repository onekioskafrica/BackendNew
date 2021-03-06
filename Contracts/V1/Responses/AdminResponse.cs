﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Responses
{
    public class AdminResponse
    {
        public Guid AdminId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Privileges { get; set; }
        public Guid CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
