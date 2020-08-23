using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class OneKioskConfiguration
    {
        [Key]
        public int Id { get; set; }

        public string Key { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Value { get; set; }
    }
}
