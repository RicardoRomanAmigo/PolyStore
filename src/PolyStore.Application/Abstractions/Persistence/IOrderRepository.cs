namespace PolyStore.Application.Abstractions.Persistence;

using System;
using System.Threading.Tasks;
using PolyStore.Domain.Entities;

public interface IOrderRepository
{
    // Siguiendo tu patrón de "AddProductAsync" -> "AddOrderAsync"
    Task AddOrderAsync(Order order);

    Task<Order?> GetOrderByIdAsync(Guid id);

    // NUEVO: Necesario para que el Webhook localice la orden mediante Stripe
    Task<Order?> GetByPaymentIntentIdAsync(string paymentIntentId);

    // Calcado a tu IProductRepository
    Task<bool> SaveChangesAsync();

    // Metodo para Obtener Order por userId
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
}