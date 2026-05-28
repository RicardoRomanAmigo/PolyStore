using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PolyStore.Application.Abstractions.Persistence;

namespace PolyStore.Application.Features.Orders.GetOrderById;

public class GetOrderByIdHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDetailResponse?> ExecuteAsync(GetOrderByIdRequest request)
    {
        // 1. Buscamos el pedido en la BD con todos sus includes necesarios
        var order = await _orderRepository.GetOrderByIdWithItemsAsync(request.OrderId);

        if (order is null)
            return null; // El controlador se encargará de escupir un 404 Not Found

        // 2. Mapeamos la entidad de dominio y sus OrderItems al DTO detallado
        return new OrderDetailResponse(
            order.Id,
            order.OrderDate,
            order.TotalAmount, 
            order.Status,
            order.CustomerEmail,
            order.UserId,
            order.OrderItems.Select(item => new OrderItemDetailResponse(
                item.ProductId,
                item.Product?.Name ?? "Producto no disponible", // Evitamos romper la web si el producto se borrase
                item.Quantity,
                item.UnitPrice,
                item.Quantity * item.UnitPrice // Calculamos el subtotal de la línea
            )).ToList()
        );
    }
}