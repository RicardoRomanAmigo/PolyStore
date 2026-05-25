using FluentValidation;

namespace PolyStore.Application.Features.Orders.UpdateOrderStatusToPaid;

public class UpdateOrderStatusToPaidValidator : AbstractValidator<UpdateOrderStatusToPaidRequest>
{
    public UpdateOrderStatusToPaidValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El ID del pedido es obligatorio");
    }
}