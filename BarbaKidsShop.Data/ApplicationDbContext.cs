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

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            // TODO: SEEDING THE DATABASE WHEN CREATED
            //builder
            //    .Entity<Category>()
            //    .HasData(
            //        new Category { Id = 1, Name = "" },
            //        new Category { Id = 2, Name = "" },
            //        new Category { Id = 3, Name = "" },
            //        new Category { Id = 4, Name = "" },
            //        new Category { Id = 5, Name = "" });
        }
    }
}
