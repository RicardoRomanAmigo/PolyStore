using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging; // Añadimos logging para dejar rastro en producción <-------------------------------------------------------
using PolyStore.Application.Abstractions.Persistence;

namespace PolyStore.Application.Features.Orders.UpdateOrderStatusToPaid;

public class UpdateOrderStatusToPaidHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly UpdateOrderStatusToPaidValidator _validator;
    private readonly ILogger<UpdateOrderStatusToPaidHandler> _logger; // Recomendado para trazabilidad <----------------------------------------

    public UpdateOrderStatusToPaidHandler(
        IOrderRepository orderRepository,
        UpdateOrderStatusToPaidValidator validator,
        ILogger<UpdateOrderStatusToPaidHandler> logger)
    {
        _orderRepository = orderRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(UpdateOrderStatusToPaidRequest request)
    {
        // 1. Validar el formato del Request
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 2. Buscar el pedido en la base de datos
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
        if(order is null)
        {
            throw new Exception($"No se encontró ningún pedido con el ID {request.OrderId}.");
        }

        // ---------------------------------------------------------------------
        // PASO COMPLEMENTARIO: GUARDA DE IDEMPOTENCIA (Short-Circuit)                  <------------------------------------------------------
        // ---------------------------------------------------------------------
        // Evaluamos el estado antes de procesar. (Asumo que tu entidad expone un Status o propiedad equivalente, ej: OrderStatus.Paid o string "Paid")
        // Nota: Ajusta 'order.Status == OrderStatus.Paid' o 'order.IsPaid' según tu modelo real.
        if (order.Status == "Paid") 
        {
            _logger.LogInformation(
                "Idempotencia activada: El pedido {OrderId} ya fue procesado y pagado previamente. Se ignora el reintento del webhook.", 
                request.OrderId);
                
            // Retornamos true para que el controlador devuelva un 200 OK a Stripe y detenga los reintentos.
            return true; 
        }
        // ---------------------------------------------------------------------

        // 3 y 4. Ejecutamos el método de dominio de la Entidad
        // Ahora este método solo se ejecutará si el pedido está en "Pending".
        order.CompletePayment(request.PaymentIntentId);

        // 5. Descontamos el stock usando el método implementado en la entidad producto ReduceStock
        foreach(var item in order.OrderItems)
        {
            if (item.Product is null)
            {
                throw new Exception($"Integrity error: Product with ID {item.ProductId} is not loaded.");
            }
            // La entidad reduce el stock y se pone en'SoldOut' si llega a cero automaticamente
            item.Product.ReduceStock(item.Quantity);
        }

        // 6. Persistir el cambio de estado en Postgres
        return await _orderRepository.SaveChangesAsync();
    }
}