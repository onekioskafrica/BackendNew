using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class BlogPost
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public bool IsActive { get; set; }
        public DateTime SchedulePublishTime { get; set; }
        public bool IsPublished { get; set; }
        public DateTime PublishedDate { get; set; }

        [ForeignKey("Admin")]
        public Guid PublishedBy { get; set; }


        public Admin Admin { get; set; } //Admin who published the BlogPost
        public ICollection<BlogPostImage> BlogPostImages { get; set; } // All the images of this blog post
    }
}
