using PolyStore.Application.Abstractions.Services;
using Stripe;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyStore.Application.DTOs;

namespace PolyStore.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly string _webhookSecret;
    private readonly PaymentIntentService _paymentIntentService;

    public PaymentService(IConfiguration configuration)
    {
        // Configuramos la API Key desde appsettings.json
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"]
            ?? throw new InvalidOperationException("Stripe:SecretKey no encontrado en la configuración."); //<-----------
        // Obtenemos el secreto del webhook
        _webhookSecret = configuration["Stripe:WebhookSecret"]
            ?? throw new ArgumentNullException(nameof(configuration), "Stripe:WebhookSecret no encontrado en configuración"); //<------------

        _paymentIntentService = new PaymentIntentService();
    }

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(Guid id, decimal amount) //<-----------
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

        var intent = await _paymentIntentService.CreateAsync(options);

        if (string.IsNullOrEmpty(intent.ClientSecret))  //<-----------
        {
            throw new InvalidOperationException("Stripe no devolvio un ClientSecret valido");
        }

        // Retornamos el record con el Id (para persistir en BD) y el ClientSecret (para el frontend)  <------------
        return new PaymentIntentResult(intent.Id, intent.ClientSecret);
    }

    public async Task<bool> IsPaymentCompletedAsync(string paymentIntentId)
    {
        var intent = await _paymentIntentService.GetAsync(paymentIntentId);
        return intent.Status == "succeeded";
    }

    public async Task<PaymentWebhookResult?> GetOrderDataFromWebhookAsync(string json, string signature)
    {
        try
        {
            // 1. Construir y validar el evento se Stripe
            // Stripe lanzará una excepción si la firma no es válida
            var stripeEvent = EventUtility.ConstructEvent(json, signature, _webhookSecret);

            //2. Filtrar solo los eventos que nos interesen
            // Extraemos el objeto (PaymentIntent)
            if (stripeEvent.Data.Object is PaymentIntent paymentIntent)
            {
                if (paymentIntent.Metadata.TryGetValue("OrderId", out var orderIdString) &&
                    Guid.TryParse(orderIdString, out var orderId))
                {
                    return new PaymentWebhookResult(
                        OrderId: orderId,
                        PaymentIntentId: paymentIntent.Id,
                        Status: stripeEvent.Type, // Devolvemos el evento crudo: "payment_intent.payment_failed"
                        ErrorMessage: paymentIntent.LastPaymentError?.Message
                    );
                }
            }
        }
        catch (StripeException)
        {
            // Retornamos null para que el controlador sepa que no hay datos procesables
            return null;
        }
        catch (Exception)
        {
            return null;
        }

        return null;
    }

}