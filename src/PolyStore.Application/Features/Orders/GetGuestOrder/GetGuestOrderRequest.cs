using System;

namespace PolyStore.Application.Features.Orders.GetGuestOrder;

public record GetGuestOrderRequest(Guid OrderId, string CustomerEmail);
