using System;

namespace PolyStore.Application.Features.Orders.GetOrdersByUserId;

public record OrderSummaryResponse(
    Guid OrderId,
    DateTimeOffset OrderDate,
    decimal TotalAmount, //<----
    string Status,
    int TotalItems
);