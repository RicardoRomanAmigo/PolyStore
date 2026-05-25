using System;

namespace PolyStore.Application.Features.Orders.UpdateOrderStatusToPaid;

public record UpdateOrderStatusToPaidRequest(Guid OrderId);