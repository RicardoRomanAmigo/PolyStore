namespace PolyStore.Application.Features.Products.UpdateProduct;

// Definimos el objeto DTO con los datos que permitimos que lleguen desde la Web
public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    // Multimedia
    string? MainImage,
    string? VideoUrl,
    string? RenderUrl,
    List<string> Gallery,
    // Inventario
    List<string> Tags,
    int Stock,
    // Estilo
    string? PrimaryColor,
    string? AccentColor,
    string? FontFamily,
    string? BackgroundImageUrl,
    string? CustomCss
);