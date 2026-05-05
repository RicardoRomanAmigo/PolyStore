using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Authentication; // 1. Importamos la abstraccion

namespace PolyStore.Application.Features.Products.UpdateProduct;

public class UpdateProductHandler
{
    private readonly IProductRepository _repository;
    private readonly IUserContext _userContext; // 2. Campo para la identidad

    public UpdateProductHandler (IProductRepository repository, IUserContext userContext )
    {
        _repository = repository;
        _userContext = userContext; // Inyectamos el servicio
    }

    // 3. Cambiamos el parámetro a 'Request'
    public async Task ExecuteAsync(Guid id, UpdateProductRequest request)
    {
        // 4. Lógica de negocio: Archivamos el producto actual si existe
        var existingProduct = await _repository.GetProductByIdAsync(id);

        // 5. Si el producto no exite
        if(existingProduct is null)
            throw new Exception("Producto no encontrado");

        // 6. Si el creador es otro
        if (existingProduct.CreatedBy != _userContext.UserId)
            throw new UnauthorizedAccessException("No tienes permiso para modificar este producto.");    

        existingProduct.UpdateDetails(
            request.Name,
            request.Description,
            request.Price
        );

        // Actualizar Media
        existingProduct.UpdateMedia(
            request.MainImage, request.VideoUrl, request.RenderUrl
        );

        // Reemplazar Galeria
        existingProduct.ReplaceGallery(
            request.Gallery
        );

        // Setear Tags
        existingProduct.SetTags(
            request.Tags
        );

        // Setear Stock
        existingProduct.SetStock(
            request.Stock
        );

        // Persistencia 
        await _repository.SaveChangesAsync();
    }
}