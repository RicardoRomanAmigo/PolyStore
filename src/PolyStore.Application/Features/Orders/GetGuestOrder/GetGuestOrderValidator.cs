using FluentValidation;

namespace PolyStore.Application.Features.Orders.GetGuestOrder;

public class GetGuestOrderValidator : AbstractValidator<GetGuestOrderRequest>
{
    public GetGuestOrderValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El ID dle pedido es obligatorio.");
        
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("El correo electronico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo electronico no es valido.");
    }
}