using System;
using System.Collections.Generic;
using System.Linq;

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
    public DateTimeOffset? ReserveUntil { get; private set; } 
    public string? PaymentIntentId { get; private set; } //<---- Nueva propiedad
    public  DateTimeOffset? PaymentDate { get; private set; } //<---- Nueva propiedad

    // Campos de envío (ahora forman parte del pedido)
    public string FullName { get; private set; } = string.Empty;
    public string Dni { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;

    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    private Order()
    {
        CustomerEmail = string.Empty;
        Status = "Pending";
    }

    public Order(Guid? userId, string customerEmail, IEnumerable<OrderItem> items)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email cannot be empty");

        if (items == null || !items.Any())
            throw new ArgumentException("An order must contain at least one item");

        Id = Guid.NewGuid();
        UserId = userId;
        CustomerEmail = customerEmail.ToLower().Trim();
        OrderDate = DateTime.UtcNow;
        Status = "Pending";
        // Implementamos la reserva al generar la orden y establecemos un tiempo 
        ReserveUntil = DateTimeOffset.UtcNow.AddMinutes(15);

        foreach (var item in items)
        {
            _orderItems.Add(item);
        }

        TotalAmount = _orderItems.Sum(item => item.Quantity * item.UnitPrice);
    }

    // Método para encapsular la asignación de dirección
    public void SetShippingAddress(string fullName, string dni, string phoneNumber, string address, string city, string postalCode)
    {
        FullName = fullName;
        Dni = dni;
        PhoneNumber = phoneNumber;
        Address = address;
        City = city;
        PostalCode = postalCode;
    }

    public void CompletePayment(string paymentIntentId) // <--- pasamos en parametro de id del intento de pago
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Only pending orders can be paid");

        // Validar que no haya expirado antes de dejar pagar 
        if (ReserveUntil < DateTimeOffset.UtcNow)
            throw new InvalidOperationException("The order reservation has expired");

        PaymentIntentId = paymentIntentId; // <--- asignamos el valor a la propiedad
        PaymentDate = DateTimeOffset.UtcNow; // <--- Registramos cuándo se confirmó
        Status = "Paid";
        ReserveUntil = null; // Ya no hay reserva pendiente 
    }

    public void Cancel()
    {
        if (Status == "Shipped")
            throw new InvalidOperationException("Shipped orders cannot be cancelled");

        Status = "Cancelled";

        //  Aquí es donde aplicamos la lógica de restitución 
        foreach (var item in OrderItems)
        {
            // El producto sabe qué hacer con este parámetro 'true'
            item?.Product?.AddStock(item.Quantity, isReturnFromCancellation: true);
        }
    }
}