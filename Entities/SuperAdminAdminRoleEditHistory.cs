using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class SuperAdminAdminRoleEditHistory
    {
        [Key]
        public Guid Id { get; set; }
        public string OldPrivileges { get; set; }

        [ForeignKey("Admin")]
        public Guid EdittedAdminId { get; set; }

        public Admin Admin { get; set; } // Admin whose details was updated
    }
}
