using OK_OnBoarding.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class CustomerToken : Token
    {
        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; } // The Customer who the token was generated for
    }
}
