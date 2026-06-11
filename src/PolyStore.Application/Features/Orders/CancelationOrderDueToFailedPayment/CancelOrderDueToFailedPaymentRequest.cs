using System;

namespace PolyStore.Application.Features.Orders.CancelOrderDueToFailedPayment;

public record CancelOrderDueToFailedPaymentRequest(Guid OrderId, string? ErrorMessage);