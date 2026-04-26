namespace PolyStore.Domain.Enums;

public enum ProductStatus
{
    // Producto en creación, no visible
    Draft = 0,

    // Producto publicado y visible en web 1
    Live = 1,

    // // Producto retirado del foco principal
    // Se muestra en la web 2
    Archived = 2,

    // Producto sin stock    
    SoldOut = 3
}