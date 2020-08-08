using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; }

        public Product Product { get; set; }
    }
}
