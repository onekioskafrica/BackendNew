using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public int CategoryId { get; set; }

        public Product Product { get; set; }
    }
}
