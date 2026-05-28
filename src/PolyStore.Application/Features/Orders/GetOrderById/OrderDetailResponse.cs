using System;
using System.Collections.Generic;

namespace PolyStore.Application.Features.Orders.GetOrderById;

public record OrderDetailResponse(
    Guid OrderId,
    DateTimeOffset OrderDate,
    decimal TotalAmount,
    string Status,
    string CustomerEmail,
    Guid? UserId,
    List<OrderItemDetailResponse> Items // Lista detallada de productos
);

public record OrderItemDetailResponse(
    Guid ProductId,
    string ProductName, // ¡Clave! El frontend quiere ver el nombre del producto, no solo el ID
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);