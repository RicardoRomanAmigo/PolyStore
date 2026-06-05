using Microsoft.EntityFrameworkCore;
using PolyStore.Application.Abstractions.Services;
using PolyStore.Infrastructure.Persistence.Context;

namespace PolyStore.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly StoreDbContext _context;

    public OrderService(StoreDbContext context)
    {
        _context = context;
    }

    public async Task CancelExpiredOrders()
    {
        // 1. Buscar órdenes que están pendientes y cuya fecha de reserva ya pasó
        var expiredOrders = await _context.Orders
            .Where(o => o.Status == "Pending" && o.ReserveUntil < DateTimeOffset.UtcNow)
            .ToListAsync();

        if (!expiredOrders.Any()) return; // Si no hay nada, salimos rápido

        // 2. Iterar y cancelar
        foreach (var order in expiredOrders)
        {
            order.Cancel();
        }

        // 3. Guardar cambios en la base de datos
        // EF Core lo hará dentro de una transacción automáticamente
        await _context.SaveChangesAsync();
    }
}