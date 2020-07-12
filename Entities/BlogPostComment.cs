using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class BlogPostComment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid BlogPostId { get; set; }
        public string CommenterName { get; set; }
        public string CommenterEmail { get; set; }
        public string Comment { get; set; }
        public DateTime DateOfComment { get; set; }
        public bool Visibility { get; set; }

        [ForeignKey("Admin")]
        public Guid VisibilitySetBy { get; set; }


        public Admin Admin { get; set; } //Admin who set the visibility
    }
}
