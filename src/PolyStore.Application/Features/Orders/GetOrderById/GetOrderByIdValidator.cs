using FluentValidation;

namespace PolyStore.Application.Features.Orders.GetOrderById;

public class GetOrderByIdValidator : AbstractValidator<GetOrderByIdRequest>
{
    public GetOrderByIdValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El ID del pedido es obligatorio");
    }
}