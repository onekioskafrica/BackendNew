using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class ProductCategory
    {
        [Key]
        public Guid ProductId { get; set; }
        public int CategoryId { get; set; }
    }
}
