using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace PolyStore.Api.MiddleWares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    // 'next' es el siguiente paso en la tuberia de la API
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            //Intentamos que la peticion siga su camino normal
            await _next(context);
        }
        catch (Exception ex)
        {
            //Si algo explota en cualquier parte (Controller , Handler, DB...) cae aqui
            _logger.LogError(ex, "Algo salio mal: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Creamos una respuesta estandar de error
        var response = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = "Error interno en la API",
            Detail = exception.Message // Luego filtraremos esto para no dar pistas a hackers
        };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}