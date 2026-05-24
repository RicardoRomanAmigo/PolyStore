namespace PolyStore.Application.Abstractions.Persistence;

using System;
using System.Threading.Tasks;
using PolyStore.Domain.Entities;

public interface IOrderRepository
{
    // Siguiendo tu patrón de "AddProductAsync" -> "AddOrderAsync"
    Task AddOrderAsync(Order order);

    Task<Order?> GetOrderByIdAsync(Guid id);

    // Calcado a tu IProductRepository
    Task<bool> SaveChangesAsync();
}