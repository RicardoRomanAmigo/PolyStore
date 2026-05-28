using System;

namespace PolyStore.Application.Features.Orders.GetOrderById;

public record GetOrderByIdRequest(Guid OrderId);