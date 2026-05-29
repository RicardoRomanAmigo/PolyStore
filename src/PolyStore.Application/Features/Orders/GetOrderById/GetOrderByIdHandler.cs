using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Domain.Exceptions; 
using FluentValidation;

namespace PolyStore.Application.Features.Orders.GetOrderById;

public class GetOrderByIdHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IValidator<GetOrderByIdRequest> _validator; 

    public GetOrderByIdHandler(IOrderRepository orderRepository, IValidator<GetOrderByIdRequest> validator)
    {
        _orderRepository = orderRepository;
        _validator = validator;
    }

    public async Task<OrderDetailResponse?> ExecuteAsync(GetOrderByIdRequest request)
    {
        // --- 1. VALIDACIÓN DE DATOS (FluentValidation) ---
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            throw new PolyStore.Domain.Exceptions.ValidationException(errors); // El Middleware enviará un 400
        }
        // --- 2. LOGICA DE NEGOCIO --- Buscamos el pedido en la BD con todos sus includes necesarios
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);

        if (order is null)
            throw new NotFoundException($"El pedido con ID {request.OrderId} no fue encontrado.");

        // 2. Mapeamos la entidad de dominio y sus OrderItems al DTO detallado
        return new OrderDetailResponse(
            order.Id,
            order.OrderDate,
            order.TotalAmount, 
            order.Status,
            order.CustomerEmail,
            order.UserId,
            order.OrderItems.Select(item => new OrderItemDetailResponse(
                item.ProductId,
                item.Product?.Name ?? "Producto no disponible", // Evitamos romper la web si el producto se borrase
                item.Quantity,
                item.UnitPrice,
                item.Quantity * item.UnitPrice // Calculamos el subtotal de la línea
            )).ToList()
        );
    }
}