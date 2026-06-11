using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.Abstractions.Services;
using PolyStore.Application.Features.Orders.CancelOrderDueToFailedPayment;
using PolyStore.Application.Features.Orders.UpdateOrderStatusToPaid;

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly UpdateOrderStatusToPaidHandler _updateOrderStatusToPaidHandler;
    private readonly CancelOrderDueToFailedPaymentHandler _cancelOrderHandler; //<---
    

    public PaymentsController(
        IPaymentService paymentService,
        UpdateOrderStatusToPaidHandler updateOrderStatusToPaidHandler,
        CancelOrderDueToFailedPaymentHandler cancelOrderHandler)  //<---
    {
        _paymentService = paymentService;
        _updateOrderStatusToPaidHandler = updateOrderStatusToPaidHandler;
        _cancelOrderHandler = cancelOrderHandler;
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

        // Usamos nuestro DTO PaymentWebhookResult en lugar de una tupla
        var orderData = await _paymentService.GetOrderDataFromWebhookAsync(json, signature);

        if (orderData != null)
        {
            switch (orderData.Status)  //<-----
            {
                case "payment_intent.succeeded":
                    var paidRequest = new UpdateOrderStatusToPaidRequest(orderData.OrderId, orderData.PaymentIntentId);
                    await _updateOrderStatusToPaidHandler.ExecuteAsync(paidRequest);
                    break;

                case "payment_intent.payment_failed":
                    // Invocamos TU Handler, pasando TU Request
                    var failedRequest = new CancelOrderDueToFailedPaymentRequest(
                        orderData.OrderId, 
                        orderData.ErrorMessage
                    );
                    await _cancelOrderHandler.ExecuteAsync(failedRequest);
                    break;

                default:
                    // Ignoramos otros estados
                    break;
            }
        }

        return Ok();
    }
}