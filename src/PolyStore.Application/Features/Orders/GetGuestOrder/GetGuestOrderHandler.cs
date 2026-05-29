using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyStore.Application.Abstractions.Persistence;
// Reutilizamos el dto de GetOrderById
using PolyStore.Application.Features.Orders.GetOrderById;
using PolyStore.Domain.Exceptions;

namespace PolyStore.Application.Features.Orders.GetGuestOrder;

public class GetGuestOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IValidator<GetGuestOrderRequest> _validator;

    public GetGuestOrderHandler(IOrderRepository orderRepository, IValidator<GetGuestOrderRequest> validator)
    {
        _orderRepository = orderRepository;
        _validator = validator;
    }

    public async Task<OrderDetailResponse?> ExecuteAsync(GetGuestOrderRequest request)
    {   
        // --- 1. VALIDACIÓN DE DATOS (FluentValidation) ---
        var validationResult = await _validator.ValidateAsync(request);

        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage.ToArray())
                );
        }

        // --- 2. VALIDACIÓN DE NEGOCIO Y SEGURIDAD --- Buscamos el pedido usando el metodo real del repositorio
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);

        
        // Si el pedido no existe o el correo no coincide con ese registro, NotFound por seguridad
        if(order is null || !order.CustomerEmail.Equals(request.CustomerEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new NotFoundException("No se encontró ningún pedido con los datos proporcionados."); // Provoca un 404 estructurado
        }

        // --- 3. MAPEADO Y RETORNO --- Si el ID y el Email coinciden, mapeamos al DTO y lo devolvemos con seguridad
        return new OrderDetailResponse(
            order.Id,
            order.OrderDate,
            order.TotalAmount,
            order.Status,
            order.CustomerEmail,
            order.UserId,
            order.OrderItems.Select(item => new OrderItemDetailResponse(
                item.ProductId,
                item.Product?.Name ?? "Producto no disponible",
                item.Quantity,
                item.UnitPrice,
                item.Quantity * item.UnitPrice
            )).ToList()
        );
    }
}