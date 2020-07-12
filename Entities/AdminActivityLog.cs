using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class AdminActivityLog
    {
        [Key]
        public Guid Id { get; set; }
        public int ActionCarriedOut { get; set; }
        public Guid AdminId { get; set; }

        public DateTime DateOfAction { get; set; }
        public string ReasonOfAction { get; set; }
        public Guid StoreId { get; set; }

        public Admin Admin { get; set; }
    }
}
