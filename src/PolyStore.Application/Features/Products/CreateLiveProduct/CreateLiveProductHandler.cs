using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Application.Abstractions.Authentication; // 1. Importamos la abstaccion

namespace PolyStore.Application.Features.Products.CreateLiveProduct;

public class CreateLiveProductHandler
{
    private readonly IProductRepository _repository;
    private readonly IUserContext _userContext; // 2. Campo para la identidad

    public CreateLiveProductHandler(IProductRepository repository, IUserContext userContext )
    {
        _repository = repository;
        _userContext = userContext; // Inyectamos el servicio
    }

    // 3. Cambiamos el parámetro a 'Request'
    public async Task<Guid> ExecuteAsync(CreateLiveProductRequest request)
    {
        // 1. VALIDACIÓN DE IDENTIDAD Y ROL (Seguridad de Negocio)
        // No solo nos importa que esté autenticado, nos importa QUIÉN es.
        if (!_userContext.IsAuthenticated || _userContext.Role !="Admin")
        {
            throw new UnauthorizedAccessException("Acceso denegado: Se requieren privilegios de administrador para realizar esta acción.");
        }

        // 2. Lógica de negocio: Archivamos el producto actual (Single Live Product Pattern)
        var currentLive = await _repository.GetLiveProductAsync();
        if(currentLive is not null)
        {
            currentLive.Archive();
            _repository.Update(currentLive);
        }

        // 3. CREACIÓN SEGURA: Fabricamos el producto dentro del Handler.
        // El 'UserId' lo sacamos de _userContext, NO del request del usuario.
        // Ahora el compilador sabe que UserId no es nulo -----------------------
        var newProduct = new Product(
            request.Name,
            request.Price,
            _userContext.UserId!
        );

        // 4. MAPEADO DE DATOS EXTRA (Aquí es donde ensanchamos la lógica)
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

        // 5. Publicar y Guardar
        newProduct.Publish();

        // Guardarlo
        await _repository.AddProductAsync(newProduct);

        // Confirmacion de los cambios
        await _repository.SaveChangesAsync();

        return newProduct.Id;
    }
}