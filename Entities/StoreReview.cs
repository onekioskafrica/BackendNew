using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class StoreReview
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Store")]
        public Guid StoreId { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public Guid ParentId { get; set; } // The Parent Review
        public string Title { get; set; }
        public int Rating { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Content { get; set; }


        public Store Store { get; set; } //The Store that owns this review
        public Customer Customer { get; set; } // Reviewer
    }
}
