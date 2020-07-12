using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public int ParentId { get; set; } //To Identify the Parent Category
        public string Title { get; set; }
        public string MetaTitle { get; set; }
        public string Slug { get; set; } //To be added to its URL
        public string Description { get; set; }
    }
}
