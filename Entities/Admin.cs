using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Admin
    {
        [Key]
        public Guid AdminId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Privileges { get; set; }
        public Guid CreatedBy { get; set; }
        public bool IsActive { get; set; }


        public ICollection<AdminActivityLog> AdminActivityLogs { get; set; }
        public ICollection<AdminSelfEditHistory> AdminSelfEditHistories { get; set; }

    }
}
