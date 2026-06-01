using FluentValidation;
using PolyStore.Application.DTOs;

namespace PolyStore.Application.Features.Orders.CreateOrder;

public class UserAddressValidator : AbstractValidator<UserAddressDto>
{
    public UserAddressValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio")
            .MaximumLength(150).WithMessage("El nombre es demasiado largo");

        RuleFor(x => x.Dni)
            .NotEmpty().WithMessage("El DNI/NIF es obligatorio")
            .Length(8, 12).WithMessage("El DNI/NIF debe tener entre 8 y 12 caracteres");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("El teléfono es obligatorio")
            .Matches(@"^[0-9+\s]+$").WithMessage("El formato del teléfono no es válido");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("La dirección es obligatoria")
            .MaximumLength(250).WithMessage("La dirección es demasiado larga");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ciudad es obligatoria")
            .MaximumLength(100).WithMessage("La ciudad es demasiado larga");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("El código postal es obligatorio")
            .Matches(@"^\d{5}$").WithMessage("El código postal debe tener 5 dígitos");
    }
}