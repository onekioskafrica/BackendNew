using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class ProductType
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
    }
}
