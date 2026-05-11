namespace PolyStore.Application.Features.Products.DeleteProduct;

// Definimos el DTO con los datos que permitimos que lleguen desde la web
public record DeleteProductRequest(Guid Id);