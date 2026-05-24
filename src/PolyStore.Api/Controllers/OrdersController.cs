using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.Features.Orders.CreateOrder;

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderHandler _createOrderHanlder;

    public OrdersController(CreateOrderHandler createOrderHandler)
    {
        _createOrderHanlder = createOrderHandler;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder(CreateOrderRequest request)
    {
        // El Handler valida, comprueba stock/precios, crea el pedido y guarda en BD.
        // Mantenemos ExecuteAsync como en los productos
        var result = await _createOrderHanlder.ExecuteAsync(request);

        return Ok(result);
    }
}