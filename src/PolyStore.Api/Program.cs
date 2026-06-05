using Scalar.AspNetCore;
using PolyStore.Api.Extensions;
using PolyStore.Infrastructure;
using PolyStore.Application;
using PolyStore.Api.MiddleWares;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);


// --- SERVICIOS EXTRAIDOS ---
builder.Services.AddPersistence(builder.Configuration); // Base de Datos - Infrastructure.DependencyInjection
builder.Services.AddApplicationHandlers(); // Casos de uso - Application.DependencyInjection
builder.Services.AddInfrastructureServices(builder.Configuration); // Auth & Context - Infrastructure.DependencyInjection
builder.Services.AddIdentityServices(builder.Configuration); // El validador de JWT que ya tenía
// Configuraicion del Hangfire gestor de tareas <---------
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
// Registrar el servidor de procesamiento de Hangfire <--------
builder.Services.AddHangfireServer();

// --- SERVICIOS BASE ---
builder.Services.AddControllers(); // Registro de los Controladores
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin() 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
}); // Registro del servicio CORS para para que el navegador permita la itneractuacion

var app = builder.Build();

// --- PIPELINE (Configuracion de la app) ---
// 1. Debe ser lo primero (o de lo primero) que registres.
// ¿Por qué? Porque solo puede atrapar errores de las piezas que vienen DESPUÉS de él.
app.UseMiddleware<ExceptionMiddleware>();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //Activo la interfaz visual de Scalar
    app.MapScalarApiReference();
}

// Habilitar el Dashboard de Hangfire <------
app.UseHangfireDashboard(); // Nota: Por seguridad, en producción se debe proteger esta ruta *****************************************

app.UseHttpsRedirection();
app.UseCors("AllowFrontend"); // CORS

// Autenticaticacion y Autorizacion (EL ORDEN ES CLAVE)
app.UseAuthentication(); // ¿Quien eres?
app.UseAuthorization(); // ¿Que puedes hacer?

// Mapeo de las rutas de los controladores
app.MapControllers();

//llamada a la extension de HangfireExtensions
app.ConfigureRecurringJobs();

app.Run();

