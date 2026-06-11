using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using FluentValidation;
using PolyStore.Application.Abstractions.Persistence;

namespace PolyStore.Application.Features.Orders.CancelOrderDueToFailedPayment;

public class CancelOrderDueToFailedPaymentHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly CancelOrderDueToFailedPaymentValidator _validator;
    private readonly ILogger<CancelOrderDueToFailedPaymentHandler> _logger;

    public CancelOrderDueToFailedPaymentHandler(
        IOrderRepository orderRepository,
        CancelOrderDueToFailedPaymentValidator validator,
        ILogger<CancelOrderDueToFailedPaymentHandler> logger)
    {
        _orderRepository = orderRepository;
        _validator = validator;
        _logger = logger;
    }
    
    public async Task<bool> ExecuteAsync(CancelOrderDueToFailedPaymentRequest request)
    {
        // 1. Validar el Request
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 2. Buscar el pedido en PostgreSQL
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
        if(order is null)
        {
            throw new Exception($"No se encontró ningún pedido con el ID {request.OrderId}.");
        }

        // ---------------------------------------------------------------------
        // GUARDAS DE IDEMPOTENCIA Y SEGURIDAD
        // ---------------------------------------------------------------------
        // Caso A: El pedido ya está cancelado (Duplicado de red)
        if(order.Status == "Cancelled")
        {
            _logger.LogInformation(
                "Idempotencia de cancelacion: El pedido {OrderId} ya se encuentra cancelado. Se ignora el reintento del webHook.",
                request.OrderId);
            return true;  // Retornamos true para responder 200 OK a Stripe y frenar reintentos
        }

        // Caso B: El pedido ya está PAGADO (Alerta de conflicto)
        // Si Stripe te dice que falló pero en tu BD ya sale como Pagado por un evento previo,
        // NO debemos cancelarlo bajo ningún concepto. Registramos un log de advertencia.
        if (order.Status == "Paid")
        {
            _logger.LogWarning(
                "Conflicto Crítico: Se recibió un evento de pago fallido para el pedido {OrderId}, pero el pedido ya figura como PAGADO en el sistema. No se aplicará la cancelación.",
                request.OrderId);
            return true; // Retornamos true para decirle a Stripe que procesamos el aviso, evitando bucles de reintentos
        }
        // ---------------------------------------------------------------------

        // 3. Ejecutar la logica de negocio encapsulada en tu Entidad de Dominio
        _logger.LogWarning(
            "Cancelado el pedido {OrderId} debido a un fallo en la pasarela de pago. Motivo reportado: {Reason}",
            request.OrderId,
            request.ErrorMessage ?? "No especificado por Stripe");
        
        // El metodo de order cambia el estado a Cancellet y ejecuta AddStock de los productos automaticamente
        order.Cancel();

        // 4. Guardar los cambios de forma atomica en Postgres
        return await _orderRepository.SaveChangesAsync();
    }
}