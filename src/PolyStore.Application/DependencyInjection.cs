using Microsoft.Extensions.DependencyInjection;
using PolyStore.Application.Features.Products.CreateLiveProduct;
using PolyStore.Application.Features.Products.UpdateProduct;
using PolyStore.Application.Features.Products.GetLiveProduct;
using PolyStore.Application.Features.Products.GetArchivedProduct;
using PolyStore.Application.Features.Products.GetProductById;
using FluentValidation;
using System.Reflection;
using PolyStore.Application.Features.Products.DeleteProduct;
using PolyStore.Application.Features.Authentication.Register;

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
        services.AddScoped<DeleteProductHandler>(); 
        services.AddScoped<RegisterHandler>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); 

        return services;
    }
} 