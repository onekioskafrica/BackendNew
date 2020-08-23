using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Coupon
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsStoreOwnerConfigured { get; set; }

        [ForeignKey("Store")]
        public Guid? StoreId { get; set; }

        public bool IsForAllStoresOwnByAStoreOwner { get; set; }
        
        [ForeignKey("StoreOwner")]
        public Guid? StoreOwnerId { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }
        public bool IsActive { get; set; }


        [ForeignKey("Admin")]
        public Guid? AdminId { get; set; }
        public bool IsAdminConfigured { get; set; }

        
        
        public bool IsForCategory { get; set; }
        public int CategoryId { get; set; }
        public bool IsForProduct { get; set; }
        public Guid? ProductId { get; set; }
        public bool IsForAllProducts { get; set; }
        public bool IsForShipping { get; set; }
        public bool IsForPrice { get; set; }
        
        public bool IsPercentageDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PercentageDiscount { get; set; }

        public bool IsAmountDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountDiscount { get; set; }

        public bool IsSetPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SetPrice { get; set; }

        public bool IsSlotSet { get; set; }
        public int AllocatedSlot { get; set; }
        public int SlotUsed { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        public Store Store { get; set; }
        public StoreOwner StoreOwner { get; set; }
        public Admin Admin { get; set; }
    }
}
