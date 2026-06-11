using PolyStore.Application.Abstractions.Persistence;

namespace PolyStore.Application.Features.Orders.HandlePaymentFailed;

public class HandlePaymentFailedHandler
{
    private readonly IOrderRepository _orderRepository;

    public HandlePaymentFailedHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task ExecuteAsync(HandlePaymentFailedRequest request)
    {
        // 1. Buscar la orden usando el nuevo método adaptado a Stripe
        var order = await _orderRepository.GetByPaymentIntentIdAsync(request.PaymentIntentId);

        if(order == null)
        {
            // Retorno silencioso para que el Webhook de Stripe no reintente infinitamente
            // si por alguna razón nos manda un ID inexistente.
            return;
        }

        // 2. Método de dominio de la entidad Order
        order.MarkAsFailed();

        // 3. Persistir cambios en PostgreSQL
        await _orderRepository.SaveChangesAsync();
    }
}