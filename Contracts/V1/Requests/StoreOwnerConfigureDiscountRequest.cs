using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class StoreOwnerConfigureDiscountRequest
    {
        [Required]
        public Guid StoreOwnerId { get; set; }

        [Required]
        public bool IsForAllStoresOwnByAStoreOwner { get; set; }

        //Only when IsForAllStoresOwnByAStoreOwner is false is StoreId required
        public Guid StoreId { get; set; }

        public string Title { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public bool IsForShipping { get; set; }
        public bool IsForPrice { get; set; }

        public bool IsForCategory { get; set; }
        public int CategoryId { get; set; }
        public bool IsForProduct { get; set; }
        public Guid ProductId { get; set; }
        public bool IsForAllProduct { get; set; }


        // Only one of these next three can be true
        public bool IsPercentageDiscount { get; set; }
        public bool IsAmountDiscount { get; set; }
        public bool IsSetPrice { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public decimal PercentageDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountDiscount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SetPrice { get; set; }


        public bool IsSlotSet { get; set; }
        public int AllocatedSlot { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
