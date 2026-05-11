using FluentValidation;

namespace PolyStore.Application.Features.Products.UpdateProduct;

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
         //Reglas para el nombre
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.");

        //Reglas para el Precio
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser un valor positivo.");

        // Reglas para la Imagen
        RuleFor(x => x.MainImage)
            .NotEmpty().WithMessage("Debes proporcionar una imagen principal.");
        
        // Puedes añadir mas reglas para Tags, Styck, etc...
    }
}