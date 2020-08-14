using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Error
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateOfLog { get; set; }
        public string Message { get; set; }
    }
}
