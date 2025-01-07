using System;
using OrderAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderAPI.Database
{
    public class OrderContext : DbContext
    {
        public OrderContext()
        {
        }

        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        {
        
        }

        public DbSet<Order> Order { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(gen_random_uuid())");

                entity.Property(e => e.CompanyName).IsRequired();

                entity.Property(e => e.ProductId).IsRequired();

                entity.Property(e => e.ProductName).IsRequired();

                entity.Property(e => e.ProductQuantity).IsRequired();
            });
        }
    }
}