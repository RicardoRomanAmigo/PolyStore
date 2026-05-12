using FluentValidation;

namespace PolyStore.Application.Features.Authentication.Register;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
    }
}