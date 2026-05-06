using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;

namespace PolyStore.Application.Features.Products.GetProductById;

public class GetProductByIdHandler
{
    private readonly IProductRepository _repository;

    public GetProductByIdHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product?> ExecuteAsync(Guid id)
    {
        // Centralizamos la lógica de búsqueda por ID.
        return await _repository.GetProductByIdAsync(id);
    }
}