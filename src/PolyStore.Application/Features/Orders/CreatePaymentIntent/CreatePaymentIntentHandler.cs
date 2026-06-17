using FluentValidation;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Application.Abstractions.Services;
using PolyStore.Application.DTOs; // 1. Namespace donde reside PaymentIntentResult <--------------------

namespace PolyStore.Application.Features.Orders.CreatePaymentIntent;

public class CreatePaymentIntentHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IValidator<CreatePaymentIntentRequest> _validator;

    public CreatePaymentIntentHandler( IOrderRepository orderRepository, IPaymentService paymentService, IValidator<CreatePaymentIntentRequest> validator)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _validator = validator;
    }

    public async Task<string> ExecuteAsync(CreatePaymentIntentRequest request)
    {
        //1. Validacion
        var validationResult = await _validator.ValidateAsync(request);
        if(!validationResult.IsValid)
            throw new PolyStore.Domain.Exceptions.ValidationException(validationResult.Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()));

        //2.Obtener pedido 
        var order = await _orderRepository.GetOrderByIdAsync(request.Id);
        if(order is null)
            throw new Exception($"No ser encontro ningun pedido con el ID {request.Id}.");

        //3. Llamar a la pasarela (Ahora devuelve el DTO con ambas propiedades) <--------------------
        PaymentIntentResult paymentResult = await _paymentService.CreatePaymentIntentAsync(order.Id, order.TotalAmount);

        //4. Asignar ID de pasarela y ClientSecret a la entidad (usando método dominio)
        order.SetPaymentIntent(paymentResult.PaymentIntentId, paymentResult.ClientSecret);

        //5. Persistir usando el patron de SaveChangesAsync
        await _orderRepository.SaveChangesAsync();

        // Devolvemos el token que el Frontend necesita para renderizar el formulario seguro
        return paymentResult.ClientSecret;
    }
}
