using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyStore.Application.Abstractions.Persistence;

namespace PolyStore.Application.Features.Orders.GetOrdersByUserId;

public class GetOrdersByUserIdHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersByUserIdHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderSummaryResponse>> ExecuteAsync(GetOrdersByUserIdRequest request)
    {
        //1. Buscamos los pedidos del usuario en la base de datos
        var orders = await _orderRepository.GetOrdersByUserIdAsync(request.UserId);

        //2. Mapeamos las entidades de dominio al DTO de salida para el Frontend
        return orders.Select(order => new OrderSummaryResponse(
            order.Id,
            order.OrderDate,
            order.TotalAmount,
            order.Status,
            order.OrderItems.Count
        ));
    }
}