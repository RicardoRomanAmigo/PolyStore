using PolyStore.Application.Abstractions.Services;
using Stripe;

namespace PolyStore.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    public PaymentService()
    {
        // Aquí configuraro mi API Key, posiblemente inyectada vía IConfiguration
        StripeConfiguration.ApiKey = "sk_test_tu_key_aqui";
    }

    public async Task<string> CreatePaymentIntentAsync(Guid id, decimal amount)
    {
        var options = new PaymentIntentCreateOptions
        {
            // Convertimos a centimos/moneda menor porque Stripe trabaja así
            Amount = (long)(amount * 100),
            Currency ="eur",
            Metadata = new Dictionary<string, string>
            {
                {"OrderId", id.ToString() }
            }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);

        //Devolvemos el ID de la pasarela
        return intent.Id;
    }

    public async Task<bool> IsPaymentCompletedAsync(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        var intent = await service.GetAsync(paymentIntentId);

        return intent.Status == "succeeded";
    }
}