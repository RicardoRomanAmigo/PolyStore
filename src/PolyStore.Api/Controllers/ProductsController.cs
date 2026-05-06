using Microsoft.AspNetCore.Mvc;
using PolyStore.Domain.Entities;
// using PolyStore.Application.Abstractions.Persistence; -------------------------
using PolyStore.Application.Features.Products.CreateLiveProduct;
using PolyStore.Application.Features.Products.UpdateProduct;
using PolyStore.Application.Features.Products.GetLiveProduct; // Añadimos los nuevos handlers
using PolyStore.Application.Features.Products.GetArchivedProduct; // Nuevos handlers
using PolyStore.Application.Features.Products.GetProductById; // Nuevos handlers

namespace PolyStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // La URL sera api/products
public class ProductsController : ControllerBase
{
    // private readonly IProductRepository _repository; Eliminamos el repositorio ------------------
    private readonly CreateLiveProductHandler _createHandler;
    private readonly UpdateProductHandler _updateHandler;
    private readonly GetLiveProductHandler _getLiveHandler; // Declaramos las variables ----------------
    private readonly GetArchivedProductHandler _getArchivedHandler; // Declaramos las variables  -------------------
    private readonly GetProductByIdHandler _getByIdHandler; // Declaramos las variables  -------------------

    // Inyectamos la interfaz del repositorio
    // Solo los handlers necesarios ------------
    public ProductsController(
        CreateLiveProductHandler createHandler,
        UpdateProductHandler updateHandler,
        GetLiveProductHandler getLiveHandler,
        GetArchivedProductHandler getArchivedHandler,
        GetProductByIdHandler getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _getLiveHandler = getLiveHandler;
        _getArchivedHandler = getArchivedHandler;
        _getByIdHandler = getByIdHandler;
    }

    // GET: api/products/live
    //Este lo usara la web principal para el producto unico y principal
    [HttpGet("live")]
    public async Task<ActionResult<Product>> GetLiveProduct()
    {
        // var product = await _repository.GetLiveProductAsync(); (Antes con el repositorio) ---------------------
        var product = await _getLiveHandler.ExecuteAsync(); // Ahora delegamos al handler ------------------------

        if(product == null) return NotFound("No hay ningun producto activo.");

        return Ok(product);
    }

    // GET: api/products/archived
    // Este lo usara la segunda web para el catalogo historico
    [HttpGet("archived")]
    public async Task<ActionResult<IEnumerable<Product>>> GetArchivedProducts()
    {
        // var products = await _repository.GetArchivedProductsAsync(); (Antes con el repositorio) ---------------------
        var products = await _getArchivedHandler.ExecuteAsync(); // Ahora delegamos al handler ------------------------
        return Ok(products);
    }

    // GET: api/products
    // Este metodo se usa para obtener un producto por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(Guid id)
    {
        //var product = await _repository.GetProductByIdAsync(id); (Antes con el repositorio) ---------------------
        var product = await _getByIdHandler.ExecuteAsync(id); // Ahora delegamos al handler ------------------------

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
    public async Task<ActionResult> CreateProduct(CreateLiveProductRequest request) // modificamos el parametro -----------
    {
        //await _createHandler.ExecuteAsync(product); (amtes)

        // El ID se generará solo DENTRO del Handler cuando haga 'new Product(...)'
        var newId = await _createHandler.ExecuteAsync(request); 

        return Ok(new { Id = newId, Message = "Producto creado y puesto en Live" }); 
    }

    // PUT: api/products
    // Para modificar un producto
    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(Guid id, UpdateProductRequest request) //---------------
    {
        // 1. Seguimos validando que el ID de la URL sea el mismo que el del cuerpo (opcional pero recomendado)
        // Nota: El Request debería tener también el Id o lo tomamos directamente de la URL.

        try
        {
            // 2. Ahora pasamos el 'request' (DTO), no la entidad 'product'
            await _updateHandler.ExecuteAsync(id, request);

            return NoContent(); // 204 : Todo ha ido bien, peo no hay nada que devolver
        }
        catch (UnauthorizedAccessException)
        {
            // 3. Importante: Si el Handler lanza esta excepción por el IUserContext
            return Forbid(); // 403: No tienes permiso para editar este producto
        }
        catch (Exception ex) when (ex.Message == "Producto no encontrado")
        {
                return NotFound($"No se encontro el producto con ID: {id}");   
        }
    }
}