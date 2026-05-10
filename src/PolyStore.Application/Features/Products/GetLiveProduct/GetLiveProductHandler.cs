using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Domain.Exceptions; // <--- Importamos nuestras excepciones

namespace PolyStore.Application.Features.Products.GetLiveProduct;

public class GetLiveProductHandler
{
    private readonly IProductRepository _repository;

    public GetLiveProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    // Cambiamos el retorno de Product? a Product (ya no esperamos nulos)
    public async Task<Product> ExecuteAsync()
    {
        var product = await _repository.GetLiveProductAsync();

        if(product == null)
        {
            // El Handler lanza el error
            throw new NotFoundException("No hy ningun producto activo en este momento.");
        }
        //Encapsulamos la busqueda del producto activo.
        // Si en el futuro el "live" depende de fechas o stock, se  cambia aqui
        
        return product;
    }
}