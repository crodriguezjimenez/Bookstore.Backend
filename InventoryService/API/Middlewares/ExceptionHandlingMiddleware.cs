using System.Net;
using System.Text.Json;

namespace InventoryService.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ha ocurrido un error no controlado en el servidor.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; 

        var responsePayload = new
        {
            StatusCode = context.Response.StatusCode,
            Error = "Internal Server Error",
            Message = "Ocurrió un error inesperado al procesar tu solicitud. Por favor, contacta al administrador.",
            Detail = exception.Message // En producción, se recomienda no enviar el detalle técnico por seguridad
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload, jsonOptions));
    }
}
