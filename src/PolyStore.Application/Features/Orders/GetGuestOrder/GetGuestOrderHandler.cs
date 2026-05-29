using System;
using System.Linq;
using System.Threading.Tasks;
using PolyStore.Application.Abstractions.Persistence;
// Reutilizamos el dto de GetOrderById
using PolyStore.Application.Features.Orders.GetOrderById;

namespace PolyStore.Application.Features.Orders.GetGuestOrder;

public class GetGuestOrderHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetGuestOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDetailResponse?> ExecuteAsync(GetGuestOrderRequest request)
    {
        // 1. Buscamos el pedido usando el metodo real del repositorio
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);

        //// 2. EL FILTRO DE SEGURIDAD
        // Si el pedido no existe, o existe pero el email que nos pasan NO coincide 
        // con el email que guardó la orden en Postgres, devolvemos null (puerta cerrada).
        if(order is null || !order.CustomerEmail.Equals(request.CustomerEmail, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // 3. Si el ID y el Email coinciden, mapeamos al DTO y lo devolvemos con seguridad
        return new OrderDetailResponse(
            order.Id,
            order.OrderDate,
            order.TotalAmount,
            order.Status,
            order.CustomerEmail,
            order.UserId,
            order.OrderItems.Select(item => new OrderItemDetailResponse(
                item.ProductId,
                item.Product?.Name ?? "Producto no disponible",
                item.Quantity,
                item.UnitPrice,
                item.Quantity * item.UnitPrice
            )).ToList()
        );
    }
}