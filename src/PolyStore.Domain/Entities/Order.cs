using System;
using System.Collections.Generic;

namespace PolyStore.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public User? User { get; private set; } 
    public string CustomerEmail { get; private set; }
    public DateTimeOffset OrderDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; }

    // --- Lineas de pedido encapsuladas ---
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    //Constructor vacio para Entity Framework
    private Order()
    {
        CustomerEmail = string.Empty;
        Status = "Pending";
    }

    // Constructor principal de negocio
    public Order(Guid? userId, string customerEmail, IEnumerable<OrderItem> items)
    {
        if(string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer emai cannot be empty");

        if(items == null || !items.Any())
            throw new ArgumentException("An order must contain at least one item");

        Id = Guid.NewGuid();
        UserId = userId;
        CustomerEmail = customerEmail.ToLower().Trim();
        OrderDate = DateTime.UtcNow;
        Status = "Pending";

        foreach (var item in items)
        {
            _orderItems.Add(item);
        }    

        TotalAmount = _orderItems.Sum(item => item.Quantity * item.UnitPrice);
    }

    // Metodos de Dominio
    public void CompletePayment()
    {
        if(Status != "Pending")
            throw new InvalidOperationException("Only pending orders can be paid");

        Status = "Paid";   
    }

    public void Cancel()
    {
        if(Status == "Shipped")
            throw new InvalidOperationException("Shipped orders cannot be cancelled");
        
        Status = "Cancelled";
    }
}
