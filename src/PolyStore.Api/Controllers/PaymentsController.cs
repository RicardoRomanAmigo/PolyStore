using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.Features.Orders.UpdateOrderStatusToPaid;
using System.Threading.Tasks;

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly UpdateOrderStatusToPaidHandler _updateOrderStatusToPaidHandler;

    public PaymentsController(UpdateOrderStatusToPaidHandler updateOrderStatusToPaidHandler)
    {
        _updateOrderStatusToPaidHandler = updateOrderStatusToPaidHandler;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook([FromBody] UpdateOrderStatusToPaidRequest request)
    {
        // El webhook de la pasarela golpea aquí. 
        // Tu Handler ejecuta la validación, busca el pedido, dispara el método de dominio .CompletePayment()
        // y guarda los cambios en Postgres.
        var success = await _updateOrderStatusToPaidHandler.ExecuteAsync(request);

        if (!success)
        {
            return BadRequest(new { message = "No se pudo actualizar el estado del pedido." });
        }

        return Ok(new { message = "Pedido pagado con éxito." });
    }
}