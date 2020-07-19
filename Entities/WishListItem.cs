using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class WishListItem
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        [ForeignKey("WishList")]
        public int WishListId { get; set; }
        public bool IsActive { get; set; } //To show if the item is active in the WishList to prevent it from being added again
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public WishList WishList { get; set; } // The WishList this WishListItem belongs to
        public Product Product { get; set; } // The Product that identifies this WishListItem
    }
}
