using Microsoft.AspNetCore.Mvc;
using PolyStore.Application.Features.Orders.CreateOrder;
using PolyStore.Application.Features.Orders.GetOrdersByUserId;
using PolyStore.Application.Features.Orders.GetOrderById; 
using PolyStore.Application.Features.Orders.GetGuestOrder; //  <------------ 

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderHandler _createOrderHandler;
    private readonly GetOrdersByUserIdHandler _getOrdersHandler; 
    private readonly GetOrderByIdHandler _getOrderByIdHandler; 
    private readonly GetGuestOrderHandler _getGuestOrderHandler; // <--- Inyectamos el nuevo handler

    public OrdersController(CreateOrderHandler createOrderHandler, GetOrdersByUserIdHandler getOrdersHandler, GetOrderByIdHandler getOrderByIdHandler, GetGuestOrderHandler getGuestOrderHandler)
    {
        _createOrderHandler = createOrderHandler;
        _getOrdersHandler = getOrdersHandler; 
        _getOrderByIdHandler = getOrderByIdHandler;           
        _getGuestOrderHandler = getGuestOrderHandler; // <--- Lo pasamos por el constructor
    }

    // 1. Endpoint existente: Crear pedidos
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder(CreateOrderRequest request)
    {
        // El Handler valida, comprueba stock/precios, crea el pedido y guarda en BD.
        // Mantenemos ExecuteAsync como en los productos
        var result = await _createOrderHandler.ExecuteAsync(request);

        return Ok(result);
    }

    // 2. Endpoint: Historial de pedidos del usuario
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

    // 3. Detalle profundo de un pedido por su ID 
    [HttpGet("{id:guid}")] // La URL será: GET /api/orders/5702257e-...
    public async Task<IActionResult> GetOrderById([FromRoute] Guid id)
    {
        var request = new GetOrderByIdRequest(id);
        var response = await _getOrderByIdHandler.ExecuteAsync(request);

        // Si existe, devolvemos el DTO completo con sus items y nombres de productos
        return Ok(response);
    }

    // 4. NUEVO ENDPOINT: Para invitados <-----------------------------------------------------------
    [HttpGet("guest/{id:guid}")] // Ruta: GET /api/orders/guest/{id}?email=xxx
    public async Task<IActionResult> GetGuestOrder([FromRoute] Guid id, [FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { Message = "El correo electronico es obligatorio para validar el acceso."});
        }

        var request = new GetGuestOrderRequest(id, email);
        var response = await _getGuestOrderHandler.ExecuteAsync(request);

        // Si el handler devuelve null (porque el ID no existe o el email no coincide), 404 por seguridad
        if(response is null)
        {
            return NotFound(new { Message = "No se encontro ningun pedido que coincida con los datos proporcionados "});
        }

        return Ok(response);
    }
}