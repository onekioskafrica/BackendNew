﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class ActivateAdminRequest
    {
        [Required]
        public Guid PerformerId { get; set; }

        [Required]
        public Guid AdminId { get; set; }

        [Required]
        public bool Activate { get; set; }

        [Required]
        public string Reason { get; set; }
    }
}
