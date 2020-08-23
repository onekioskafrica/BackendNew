using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class AddCartItemRequest
    {
        [Required]
        public int CartId { get; set; }
        [Required]
        public string SessionId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public Guid StoreId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
