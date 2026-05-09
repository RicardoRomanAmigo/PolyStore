using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PolyStore.Domain.Exceptions;

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

        // Por defecto es 500
        var statusCode = HttpStatusCode.InternalServerError;
        var title = "Error interno en la API";

        // Aquí "cazamos" los tipos específicos
        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                title = "Recurso no encontrado";
                break;

            case ForbiddenException:
                statusCode = HttpStatusCode.Forbidden;
                title = "Acceso denegado";
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                title = "No autorizado";
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var response = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = title,
            Detail = exception.Message
        };

        //1. Definimos las reglas de estilo (CamelCase)
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true // Opcional: hace que el JSON se vea "bonito" con espacios
        };

        // 2. Serializamos el objeto 'response' aplicando las 'options'
        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}