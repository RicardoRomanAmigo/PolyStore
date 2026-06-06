namespace PolyStore.Application.Abstractions.Services;

public interface IPaymentService
{
    // Prepara el pago en la pasarela y devuelve el identificador/token necesario
    Task<string> CreatePaymentIntentAsync(Guid id, decimal amount);

    // Opcional: Verifica el estado real en la pasarela antes de procesar cambios
    Task<bool> IsPaymentCompletedAsync(string paymentIntentId);

    // Añadimos este nuevo método para procesar el webhook
    Task<(Guid OrderId, string PaymentIntentId)?> GetOrderDataFromWebhookAsync(string json, string signature);
}