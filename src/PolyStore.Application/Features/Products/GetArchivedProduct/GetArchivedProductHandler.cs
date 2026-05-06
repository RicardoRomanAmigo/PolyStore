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
        //Centralizamos la obtencion del catalogo
        return await _repository.GetArchivedProductsAsync();
    }
}