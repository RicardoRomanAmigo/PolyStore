using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;


namespace PolyStore.Application.Features.Products.GetArchivedProduct;

public class GetArchivedProductHandler
{
    private readonly IProductRepository _repository;

    public GetArchivedProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> ExecuteAsync()
    {
        var products = await _repository.GetArchivedProductsAsync();
        
        // Si por alguna razón el repositorio devolviera null, 
        // devolvemos una lista vacía para no romper el Frontend.
        return products ?? Enumerable.Empty<Product>();
    }
}