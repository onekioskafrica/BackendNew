using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class CreateProductRequest
    {
        [Required]
        public Guid StoreOwnerId { get; set; }
        [Required]
        public Guid StoreId { get; set; }

        [Required]
        public int[] CategoryIds { get; set; }

        [Required]
        public string Name { get; set; }
        public string MetaTitle { get; set; }
        public string Brand { get; set; }

        [Required]
        public int InStock { get; set; } //The number of available unit in stock

        [Required]
        public bool IsVisible { get; set; }
        public string Model { get; set; }
        public string MainColor { get; set; }
        public string ProductLine { get; set; }
        public string ColorFamily { get; set; }
        public string MainMaterial { get; set; }

        [Required]
        public string ProductDescription { get; set; }
        public string YoutubeVideoId { get; set; }
        public string Highlights { get; set; }
        public string Notes { get; set; }

        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string ProductWarranty { get; set; }
        public string WarrantyTypes { get; set; }
        public string WarrantyAddress { get; set; }
        public string Certification { get; set; }
        public string ProductionCountry { get; set; }
        public string ManufacturerNote { get; set; }
        public string CareLabel { get; set; }
        public string SellerSku { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal SalePrice { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }

    }
}
