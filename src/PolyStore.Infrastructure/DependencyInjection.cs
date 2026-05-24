using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PolyStore.Application.Abstractions.Authentication;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Infrastructure.Persistence.Context;
using PolyStore.Infrastructure.Persistence.Repositories;
using PolyStore.Infrastructure.Services;

namespace PolyStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // Aquí movemos el AddDbContext...
        // 1. Configuración del DbContext (Postgres)
        services.AddDbContext<StoreDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // 2. Registro de Repositorios
        // Aquí mueves todos los AddScoped de tus repositorios
        services.AddScoped<IProductRepository, ProductRepository>();

        // --- REGISTRO DEL REPOSITORIO DE PEDIDOS ---
        services.AddScoped<IOrderRepository, OrderRepository>(); // <--------------------------------

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Registro de authentication (Lógica de negocio de identidad)
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        // 2. Registro de herramientas de chequeo de entrada
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}