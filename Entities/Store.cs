using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Store
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("StoreOwner")]
        public Guid StoreOwnerId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string StorePhoneNumber { get; set; }
        public string StoreEmailAddress { get; set; }
        public string LogoUrl { get; set; }
        public string StoreIntro { get; set; }
        public string StoreCreationReason { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActivated { get; set; }

        public StoreOwner StoreOwner { get; set; } //Owner of Store
    }
}
