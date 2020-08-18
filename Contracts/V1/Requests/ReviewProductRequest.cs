using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class ReviewProductRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        public Guid ParentId { get; set; }
        public string Title { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
    }
}
