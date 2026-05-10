using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Application.Abstractions.Authentication; 
using PolyStore.Domain.Exceptions; // <--- Nuestras excepciones
using FluentValidation;           // <--- FluentValidation

namespace PolyStore.Application.Features.Products.CreateLiveProduct;

public class CreateLiveProductHandler
{
    private readonly IProductRepository _repository;
    private readonly IUserContext _userContext; 
    private readonly IValidator<CreateLiveProductRequest> _validator; // 1. Campo privado

    public CreateLiveProductHandler(IProductRepository repository, IUserContext userContext, IValidator<CreateLiveProductRequest> valiator ) // 2. Se inyecta aquí automáticamente
    {
        _repository = repository;
        _userContext = userContext; 
        _validator = valiator;
    }

    public async Task<Guid> ExecuteAsync(CreateLiveProductRequest request)
    {
        // --- VALIDACIÓN DE DATOS (FluentValidation) ---
        // Lo hacemos lo primero de todo para no trabajar en balde si los datos están mal
        // 3. VALIDACIÓN USANDO EL CAMPO INYECTADO en el constructor
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            throw new PolyStore.Domain.Exceptions.ValidationException(errors); // El Middleware atrapa esto y envía 400
        }

        // VALIDACIÓN DE IDENTIDAD Y ROL (Seguridad de Negocio)
        // No solo nos importa que esté autenticado, nos importa QUIÉN es.
        if (!_userContext.IsAuthenticated || _userContext.Role !="Admin")
        {
            throw new UnauthorizedAccessException("Acceso denegado: Se requieren privilegios de administrador para realizar esta acción.");
        }

        // Lógica de negocio: Archivamos el producto actual (Single Live Product Pattern)
        var currentLive = await _repository.GetLiveProductAsync();
        if(currentLive is not null)
        {
            currentLive.Archive();
            _repository.Update(currentLive);
        }

        // CREACIÓN SEGURA: Fabricamos el producto dentro del Handler.
        // El 'UserId' lo sacamos de _userContext, NO del request del usuario.
        // Ahora el compilador sabe que UserId no es nulo -----------------------
        var newProduct = new Product(
            request.Name,
            request.Price,
            _userContext.UserId!
        );

        // MAPEADO DE DATOS EXTRA (Aquí es donde ensanchamos la lógica)
        // Usamos los métodos de dominio que ya tienes en la Entidad
        newProduct.UpdateDetails(request.Name, request.Description, request.Price);
        newProduct.UpdateMedia(request.MainImage, request.VideoUrl, request.RenderUrl);
        newProduct.SetStock(request.Stock);

       if (request.Gallery is not null) newProduct.ReplaceGallery(request.Gallery);
       if (request.Tags is not null) newProduct.SetTags(request.Tags);

       // Si la entidad tiene el método UpdateStyle, lo usamos:
       newProduct.UpdateStyle(
            request.PrimaryColor,
            request.AccentColor,
            request.FontFamily,
            request.BackgroundImageUrl,
            request.CustomCss
       );

        // Publicar y Guardar
        newProduct.Publish();

        // Guardarlo
        await _repository.AddProductAsync(newProduct);

        // Confirmacion de los cambios
        await _repository.SaveChangesAsync();

        return newProduct.Id;
    }
}