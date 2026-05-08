using Microsoft.Extensions.DependencyInjection;
using PolyStore.Application.Features.Products.CreateLiveProduct;
using PolyStore.Application.Features.Products.UpdateProduct;
using PolyStore.Application.Features.Products.GetLiveProduct;
using PolyStore.Application.Features.Products.GetArchivedProduct;
using PolyStore.Application.Features.Products.GetProductById;

namespace PolyStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationHandlers(this IServiceCollection services)
    {
        services.AddScoped<CreateLiveProductHandler>();
        services.AddScoped<UpdateProductHandler>();
        services.AddScoped<GetLiveProductHandler>();
        services.AddScoped<GetArchivedProductHandler>();
        services.AddScoped<GetProductByIdHandler>();

        return services;
    }
} 