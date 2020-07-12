using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Transactions
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        [ForeignKey("Order")]
        public int? OrderId { get; set; }
        public string Code { get; set; } // The Payment Id provided by the payment gateway
        public string Type { get; set; } // The type of order transaction can be either Credit or Debit
        public string Mode { get; set; } // The mode of the order transaction can be Offline, Cash On Delivery, Cheque, Online
        public string Status { get; set; } // The status of the order transaction can be New, Cancelled, Failed, Pending, Declined, Rejected, and Success.
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Content { get; set; } // For additional details of the transaction.


        public Customer Customer { get; set; } // The Customer that owns the transaction
        public Order Order { get; set; } // The Order paid for
    }
}
