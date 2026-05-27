using Microsoft.EntityFrameworkCore;
using PolyStore.Domain.Entities;

namespace PolyStore.Infrastructure.Persistence.Context;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();

    // --- Order DBSets <----------------------------------------------------------------
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();


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

        // --- Configuracion de Order <----------------------------------------------------------------
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            // configuramos la navegación indicando cuál es su campo privado de respaldo:
            entity.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Le decimos a EF Core que acceda a través del campo privado directamente
            entity.Navigation(o => o.OrderItems)
                .HasField("_orderItems")
                .UsePropertyAccessMode(PropertyAccessMode.Field);    

            entity.Property(o => o.TotalAmount)
                .HasColumnType("numeric(18,2)");

            entity.Property(o => o.CustomerEmail)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(30);

            // Relacion opcional con User (compra como invitado/registrado)
            entity.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // --- Configuracion de OrderItem <----------------------------------------------------------------
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);

            entity.Property(oi => oi.UnitPrice)
                .HasColumnType("numeric(18,2)");

            // Relación con el Producto
            entity.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Evita borrar un producto si ya está comprado
        });
    }
}