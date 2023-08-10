using System.Configuration;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace StockHouseApi.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<GroceryItem> StockItems { get; set; }
    //public DbSet<GroceryItem> GroceryItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroceryItem>()
            .HasOne(i => i.User)
            .WithMany(u => u.StockItems)
            .HasForeignKey(i => i.UserId)
            .IsRequired();
    }
}