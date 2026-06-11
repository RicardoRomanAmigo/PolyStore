namespace PolyStore.Application.DTOs;

public record PaymentWebhookResult(
    Guid OrderId, 
    string PaymentIntentId, 
    string Status, 
    string? ErrorMessage
);