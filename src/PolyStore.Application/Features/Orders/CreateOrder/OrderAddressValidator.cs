namespace PolyStore.Application.Features.Orders.CreateOrder;
using FluentValidation;
using PolyStore.Application.DTOs;

public class OrderAddressValidator : AbstractValidator<OrderAddressDto>
{
    public OrderAddressValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().WithMessage("El nombre es obligatorio");
        RuleFor(x => x.Dni).NotEmpty().WithMessage("El DNI es obligatorio");
        RuleFor(x => x.Address).NotEmpty().WithMessage("La dirección es obligatoria");
        RuleFor(x => x.City).NotEmpty().WithMessage("La ciudad es obligatoria");
        RuleFor(x => x.PostalCode).NotEmpty().WithMessage("El código postal es obligatorio");
    }
}