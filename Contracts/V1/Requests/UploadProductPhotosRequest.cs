using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class UploadProductPhotosRequest
    {
        [Required]
        public Guid StoreId { get; set; }

        [Required]
        public Guid StoreOwnerId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public List<IFormFile> ProductPhotos { get; set; }
    }
}
