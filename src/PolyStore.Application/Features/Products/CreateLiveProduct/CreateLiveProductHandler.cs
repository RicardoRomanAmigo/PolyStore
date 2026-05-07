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

        // 5. Lógica de negocio: Archivamos el producto actual si existe
        var currentLive = await _repository.GetLiveProductAsync();

        // 6.Si existe, archivarlo
        if(currentLive is not null)
        {
            currentLive.Archive();
            _repository.Update(currentLive);
        }

        // 6. CREACIÓN SEGURA: Fabricamos el producto dentro del Handler.
        // El 'UserId' lo sacamos de _userContext, NO del request del usuario.
        // Ahora el compilador sabe que UserId no es nulo -----------------------
        var newProduct = new Product(
            request.Name,
            request.Price,
            _userContext.UserId!
        );

        // Si viene descripción, la actualizamos mediante su método de dominio
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            newProduct.UpdateDetails(request.Name, request.Description, request.Price);
        }

        // 7. Publicar y Guardar
        newProduct.Publish();

        // Guardarlo
        await _repository.AddProductAsync(newProduct);

        // Confirmacion de los cambios
        await _repository.SaveChangesAsync();

        return newProduct.Id;
    }
}