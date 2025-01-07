using System;
using OrderAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderAPI.Database
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor to pass in the DbContextOptions
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Define DbSets 
        public DbSet<Order> Orders { get; set; }
        //public DbSet<UpdateOrderModel> Orders { get; set; }

        // Define a DbSet for the keyless entity 'StatusList'
        public DbSet<StatusList> StatusLists { get; set; }  // We are defining it as a DbSet, but it's keyless

        // Configure entities in OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        // Mark 'StatusList' as keyless (no primary key)
            modelBuilder.Entity<StatusList>().HasNoKey();

        // Optionally, you can configure other properties, if needed
        // For example, you could specify the table name using:
        // modelBuilder.Entity<StatusList>().ToTable("YourTableName");
        }
    }
}