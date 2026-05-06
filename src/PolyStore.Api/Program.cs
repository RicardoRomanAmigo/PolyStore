using Microsoft.EntityFrameworkCore;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Application.Features.Products.CreateLiveProduct;
using PolyStore.Infrastructure.Persistence.Context;
using PolyStore.Infrastructure.Persistence.Repositories;
using PolyStore.Application.Abstractions.Authentication;
using PolyStore.Infrastructure.Services;
using Scalar.AspNetCore;
using PolyStore.Application.Features.Products.UpdateProduct;
using PolyStore.Api.Extensions;
using PolyStore.Application.Features.Products.GetLiveProduct;
using PolyStore.Application.Features.Products.GetArchivedProduct;
using PolyStore.Application.Features.Products.GetProductById;

var builder = WebApplication.CreateBuilder(args);


// --- SERVICIOS ---

// Identidad (Configuracion del validador de Tokens)
builder.Services.AddIdentityServices(builder.Configuration);

// Registro el handler para el caso de uso de nuevo producto live
builder.Services.AddScoped<CreateLiveProductHandler>();

// Registro el handler para el caso de uso de actualizar producto
builder.Services.AddScoped<UpdateProductHandler>();

// Registro el handler para obtener el producto live
builder.Services.AddScoped<GetLiveProductHandler>();

// Registro el hendler para obtener los productos archived
builder.Services.AddScoped<GetArchivedProductHandler>();

// Registro el hendler para obtener el producto por id
builder.Services.AddScoped<GetProductByIdHandler>();

// Registro de los Controladores
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Registro del DbContext en PostgreSQL
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Registro el Repositorio (Asocio interfaz con Implementacion)
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Registro de authentication
builder.Services.AddScoped<IAuthService, AuthService>();

// Registro de ServicioToken
builder.Services.AddScoped<ITokenService, TokenService>();

// Registro de herraminetas de chequeo de entrada
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext,UserContext>();

// 2. Configurar JWT (Si usas Identity o JWT Bearer)
//builder.Services.AddAuthentication().AddJwtBearer(); // Configura aquí tus Keys/Issuer
//builder.Services.AddAuthorization();

// Registro del servicio CORS para para que el navegador permita la itneractuacion
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin() 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// --- PIPELINE (Configuracion de la app) ---
//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //Activo la interfaz visual de Scalar
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// CORS
app.UseCors("AllowFrontend");

// Autenticaticacion y Autorizacion (EL ORDEN ES CLAVE)
app.UseAuthentication(); // ¿Quien eres?
app.UseAuthorization(); // ¿Que puedes hacer?

// Mapeo de las rutas de los controladores
app.MapControllers();

app.Run();

