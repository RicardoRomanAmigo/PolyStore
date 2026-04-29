using PolyStore.Domain.Enums;

namespace PolyStore.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }

    public string Name {get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }

    // --- Multimedia ---
    public string? MainImage { get; private set; } // Foto principal del producto
    private readonly List<string> _gallery = new();// Lista del URLs de fotos
    public IReadOnlyCollection<string> Gallery => _gallery;
    public string? VideoUrl { get; private set; } // Video del producto (opcional)
    public string? RenderUrl { get; private set; } // El render 3D original (opcional)

    // --- Inventario y Logica ---
    public List<string> Tags {get; private set;} = new(); //Etiquetas para busqueda y filtrado
    public int Stock { get; private set; } 
    public ProductStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; } 
    public DateTime? PublishedAt { get; private set; }

    // --- Propiedades de Estilo Dinamico ---
    public string? PrimaryColor { get; private set; } // Ejemplo: "#0f172a"
    public string? AccentColor { get; private set; } // Ejemplo: "#21a174"
    public string? FontFamily { get; private set; }  // Ejemplo: "'Inter', sans-serif"
    public string? BackgroundImageUrl { get; private set; } // Para fondo decorativo unico
    public string? CustomCss { get; private set; } // Por si se mete algun estilo extra

    // --- Constructor ---
    public Product(string name, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Status = ProductStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    // --- Metodos de dominio ---
    // - Update de los datos basicos -
    public void UpdateDetails(string name, string? description, decimal price)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if(price < 0)
            throw new ArgumentException("Price cannot be negative");
        
        Name = name;
        Description = description;
        Price = price;
    }

    // - Update de los datos de Media -
    public void UpdateMedia(string? mainImage, string? videoUrl, string? renderUrl)
    {
        MainImage = mainImage;
        VideoUrl = videoUrl;
        RenderUrl = renderUrl;
    }

    // - Update del stock -
    public void SetStock(int stock)
    {
        if(stock < 0)
            throw new ArgumentException("Stock cannot be negative");
            
       Stock = stock;

       if(stock == 0)
            Status = ProductStatus.SoldOut; 
    }

    // - Metodo de dominio para Status Live -
    public void Publish()
    {
        if(Status == ProductStatus.Live)
            return;

        Status = ProductStatus.Live;
        PublishedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        if(Status != ProductStatus.Live)
            throw new InvalidOperationException("Only live products can be archived");

        Status = ProductStatus.Archived;
    }

    // - Update de imagen -
    public void AddImage(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        throw new ArgumentException("Image URL cannot be empty");
        
        _gallery.Add(url);
    }

    // - Update Reemplazar galeria -
    public void ReplaceGallery(IEnumerable<string> urls)
    {
        _gallery.Clear();

        foreach (var url in urls)
        {
            AddImage(url);
        }
    }

    // - Update de los Tags -
    public void SetTags(IEnumerable<string> tags)
    {
        Tags = tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct()
            .ToList();
    }

    // - Update del estilo de pagina -
    public void UpdateStyle(
        string? primaryColor,
        string? accentColor,
        string? fontFamily,
        string? backgroundImageUrl,
        string? customCss)
    {
        PrimaryColor = primaryColor;
        AccentColor = accentColor;
        FontFamily = fontFamily;
        BackgroundImageUrl = backgroundImageUrl;
        CustomCss = customCss;
    }
}