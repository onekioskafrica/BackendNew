using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class BlogPostImage
    {
        [Key]
        public Guid BlogPostId { get; set; }
        public string ImageUrl { get; set; }
    }
}
