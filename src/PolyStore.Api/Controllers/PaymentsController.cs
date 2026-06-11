using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.Abstractions.Services;
using PolyStore.Application.Features.Orders.HandlePaymentFailed;// <--- Namespace de tu nueva Feature
using PolyStore.Application.Features.Orders.UpdateOrderStatusToPaid;

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly UpdateOrderStatusToPaidHandler _updateOrderStatusToPaidHandler;
    private readonly HandlePaymentFailedHandler _handlePaymentFailedHandler; // <--- Nueva dependencia

    public PaymentsController(
        IPaymentService paymentService,
        UpdateOrderStatusToPaidHandler updateOrderStatusToPaidHandler,
        HandlePaymentFailedHandler handlePaymentFailedHandler)
    {
        _paymentService = paymentService;
        _updateOrderStatusToPaidHandler = updateOrderStatusToPaidHandler;
        _handlePaymentFailedHandler = handlePaymentFailedHandler;
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

        // Recibimos la tupla de 4 valores desde tu servicio actual
        var orderData = await _paymentService.GetOrderDataFromWebhookAsync(json, signature);

        if (orderData.HasValue)
        {
            switch (orderData.Value.Status.ToLower().Trim())  //<-----
            {
                case "succeeded":
                    var paidRequest = new UpdateOrderStatusToPaidRequest(orderData.Value.OrderId, orderData.Value.PaymentIntentId);
                    await _updateOrderStatusToPaidHandler.ExecuteAsync(paidRequest);
                    break;

                case "failed":
                    // Invocamos tu nueva Feature pasando el PaymentIntentId para liberar stock y cancelar reserva
                    var failedRequest = new HandlePaymentFailedRequest(orderData.Value.PaymentIntentId);
                    await _handlePaymentFailedHandler.ExecuteAsync(failedRequest);
                    break;

                default:
                    // Ignoramos pacíficamente cualquier otro estado que devuelva la pasarela
                    break;
            }
        }

        return Ok();
    }
}