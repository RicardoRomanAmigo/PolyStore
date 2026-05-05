namespace PolyStore.Application.Features.Products.CreateLiveProduct;

// Definimos el objeto DTO con los datos que permitimos que lleguen desde la Web
public record CreateLiveProductRequest(string Name, decimal Price, string? Description);