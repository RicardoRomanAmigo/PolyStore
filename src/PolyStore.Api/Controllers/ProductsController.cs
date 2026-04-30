using Microsoft.AspNetCore.Mvc;
using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Application.Features.Products.CreateLiveProduct;
using PolyStore.Application.Features.Products.UpdateProduct;

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // La URL sera api/products
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly CreateLiveProductHandler _handler;
    private readonly UpdateProductHandler _updateHandler;

    // Inyectamos la interfaz del repositorio
    public ProductsController(IProductRepository repository, CreateLiveProductHandler handler, UpdateProductHandler updateHandler)
    {
        _repository = repository;
        _handler = handler;
        _updateHandler = updateHandler;
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

    // GET: api/products
    // Este metodo se usa para obtener un producto por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(Guid id)
    {
        // 1. Llamamos al repoisorio para buscar por id
        var product = await _repository.GetProductByIdAsync(id);

        // 2. Si no existe devolvemos 404
        if(product == null)
        {
            return NotFound(new {Message = "El producto no existe en el archivo. "});
        }

        // 3. Devolvemos al entidad Product
        return Ok(product);
    }

    // POST: api/products
    // Para cuando haga falta añadir nuevos productos
    [HttpPost]
    public async Task<ActionResult> CreateProduct(Product product)
    {
        await _handler.ExecuteAsync(product);

        return Ok(product); // temporal
    }

    // PUT: api/products
    // Para modificar un producto
    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(Guid id, Product product)
    {
        // 1. Validacion de seguridad: que el ID de la URL coincida con el del objeto
        if(id != product.Id)
        {
            return BadRequest("El ID del producto no coincide con el de la URL");
        }

        try
        {
            // 2. Llamamos al handler para filtrar
            await _updateHandler.ExecuteAsync(product, id);
            return NoContent(); // 204 : Todo ha ido bien, peo no hay nada que devolver
        }
        catch (Exception ex) when (ex.Message == "Producto no encontrado")
        {
                return NotFound($"No se encontro el producto con ID: {id}");   
        }
    }
}