using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class List
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }

        [ForeignKey("Admin")]
        public Guid AdminId { get; set; }

        public string Session { get; set; }
        public string ProductItem { get; set; }
        public decimal EstimatePrice { get; set; }
        public decimal ActualPrice { get; set; }

        public string Status { get; set; } // Request, Estimated, Confirmed, Paid, Failed, Shipped, Delivered, Returned, Completed

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Customer Customer { get; set; }
        public Admin Admin { get; set; }

    }
}
