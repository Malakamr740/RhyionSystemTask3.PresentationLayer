using Microsoft.EntityFrameworkCore;
using RhyionSystemTask3.DataAccessLayer.Models;

namespace RhyionSystemTask3.DataAccessLayer.Context
{
    public class AppDBContext: DbContext
    {
        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=RhyionSystemTask3DB;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);

            modelBuilder.Entity<Payment>()
                .HasKey(p => p.OrderId); 

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId); 

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            base.OnModelCreating(modelBuilder);
        }
        DbSet<User> Users { get; set; }  
        DbSet<Order> Orders { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<OrderProduct> OrderProducts { get; set; }
        DbSet<Payment> Payments { get; set; }


    }
}
