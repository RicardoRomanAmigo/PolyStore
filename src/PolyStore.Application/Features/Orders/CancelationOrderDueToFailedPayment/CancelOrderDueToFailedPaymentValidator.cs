using FluentValidation;
using FluentValidation.Validators;

namespace PolyStore.Application.Features.Orders.CancelOrderDueToFailedPayment;

public class CancelOrderDueToFailedPaymentValidator : AbstractValidator<CancelOrderDueToFailedPaymentRequest>
{
    public CancelOrderDueToFailedPaymentValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El ID del pedido es obligatorio");
    }
}