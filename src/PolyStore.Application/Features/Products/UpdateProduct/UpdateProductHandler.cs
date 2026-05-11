using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Domain.Entities;
using PolyStore.Application.Abstractions.Authentication; 
using PolyStore.Domain.Exceptions; 
using FluentValidation; // <--- FluentValidation
using DomainExceptions = PolyStore.Domain.Exceptions; // Alias de seguridad para usarlo

namespace PolyStore.Application.Features.Products.UpdateProduct;

public class UpdateProductHandler
{
    private readonly IProductRepository _repository;
    private readonly IUserContext _userContext; 
    private readonly IValidator<UpdateProductRequest> _validator; // 1. Campo para el validador

    public UpdateProductHandler (IProductRepository repository, IUserContext userContext, IValidator<UpdateProductRequest> validator ) // 2. Se inyecta aquí automáticamente
    {
        _repository = repository;
        _userContext = userContext;
        _validator = validator;
    }

    // 3. Cambiamos el parámetro a 'Request'
    public async Task ExecuteAsync(Guid id, UpdateProductRequest request)
    {
        // --- 3. VALIDACION DE DATOS --------------------------------------------
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
            throw new DomainExceptions.ValidationException(errors);
        }
        // -----------------------------------------------------------------------

        // PRIMER CANDADO: ¿Es un Admin?
        if (_userContext.Role != "Admin")
        {
            // Usamos Forbidden porque el usuario está autenticado pero no tiene el rol
            throw new ForbiddenException("Solo los administradores pueden modificar productos. ");
        }

        var existingProduct = await _repository.GetProductByIdAsync(id);

        // Si el producto no exite
        if(existingProduct is null)
        {
            // Ya no lanzamos una Exception genérica, lanzamos nuestro caso de dominio
            throw new NotFoundException($"No se encontró el producto con ID: {id}");
        }
            
        // Si el creador es otro (Seguridad por propiedad)
        if (existingProduct.CreatedBy != _userContext.UserId)
            throw new ForbiddenException("No tienes permiso para modificar este producto.");    

        // --- Lógica de actualización (Sin cambios) ---
        existingProduct.UpdateDetails(request.Name,request.Description,request.Price);
        // Actualizar Media
        existingProduct.UpdateMedia(request.MainImage, request.VideoUrl, request.RenderUrl);
        // Reemplazar Galeria
        existingProduct.ReplaceGallery( request.Gallery);
        // Setear Tags
        existingProduct.SetTags(request.Tags);
        // Setear Stock
        existingProduct.SetStock(request.Stock);
        // Los estilos
        existingProduct.UpdateStyle(request.PrimaryColor,request.AccentColor,request.FontFamily,request.BackgroundImageUrl,request.CustomCss);
        
        // Persistencia 
        await _repository.SaveChangesAsync();
    }
}