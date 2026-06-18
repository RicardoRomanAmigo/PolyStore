using FluentValidation;

namespace PolyStore.Application.Features.Orders.CreatePaymentIntent;
public class CreatePaymentIntentValidator : AbstractValidator<CreatePaymentIntentRequest>
{
    public CreatePaymentIntentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del pedido es obligatorio para generar el pago.");
    }
}