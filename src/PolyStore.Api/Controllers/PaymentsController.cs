using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.Abstractions.Services;
using PolyStore.Application.Features.Orders.UpdateOrderStatusToPaid;

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly UpdateOrderStatusToPaidHandler _updateOrderStatusToPaidHandler;

    public PaymentsController(
        IPaymentService paymentService, 
        UpdateOrderStatusToPaidHandler updateOrderStatusToPaidHandler)
    {
        _paymentService = paymentService;
        _updateOrderStatusToPaidHandler = updateOrderStatusToPaidHandler;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];

        // Delegamos la verificación y extracción a tu PaymentService
        var orderId = await _paymentService.GetOrderIdFromWebhookAsync(json, signature);

        if (orderId.HasValue)
        {
            // Ejecutamos tu lógica de negocio
            await _updateOrderStatusToPaidHandler.ExecuteAsync(new UpdateOrderStatusToPaidRequest(orderId.Value));
        }

        // Siempre devolvemos 200 OK para que Stripe no reintente el envío
        return Ok();
    }
}