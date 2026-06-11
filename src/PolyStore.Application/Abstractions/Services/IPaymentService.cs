namespace PolyStore.Application.Abstractions.Services;

public interface IPaymentService
{
    // Prepara el pago en la pasarela y devuelve el identificador/token necesario
    Task<string> CreatePaymentIntentAsync(Guid id, decimal amount);

    // Opcional: Verifica el estado real en la pasarela antes de procesar cambios
    Task<bool> IsPaymentCompletedAsync(string paymentIntentId);

    // Antes devolvía: Task<(Guid OrderId, string PaymentIntentId)?>
    // Ahora le añadimos el Status del evento ("succeeded" o "failed") y el ErrorMessage opcional:
    Task<(Guid OrderId, string PaymentIntentId, string Status, string? ErrorMessage)?> GetOrderDataFromWebhookAsync(string json, string signature);
}