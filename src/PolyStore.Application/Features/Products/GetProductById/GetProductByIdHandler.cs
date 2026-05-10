using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Domain.Exceptions; // <--- Importamos nuestras excepciones

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
        var product = await _repository.GetProductByIdAsync(id);

        if(product == null)
        {
            //El handler lanza el error
            throw new NotFoundException($"El producto con ID {id} no existe en el catálogo.");
        }
        
        return product;
    }
}