using OK_OnBoarding.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class DeliverymanToken : Token
    {
        [ForeignKey("Deliveryman")]
        public Guid DeliverymanId { get; set; }
        public Deliveryman Deliveryman { get; set; }
    }
}
