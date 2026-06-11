using PolyStore.Application.DTOs;

namespace PolyStore.Application.Abstractions.Services;

public interface IPaymentService
{
    // Prepara el pago en la pasarela y devuelve el identificador/token necesario
    Task<string> CreatePaymentIntentAsync(Guid id, decimal amount);

    // Opcional: Verifica el estado real en la pasarela antes de procesar cambios
    Task<bool> IsPaymentCompletedAsync(string paymentIntentId);

    // Procesamiento de Webhook: recibe los datos crudos, devuelve nuestro DTO
    // Si la firma es inválida o el pedido no existe, puedes retornar null 
    // o lanzar una excepción personalizada (ej: InvalidWebhookException)
    Task<PaymentWebhookResult?> GetOrderDataFromWebhookAsync(string json, string signature);
}