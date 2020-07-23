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
        [ForeignKey("StoreOwner")]
        public Guid StoreOwnerId { get; set; }

        public StoreOwner StoreOwner { get; set; } // The StoreOwner the token was generated for.
    }
}
