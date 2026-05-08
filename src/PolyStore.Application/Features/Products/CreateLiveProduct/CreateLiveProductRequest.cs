namespace PolyStore.Application.Features.Products.CreateLiveProduct;

// Definimos el objeto DTO con los datos que permitimos que lleguen desde la Web
public record CreateLiveProductRequest(
    string Name,
    decimal Price,
    string? Description,
    // Multimedia
    string? MainImage,
    List<string>? Gallery,
    string? VideoUrl,
    string? RenderUrl,
    // Inventario
    int Stock,
    List<string>? Tags,
    // Estilo (Opcionales)
    string? PrimaryColor,
    string? AccentColor,
    string? FontFamily,
    string? BackgroundImageUrl,
    string? CustomCss
);