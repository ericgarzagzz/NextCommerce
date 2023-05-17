using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NextCommerce.Data.Entities;
using NextCommerce.Data.Enums;

namespace NextCommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ServiceBanner> ServiceBanners { get; set; }
        public DbSet<ProductShowcaseCollection> ProductShowcaseCollections { get; set; }
        public DbSet<ProductShowcaseCollectionItem> ProductShowcaseCollectionItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ShoppingSession> ShoppingSessions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderLineItem> OrderLineItems { get; set; }
        public DbSet<WishItem> Wishlist { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().OwnsOne(p => p.Dimension);

            builder.Entity<Setting>()
                .Property(s => s.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (SettingType)Enum.Parse(typeof(SettingType), v));

            builder.Entity<ProductShowcaseCollection>()
                .Property(s => s.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (ProductShowcaseType)Enum.Parse(typeof(ProductShowcaseType), v));

            builder.Entity<BuyedWithProduct>()
                .HasKey(e => new { e.ProductId, e.BuyedWithId });

            builder.Entity<BuyedWithProduct>()
                .HasOne(e => e.Product)
                .WithMany(e => e.BuyedWithProducts)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderDetails>()
                .OwnsOne(e => e.ShippingAddress);

            builder.Entity<OrderDetails>()
                .OwnsOne(e => e.BillingAddress);

            base.OnModelCreating(builder);
        }
    }
}