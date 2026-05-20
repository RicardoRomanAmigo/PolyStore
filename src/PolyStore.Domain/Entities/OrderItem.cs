using System;

namespace PolyStore.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }

    // --- Relacion con al cabecera (Orden) ---
    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }

    // --- Relacion con el catalogo (Product) ---
    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }

    // --- Datos de venta ---
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    // Constructor vacio requerido por Entity Framework Core
    private OrderItem(){}

    // Constructor principal de negocio
    public OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        if(productId == Guid.Empty)
            throw new ArgumentException("Product cannot be empty");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative");

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}