using Microsoft.EntityFrameworkCore;
using ShopSphere.Api.Models;

namespace ShopSphere.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Name).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.Property(u => u.Role).HasMaxLength(20);
        });

        b.Entity<Category>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(100).IsRequired();
        });

        b.Entity<Product>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(200).IsRequired();
            e.Property(p => p.Price).HasPrecision(18, 2);
            e.HasIndex(p => p.Name);               // faster search
            e.HasIndex(p => p.CategoryId);
            e.HasOne(p => p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p => p.CategoryId);
        });

        b.Entity<CartItem>(e =>
        {
            e.HasIndex(c => new { c.UserId, c.ProductId }).IsUnique();
        });

        b.Entity<Order>(e =>
        {
            e.Property(o => o.TotalAmount).HasPrecision(18, 2);
            e.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(o => o.UserId);
        });

        b.Entity<OrderItem>(e =>
        {
            e.Property(i => i.UnitPrice).HasPrecision(18, 2);
        });

        // Seed data
        b.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Books" },
            new Category { Id = 3, Name = "Clothing" });

        b.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Wireless Mouse", Description = "2.4GHz ergonomic mouse", Price = 799, Stock = 50, CategoryId = 1, CreatedAt = new DateTime(2026, 1, 1) },
            new Product { Id = 2, Name = "Mechanical Keyboard", Description = "Blue switch, backlit", Price = 2499, Stock = 30, CategoryId = 1, CreatedAt = new DateTime(2026, 1, 1) },
            new Product { Id = 3, Name = "Clean Code", Description = "A Handbook of Agile Software Craftsmanship", Price = 599, Stock = 100, CategoryId = 2, CreatedAt = new DateTime(2026, 1, 1) },
            new Product { Id = 4, Name = "Cotton T-Shirt", Description = "100% cotton, regular fit", Price = 499, Stock = 80, CategoryId = 3, CreatedAt = new DateTime(2026, 1, 1) });
    }
}
