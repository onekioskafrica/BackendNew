using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class CreateProductCategoryRequest
    {
        [Required]
        public Guid AdminId { get; set; }
        public int ParentId { get; set; }

        [Required]
        public string Title { get; set; }

        public string MetaTitle { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
    }
}
