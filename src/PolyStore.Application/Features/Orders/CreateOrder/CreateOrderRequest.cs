using System.Net;
using System.Collections.Generic;
using PolyStore.Application.DTOs;

namespace PolyStore.Application.Features.Orders.CreateOrder;

// Definimos el objeto DTO con los datos que permitimos que lleguen desde la Web
public record CreateOrderRequest(
    Guid? UserId, 
    string CustomerEmail,
    List<OrderItemDto> Items
);

