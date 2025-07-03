using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ExamenPOO.API.Middleware;

/// <summary>
/// Middleware global para el manejo de errores no controlados
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en la solicitud {RequestPath}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ArgumentException ex:
                errorResponse.Message = ex.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Type = "ArgumentException";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case KeyNotFoundException ex:
                errorResponse.Message = ex.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Type = "KeyNotFoundException";
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case UnauthorizedAccessException ex:
                errorResponse.Message = ex.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Type = "UnauthorizedAccessException";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case InvalidOperationException ex:
                errorResponse.Message = ex.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Type = "InvalidOperationException";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case TimeoutException ex:
                errorResponse.Message = "La operación ha excedido el tiempo límite";
                errorResponse.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Type = "TimeoutException";
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                break;

            default:
                errorResponse.Message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "Ha ocurrido un error interno del servidor";
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Type = "InternalServerError";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        // Agregar información adicional en modo desarrollo
        if (_environment.IsDevelopment())
        {
            errorResponse.Detail = exception.Message;
            errorResponse.StackTrace = exception.StackTrace;
            errorResponse.InnerException = exception.InnerException?.Message;
        }

        // Información de la solicitud
        errorResponse.Instance = context.Request.Path;
        errorResponse.Method = context.Request.Method;
        errorResponse.Timestamp = DateTime.UtcNow;

        // Información adicional para debugging
        errorResponse.RequestId = context.TraceIdentifier;
        errorResponse.UserAgent = context.Request.Headers["User-Agent"].ToString();
        errorResponse.RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Modelo de respuesta para errores
/// </summary>
public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string Instance { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string RemoteIpAddress { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
}
