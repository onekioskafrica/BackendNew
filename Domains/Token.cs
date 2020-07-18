using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Domains
{
    public class Token
    {
        public int Id { get; set; }
        public string TheToken { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsUsed { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string StatusOperation { get; set; }
    }
}
