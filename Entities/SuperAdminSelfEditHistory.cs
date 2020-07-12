using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class SuperAdminSelfEditHistory
    {
        [Key]
        public Guid Id { get; set; }
        public string OldFirstName { get; set; }
        public string OldMiddleName { get; set; }
        public string OldLastName { get; set; }
        public string OldEmail { get; set; }
        public string OldPassword { get; set; }
        public string OldPhoneNumber { get; set; }
        public DateTime DateEditted { get; set; }
    }
}
