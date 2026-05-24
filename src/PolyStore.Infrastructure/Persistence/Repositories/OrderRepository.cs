using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Infrastructure.Persistence.Context;

namespace PolyStore.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StoreDbContext _context;

    //Inyector del contexto 
    public OrderRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        // Busca el pedido incluyendo sus líneas Y los productos de cada línea
        return await _context.Orders
            .Include(o => o.OrderItems)        // Carga las líneas del pedido
                .ThenInclude(oi => oi.Product) // ¡Súper importante! Carga el objeto Producto de cada línea
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task AddOrderAsync(Order order)
    {
        // Guarda un pedido nuevo
        await _context.Orders.AddAsync(order);
    }

    public async Task<bool> SaveChangesAsync()
    {
        // Devuelve true si se guardo al menos un cambio
        return await _context.SaveChangesAsync() > 0;
    }
}