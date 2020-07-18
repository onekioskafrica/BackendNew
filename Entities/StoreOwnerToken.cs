using OK_OnBoarding.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class StoreOwnerToken : Token
    {
        [ForeignKey("Store")]
        public Guid StoreOwnerId { get; set; }

        public Store Store { get; set; } // The Store the token was generated for.
    }
}
