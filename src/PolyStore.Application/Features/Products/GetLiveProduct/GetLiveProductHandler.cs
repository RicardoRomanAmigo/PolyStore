using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;

namespace PolyStore.Application.Features.Products.GetLiveProduct;

public class GetLiveProductHandler
{
    private readonly IProductRepository _repository;

    public GetLiveProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product?> ExecuteAsync()
    {
        //Encapsulamos la busqueda del producto activo.
        // Si en el futuro el "live" depende de fechas o stock, se  cambia aqui
        return await _repository.GetLiveProductAsync();
    }
}