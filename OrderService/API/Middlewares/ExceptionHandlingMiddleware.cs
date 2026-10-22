using System.Net;
using System.Text.Json;

namespace OrderService.API.Middlewares;

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
            _logger.LogError(ex, "Ha ocurrido un error no controlado en el OrderService.");
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
            Error = "Order Service Error",
            Message = "Ocurrió un error inesperado al procesar tu orden. Por favor, contacta al soporte técnico de Deloitte.",
            Detail = exception.Message
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload, jsonOptions));
    }
}
