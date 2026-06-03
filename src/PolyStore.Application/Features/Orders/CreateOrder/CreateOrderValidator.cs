using FluentValidation;

namespace PolyStore.Application.Features.Orders.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        //Regla para el Email
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("El email del cliente es obligatorio")
            .EmailAddress().WithMessage("El formato del email no es valido");

        //Regla para los items
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("El pedido debe contener al menos un articulo");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("El ID del producto es obligatorio");
            
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero");
        });

        // 3. NUEVA REGLA: Validar la dirección de envío
        // Aquí delegamos la validación a la clase UserAddressValidator
        RuleFor(x => x.Address)
            .NotNull().WithMessage("Los datos de envío son obligatorios")
            .SetValidator(new OrderAddressValidator());
    }
}