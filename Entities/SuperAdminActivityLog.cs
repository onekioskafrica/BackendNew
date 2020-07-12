using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class SuperAdminActivityLog
    {
        [Key]
        public Guid Id { get; set; }
        public int ActionCarriedOutId { get; set; }
        public Guid? AdminId { get; set; }
        public DateTime DateOfAction { get; set; }
        public string ReasonForAction { get; set; }

        [ForeignKey("Store")]
        public Guid? StoreId { get; set; }

        public Admin Admin { get; set; } // In the case of an activity happening on an Admin.. This holds the Admin
        public Store Store { get; set; } // In the case of an activity happening on a Store.. This holds the Store
    }
}
