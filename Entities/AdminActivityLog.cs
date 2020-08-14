using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class AdminActivityLog
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PerformerId { get; set; }

        public Guid? AdminId { get; set; }

        [ForeignKey("Deliveryman")]
        public Guid? DeliverymanId { get; set; }

        [ForeignKey("Category")]
        public int? ProductCategoryId { get; set; }

        [ForeignKey("Product")]
        public Guid? ProductId { get; set; }

        public string Action { get; set; }
        public DateTime? DateOfAction { get; set; }
        public string ReasonOfAction { get; set; }

        [ForeignKey("Store")]
        public Guid? StoreId { get; set; }

        public Admin Admin { get; set; }
        public Store Store { get; set; }
        public Deliveryman Deliveryman { get; set; }
        public Category Category { get; set; }
        public Product Product { get; set; }
    }
}
