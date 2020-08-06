using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace OK_OnBoarding.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Admin>()
                .HasIndex(a => new { a.FirstName, a.LastName, a.Email, a.PhoneNumber });
            builder.Entity<AdminActivityLog>()
                .HasIndex(a => new { a.DateOfAction });
            builder.Entity<AdminSelfEditHistory>()
                .HasIndex(a => a.AdminId);
            builder.Entity<BlogPostComment>()
                .HasIndex(b => new { b.DateOfComment, b.Visibility, b.VisibilitySetBy });
            builder.Entity<BlogPostImage>()
                .HasIndex(b => b.BlogPostId);
            builder.Entity<BlogPost>()
                .HasIndex(b => new { b.Title, b.IsActive, b.SchedulePublishTime, b.IsPublished, b.PublishedDate, b.PublishedBy });
            builder.Entity<Cart>()
                .HasIndex(c => new { c.CustomerId, c.Status, c.Mobile, c.Email, c.CreatedAt });
            builder.Entity<CartItem>()
                .HasIndex(c => new { c.ProductId, c.CartId });
            builder.Entity<Category>()
                .HasIndex(c => new { c.ParentId, c.Title });
            builder.Entity<Customer>()
                .HasIndex(c => new { c.FirstName, c.LastName, c.Email, c.PhoneNumber });
            builder.Entity<Deliveryman>()
                .HasIndex(d => new { d.RiderId, d.FirstName, d.LastName, d.Email, d.PhoneNumber, d.DateOfBirth, d.State, d.IsVerified, d.IsEnabled, d.IsActive });
            builder.Entity<Order>()
                .HasIndex(o => new { o.Status, o.SubTotal, o.ItemDiscount, o.Tax, o.Shipping, o.Total, o.Promo, o.Discount, o.GrandTotal, o.FirstName, o.LastName, o.Mobile, o.Email, o.City, o.State, o.CreatedAt });
            builder.Entity<Privilege>()
                .HasData( 
                new Privilege() { Id = 1, Action = "Create Other Admin" },
                new Privilege() { Id = 2, Action = "Deactivate Other Admin" },
                new Privilege() { Id = 3, Action = "Create Blogpost" },
                new Privilege() { Id = 4, Action = "Publish Blogpost" },
                new Privilege() { Id = 5, Action = "Deactivate Blogpost" },
                new Privilege() { Id = 6, Action = "Approve Store Creation" },
                new Privilege() { Id = 7, Action = "Deactivate Store" },
                new Privilege() { Id = 8, Action = "Activate Deliveryman"}
                );
            builder.Entity<Product>()
                .HasIndex(p => new { p.Name, p.Brand, p.InStock, p.Model, p.TypeId });
            builder.Entity<ProductCategory>()
                .HasIndex(p => new { p.ProductId, p.CategoryId });
            builder.Entity<ProductImage>()
                .HasIndex(p => p.ProductId);
            builder.Entity<ProductPricing>()
                .HasIndex(p => new { p.Price, p.SalePrice, p.SaleStartDate, p.SaleEndDate });
            builder.Entity<ProductReview>()
                .HasIndex(p => new { p.Title, p.Rating, p.IsPublished, p.CreatedAt, p.PublishedAt });
            builder.Entity<Store>()
                .HasIndex(s => new { s.StoreOwnerId, s.StoreId, s.StoreName, s.DateCreated, s.IsActivated });
            builder.Entity<StoreOwner>()
                .HasIndex(s => new { s.FirstName, s.LastName, s.PhoneNumber, s.DateOfBirth, s.Email, s.ReferredBy, s.IsVerified, s.IsFacebookRegistered, s.IsGoogleRegistered });
            builder.Entity<StoreOwnerActivityLog>()
                .HasIndex(s => new { s.DateOfAction });
            builder.Entity<StoresBankAccount>()
               .HasIndex(s => new { s.StoreId, s.BankCode, s.AccountNumber, s.BvnNumber });
            builder.Entity<StoresBusinessInformation>()
                .HasIndex(s => new { s.City, s.State, s.TaxIdentificationNumber, s.BusinessRegistrationNumber, s.VatRegistered, s.CompanyLegalName });
            builder.Entity<SuperAdminActivityLog>()
                .HasIndex(s => new { s.AdminId, s.StoreId, s.DateOfAction });
            builder.Entity<Transactions>()
                .HasIndex(t => new { t.Code, t.Type, t.Mode, t.Status, t.CreatedAt });
            builder.Entity<WishList>()
                .HasIndex(w => w.CreatedAt );
            
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminActivityLog> AdminActivityLogs { get; set; }
        public DbSet<AdminSelfEditHistory> AdminSelfEditHistory { get; set; }
        public DbSet<BlogPostComment> BlogPostComments { get; set; }
        public DbSet<BlogPostImage> BlogPostImages { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerToken> CustomerTokens { get; set; }
        public DbSet<Deliveryman> DeliveryMen { get; set; }
        public DbSet<DeliverymanToken> DeliverymenTokens { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Privilege> Priviliges { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<ProductPricing> ProductPricing { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreOwner> StoreOwners { get; set; }
        public DbSet<StoreOwnerActivityLog> StoreOwnerActivityLogs { get; set; }
        public DbSet<StoreOwnerToken> StoreOwnerTokens { get; set; }
        public DbSet<StoresBankAccount> StoresBankAccounts { get; set; }
        public DbSet<StoresBusinessInformation> StoresBusinessInformation { get; set; }
        public DbSet<SuperAdmin> SuperAdmin { get; set; }
        public DbSet<SuperAdminActivityLog> SuperAdminActivityLogs { get; set; }
        public DbSet<SuperAdminAdminRoleEditHistory> SuperAdminAdminRoleEditHistories { get; set; }
        public DbSet<SuperAdminSelfEditHistory> SuperAdminSelfEditHistories { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<WarrantyTypes> WarrantyTypes { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }
    }
}
