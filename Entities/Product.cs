using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Store")]
        public Guid StoreId { get; set; }
        public string Name { get; set; }
        public string MetaTitle { get; set; }
        public string Brand { get; set; }
        public int InStock { get; set; } //The number of available unit in stock
        public DateTime? DateCreated { get; set; }
        public string Model { get; set; }
        public string MainColor { get; set; }
        public string ProductLine { get; set; }
        public string ColorFamily { get; set; }
        public string MainMaterial { get; set; }
        public string ProductDescription { get; set; }
        public string YoutubeVideoId { get; set; }
        public string Highlights { get; set; }
        public string Notes { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Length { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Width { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Height { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Weight { get; set; }
        public string ProductWarranty { get; set; }
        public string WarrantyTypes { get; set; }
        public string WarrantyAddress { get; set; }
        public string Certification { get; set; }
        public string ProductionCountry { get; set; }
        public string ManufacturerNote { get; set; }
        public string CareLabel { get; set; }
        public bool IsVisible { get; set; } // For StoreOwners to hide their products
        public bool IsActive { get; set; } // For Admin to deactivate or activate a product

        public Store Store { get; set; } // The Store that this product belong to
        public ProductPricing ProductPricing { get; set; }  // The ProductPricing for this Product
        public ICollection<ProductCategory> ProductCategories { get; set; } // All the Categories of the Product
        public ICollection<ProductImage> ProductImages { get; set; } // the Product Images of this Product
        public ICollection<ProductReview> ProductReviews { get; set; } // the Product Reviews of this Product
    }
}
