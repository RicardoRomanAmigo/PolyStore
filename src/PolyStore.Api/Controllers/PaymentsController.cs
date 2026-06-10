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
        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault() ?? string.Empty;

        // Aseguramos que el cuerpo de la petición esté al inicio
        if (Request.Body.CanSeek)
        {
            Request.Body.Position = 0;
        }

        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        // Recibimos ambos datos desde el servicio
        var orderData = await _paymentService.GetOrderDataFromWebhookAsync(json, signature);

        if (orderData.HasValue)
        {
            // Usamos los valores de la tupla (orderData.Value.OrderId, etc.)
            var request = new UpdateOrderStatusToPaidRequest(orderData.Value.OrderId, orderData.Value.PaymentIntentId);
            await _updateOrderStatusToPaidHandler.ExecuteAsync(request);
        }

        return Ok();
    }
}