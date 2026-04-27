using Microsoft.EntityFrameworkCore;
using PolyStore.Domain.Entities;

namespace PolyStore.Infrastructure.Persistence.Context;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) {}

    public DbSet<Product> Products => Set<Product>();

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
    }
}