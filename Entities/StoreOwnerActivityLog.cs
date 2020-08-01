using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class StoreOwnerActivityLog
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("StoreOwner")]
        public Guid StoreOwnerId { get; set; }

        [ForeignKey("Store")]
        public Guid? StoreId { get; set; }
        public string Action { get; set; }
        public DateTime? DateOfAction { get; set; }

        public StoreOwner StoreOwner { get; set; }
        public Store Store { get; set; }

    }
}
