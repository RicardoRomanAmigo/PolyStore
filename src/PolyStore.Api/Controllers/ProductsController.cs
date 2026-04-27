using Microsoft.AspNetCore.Mvc;
using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Application.Features.Products.CreateLiveProduct;

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // La URL sera api/products
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly CreateLiveProductHandler _handler;

    // Inyectamos la interfaz del repositorio
    public ProductsController(IProductRepository repository, CreateLiveProductHandler handler)
    {
        _repository = repository;
        _handler = handler;
    }

    // GET: api/products/live
    //Este lo usara la web principal para el producto unico y principal
    [HttpGet("live")]
    public async Task<ActionResult<Product>> GetLiveProduct()
    {
        var product = await _repository.GetLiveProductAsync();

        if(product == null) return NotFound("No hay ningun producto activo.");

        return Ok(product);
    }

    // GET: api/products/archived
    // Este lo usara la segunda web para el catalogo historico
    [HttpGet("archived")]
    public async Task<ActionResult<IEnumerable<Product>>> GetArchivedProducts()
    {
        var products = await _repository.GetArchivedProductsAsync();
        return Ok(products);
    }

    // POST: api/products
    // Para cuando haga falta añadir nuevos productos
    [HttpPost]
    public async Task<ActionResult> CreateProduct(Product product)
    {
        await _handler.ExecuteAsync(product);

        return Ok(product); // temporal
    }
}