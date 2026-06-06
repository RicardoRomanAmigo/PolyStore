using PolyStore.Application.Abstractions.Services;
using Stripe;
using Microsoft.Extensions.Configuration;

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
                {"OrderId", id.ToString() }
            }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);

        return intent.Id;
    }

    public async Task<Guid?> GetOrderIdFromWebhookAsync(string json, string signature)
    {
        try
        {
            // Validamos la firma con el secreto inyectado
            var stripeEvent = EventUtility.ConstructEvent(json, signature, _webhookSecret);

            // Usamos el string literal para evitar errores de resolución del tipo 'Events'
            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null && paymentIntent.Metadata.ContainsKey("OrderId"))
                {
                    return Guid.Parse(paymentIntent.Metadata["OrderId"]);
                }
            }
        }
        catch (StripeException)
        {
            // En un caso real, aquí deberías loguear el error
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