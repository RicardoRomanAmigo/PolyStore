using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Authentication;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Domain.Exceptions; 

namespace PolyStore.Application.Features.Products.DeleteProduct;

public class DeleteProductHandler
{
    private readonly IProductRepository _repository;
    private readonly IUserContext _userContext;


    public DeleteProductHandler(IProductRepository repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }
    public async Task ExecuteAsync(DeleteProductRequest request)
    {
        // PRIMER CANDADO: ¿Es un Admin?
        if (_userContext.Role != "Admin")
        {
            // Usamos Forbidden porque el usuario está autenticado pero no tiene el rol
            throw new ForbiddenException("Solo los administradores pueden eliminar productos. ");
        }

        // 2. Buscamos usando el ID que viene en el request
        var existingProduct = await _repository.GetProductByIdAsync(request.Id);

        // Si el producto no exite
        if(existingProduct is null)
        {
            // Usamos request.Id para el mensaje
            throw new NotFoundException($"No se encontró el producto con ID: {request.Id}");
        }

        // 3. Seguridad por propiedad
        if (existingProduct.CreatedBy != _userContext.UserId)
            throw new ForbiddenException("No tienes permiso para eliminar este producto.");

        // 4. ACCIÓN Y PERSISTENCIA
         _repository.Delete(existingProduct);

        // ¡IMPORTANTE! Sin esto, no se borra nada en la DB
         await _repository.SaveChangesAsync();
    }
}