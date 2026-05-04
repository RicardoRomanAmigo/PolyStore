using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Domain.Entities;


namespace PolyStore.Application.Features.Products.UpdateProduct;

public class UpdateProductHandler
{
    private readonly IProductRepository _repository;

    public UpdateProductHandler (IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Product product, Guid id)
    {
        // 1. Busco producto actual
        var existingProduct = await _repository.GetProductByIdAsync(id);

        // 2. Si el producto no exite
        if(existingProduct is null)
            throw new Exception("Producto no encontrado");

        existingProduct.UpdateDetails(
            product.Name,
            product.Description,
            product.Price
        );

        // 3. Actualizar Media
        existingProduct.UpdateMedia(
            product.MainImage, product.VideoUrl, product.RenderUrl
        );

        // 4. Reemplazar Galeria
        existingProduct.ReplaceGallery(
            product.Gallery
        );

        // 5. Setear Tags
        existingProduct.SetTags(
            product.Tags
        );

        // 6. Setear Stock
        existingProduct.SetStock(
            product.Stock
        );

        // 7. Persistencia 
        await _repository.SaveChangesAsync();
    }
}