using System;

namespace PolyStore.Application.Features.Orders.GetOrdersByUserId;

public record OrderSummaryResponse(
    Guid OrderId,
    DateTimeOffset OrderDate,
    decimal TotalAmout,
    string Status,
    int TotalItems
);