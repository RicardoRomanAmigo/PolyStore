using PolyStore.Application.Abstractions.Services;
using Stripe;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolyStore.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly string _webhookSecret;

    public PaymentService(IConfiguration configuration)
    {
        // Configuramos la API Key desde appsettings.json
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        // Obtenemos el secreto del webhook
        _webhookSecret = configuration["Stripe:WebhookSecret"]
            ?? throw new ArgumentNullException("Stripe:WebhookSecret no encontrado en configuración");
    }

    public async Task<string> CreatePaymentIntentAsync(Guid id, decimal amount)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = "eur",
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", id.ToString() }
            }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);

        return intent.Id;
    }

    public async Task<(Guid OrderId, string PaymentIntentId, string Status, string? ErrorMessage)?> GetOrderDataFromWebhookAsync(string json, string signature)
    {
        try
        {
            // 1. Construir y validar el evento de Stripe
            var stripeEvent = EventUtility.ConstructEvent(json, signature, _webhookSecret);

            // 2. Filtrar los eventos que nos interesan (Éxito o Fallo)
            if (stripeEvent.Type == "payment_intent.succeeded" || stripeEvent.Type == "payment_intent.payment_failed")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // 3. Validar que el objeto contenga los metadatos de nuestra orden
                if (paymentIntent != null && paymentIntent.Metadata.TryGetValue("OrderId", out var orderIdString))
                {
                    // 4. Parsear el GUID de la orden
                    if (Guid.TryParse(orderIdString, out var orderId))
                    {
                        // 5. Determinar el estado agnóstico para la capa de Application
                        string status = stripeEvent.Type == "payment_intent.succeeded" ? "succeeded" : "failed";
                        
                        // 6. Extraer el motivo del fallo si Stripe lo proporciona
                        string? errorMessage = paymentIntent.LastPaymentError?.Message;

                        // Retornamos la tupla de 4 elementos requerida por la interfaz
                        return (orderId, paymentIntent.Id, status, errorMessage);
                    }
                }
            }
        }
        catch (StripeException)
        {
            // LOGUEAR EL ERROR AQUÍ: Es vital para detectar intentos de fraude o errores de configuración
            // _logger.LogError(ex, "Error al procesar el webhook de Stripe");
            return null;
        }

        return null;
    }

    public async Task<bool> IsPaymentCompletedAsync(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        var intent = await service.GetAsync(paymentIntentId);

        return intent.Status == "succeeded";
    }
}