using PolyStore.Domain.Entities;

namespace PolyStore.Application.Abstractions.Persistence;

public interface IProductRepository
{
    // Obtener el producto principal de la Web 1
    Task<Product?> GetLiveProductAsync();

    // Obtener el catalogo de productos de la Web 2
    Task<IEnumerable<Product>> GetArchivedProductsAsync();

    // Obtener un producto por su ID (para la pagina de detalles de ambas webs)
    Task<Product?> GetProductByIdAsync(Guid id);

    // Guardar un producto nuevo
    Task AddProductAsync(Product product);
}