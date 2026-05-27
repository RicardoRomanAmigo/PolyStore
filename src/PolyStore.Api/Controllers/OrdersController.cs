using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.Features.Orders.CreateOrder;
using PolyStore.Application.Features.Orders.GetOrdersByUserId;

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderHandler _createOrderHanlder;
    private readonly GetOrdersByUserIdHandler _getOrdersHandler; // <--- Inyectamos el nuevo handler

    public OrdersController(CreateOrderHandler createOrderHandler, GetOrdersByUserIdHandler getOrdersHandler)
    {
        _createOrderHanlder = createOrderHandler;
        _getOrdersHandler = getOrdersHandler;                       // <--- Lo pasamos por el constructor
    }

    // 1. Endpoint existente: Crear pedidos
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder(CreateOrderRequest request)
    {
        // El Handler valida, comprueba stock/precios, crea el pedido y guarda en BD.
        // Mantenemos ExecuteAsync como en los productos
        var result = await _createOrderHanlder.ExecuteAsync(request);

        return Ok(result);
    }

    // 2. NUEVO Endpoint: Historial de pedidos del usuario
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<OrderSummaryResponse>>> GetOrdersByUserId([FromRoute] Guid userId)
    {
        // Construimos el Request interno con el ID que viene en la URL de la web
        var request = new GetOrdersByUserIdRequest(userId);

        // Ejecutamos la consulta a través de su handler
        var response = await _getOrdersHandler.ExecuteAsync(request);

        // Devolvemos el 200 OK con el listado limpio para el frontend
        return Ok(response);
    }
}