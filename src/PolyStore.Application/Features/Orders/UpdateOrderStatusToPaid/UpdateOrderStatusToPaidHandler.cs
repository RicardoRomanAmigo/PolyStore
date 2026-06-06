using System;
using System.Threading.Tasks;
using FluentValidation;
using PolyStore.Application.Abstractions.Persistence;

namespace PolyStore.Application.Features.Orders.UpdateOrderStatusToPaid;

public class UpdateOrderStatusToPaidHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly UpdateOrderStatusToPaidValidator _validator;

    public UpdateOrderStatusToPaidHandler(
        IOrderRepository orderRepository,
        UpdateOrderStatusToPaidValidator validator)
    {
        _orderRepository = orderRepository;
        _validator = validator;
    }

    public async Task<bool> ExecuteAsync(UpdateOrderStatusToPaidRequest request)
    {
        // 1. Validar el formato del Request
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 2. Buscar el pedido en la base de datos
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
        if(order is null)
        {
            throw new Exception($"No se encontró ningún pedido con el ID {request.OrderId}.");
        }

        // 3 y 4. Ejecutamos el método de dominio de la Entidad
        // Si el estado no es "Pending", la propia entidad lanzará la InvalidOperationException
        // y el ExceptionMiddleware la capturará limpiamente.
        order.CompletePayment(request.PaymentIntentId);

        // 5. Descontamos el stock usando el metodo implementado en la entidad producto ReduceStock
        foreach(var item in order.OrderItems)
        {
            if (item.Product is null)
            {
                throw new Exception($"Integrity error: Product with ID {item.ProductId} is not loaded.");
            }
            // La entidad reduce el stock y se pone en'SoldOut' si llega a cero automaticamente
            item.Product.ReduceStock(item.Quantity);
        }

        // 6. Persistir el cambio de estado en Postgres
        return await _orderRepository.SaveChangesAsync();
    }
}