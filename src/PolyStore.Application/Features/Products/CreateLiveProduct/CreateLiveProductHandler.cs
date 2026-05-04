using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;

namespace PolyStore.Application.Features.Products.CreateLiveProduct;

public class CreateLiveProductHandler
{
    private readonly IProductRepository _repository;

    public CreateLiveProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Product newProduct)
    {
        // 1.Busca prodcuto Live Actual
        var currentLive = await _repository.GetLiveProductAsync();

        // 2.Si existe, archivarlo
        if(currentLive is not null)
        {
            currentLive.Archive();
            _repository.Update(currentLive);
        }

        // 3.Publicar el nuevo producto
        newProduct.Publish();

        // 4. Guardarlo
        await _repository.AddProductAsync(newProduct);

        // 5. Confirmacion de los cambios
        await _repository.SaveChangesAsync();
    }
}