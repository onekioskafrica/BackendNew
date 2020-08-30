using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public int CartItemId { get; set; }

        public string SessionId { get; set; }

        [ForeignKey("Store")]
        public Guid StoreId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal StoreDiscountOnShipping { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal StoreDiscountOnPrice { get; set; }
        public string PaymentReference { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsSettled { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountPaidToOneKiosk { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountPaidToStore { get; set; }
        public Store Store { get; set; }
    }
}
