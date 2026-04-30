using Microsoft.EntityFrameworkCore;
using PolyStore.Domain.Entities;

namespace PolyStore.Infrastructure.Persistence.Context;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) {}

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Ignore(p => p.Gallery);

            entity.Property<List<string>>("_gallery")
                .HasColumnName("Gallery")
                .HasColumnType("text[]");

            entity.Property(p => p.Price)
                .HasColumnType("numeric(18,2)");

            entity.Property(p => p.Tags)
                .HasColumnType("Text[]");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            //*****Hacer que el Email y el Usename sean unicos para evitar duplicados (falta mas que esto)
            entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.Property(u => u.Role).IsRequired();
        });
    }
}