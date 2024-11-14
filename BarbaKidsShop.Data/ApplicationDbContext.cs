using System.Reflection.Emit;
using BarbaKidsShop.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarbaKidsShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ShippingDetail> ShippingDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Category>()
           .HasMany(c => c.Products)
           .WithOne(p => p.Category)
           .HasForeignKey(p => p.CategoryId);

            builder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            builder.Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId);

            builder.Entity<Order>()
                .HasOne(o => o.ShippingDetail)
                .WithOne(sd => sd.Order)
                .HasForeignKey<Order>(o => o.ShippingDetailId);

            builder
                .Entity<Category>()
                .HasData(
                    new Category { CategoryId = 1, Name = "Clothes" },
                    new Category { CategoryId = 2, Name = "Toys" },
                    new Category { CategoryId = 3, Name = "Shoes" },
                    new Category { CategoryId = 4, Name = "Accessories" },
                    new Category { CategoryId = 5, Name = "Educational Materials" });
        }
    }
}
