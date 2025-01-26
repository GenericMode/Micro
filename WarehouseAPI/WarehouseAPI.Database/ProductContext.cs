using System;
using WarehouseAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace WarehouseAPI.Database
{
    public class ProductContext : DbContext
    {
        public ProductContext()
        {
        }

        public ProductContext(DbContextOptions<ProductContext> options)
            : base(options)
        {
        
        }

        public DbSet<Product> Product { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(gen_random_uuid())");

                entity.Property(e => e.ProductId).IsRequired();

                entity.Property(e => e.ProductName).IsRequired();

                entity.Property(e => e.ProductStoredQuantity).IsRequired();

                entity.Property(e => e.ProductStorage).IsRequired();
            });
        }
    }
}
