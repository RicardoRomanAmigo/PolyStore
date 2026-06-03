using PolyStore.Application.DTOs;

namespace PolyStore.Application.Features.Orders.CreateOrder;

public record CreateOrderRequest(
    Guid? UserId, 
    string CustomerEmail,
    List<OrderItemDto> Items,
    OrderAddressDto Address // Cambiamos UserAddressDto por OrderAddressDto
);