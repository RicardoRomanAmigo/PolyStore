using FluentValidation;
using PolyStore.Application.Features.Authentication.Login;

namespace PolyStore.Application.Features.Authentication.Register;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligaroria.");
    }
}