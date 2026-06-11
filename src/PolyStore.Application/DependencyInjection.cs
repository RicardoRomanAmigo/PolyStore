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
using PolyStore.Application.Features.Authentication.Login;
using PolyStore.Application.Features.Products.GetAllProducts;
using PolyStore.Application.Features.Orders.CreateOrder;
using PolyStore.Application.Features.Orders.UpdateOrderStatusToPaid;
using PolyStore.Application.Features.Orders.GetOrdersByUserId;
using PolyStore.Application.Features.Orders.GetOrderById;
using PolyStore.Application.Features.Orders.GetGuestOrder;
using PolyStore.Application.Features.Orders.CancelOrderDueToFailedPayment;

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
        services.AddScoped<GetAllProductsHandler>();
        services.AddScoped<DeleteProductHandler>(); 
        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<UpdateOrderStatusToPaidHandler>();
        services.AddScoped<GetOrdersByUserIdHandler>();
        services.AddScoped<GetOrderByIdHandler>();
        services.AddScoped<GetGuestOrderHandler>();
        services.AddScoped<CancelOrderDueToFailedPaymentHandler>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); //<-- Evita registrar los validadores uno a uno

        return services;
    }
} 