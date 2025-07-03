using Microsoft.AspNetCore.Mvc;
using ExamenPOO.API.Services;

namespace ExamenPOO.API.Controllers;

/// <summary>
/// Controlador de ejemplo que demuestra el manejo de errores
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ErrorTestController : ControllerBase
{
    private readonly IDetailedLoggerService _logger;

    public ErrorTestController(IDetailedLoggerService logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Endpoint que demuestra diferentes tipos de errores
    /// </summary>
    [HttpGet("test-error/{errorType}")]
    public IActionResult TestError(string errorType)
    {
        try
        {
            _logger.LogInfo("ErrorTest", $"Testing error type: {errorType}");

            return errorType.ToLower() switch
            {
                "argument" => throw new ArgumentException("Este es un ejemplo de ArgumentException"),
                "notfound" => throw new KeyNotFoundException("Este es un ejemplo de KeyNotFoundException"),
                "unauthorized" => throw new UnauthorizedAccessException("Este es un ejemplo de UnauthorizedAccessException"),
                "invalid" => throw new InvalidOperationException("Este es un ejemplo de InvalidOperationException"),
                "timeout" => throw new TimeoutException("Este es un ejemplo de TimeoutException"),
                "general" => throw new Exception("Este es un ejemplo de excepción general"),
                "success" => Ok(new { Message = "¡Todo funcionó correctamente!", Timestamp = DateTime.UtcNow }),
                _ => BadRequest(new { Message = "Tipo de error no reconocido. Usa: argument, notfound, unauthorized, invalid, timeout, general, success" })
            };
        }
        catch (Exception ex) when (!(ex is ArgumentException || ex is KeyNotFoundException || 
                                   ex is UnauthorizedAccessException || ex is InvalidOperationException || 
                                   ex is TimeoutException))
        {
            // Log del error antes de re-lanzarlo para que sea capturado por el middleware global
            _logger.LogError("ErrorTest", ex, $"Error testing error type: {errorType}");
            throw; // Re-lanza para que sea manejado por el middleware global
        }
    }

    /// <summary>
    /// Endpoint que demuestra logging sin errores
    /// </summary>
    [HttpGet("test-logging")]
    public IActionResult TestLogging()
    {
        _logger.LogInfo("ErrorTest", "Testing info logging");
        _logger.LogWarning("ErrorTest", "Testing warning logging", "Additional context info");
        
        return Ok(new { 
            Message = "Logging test completed", 
            Timestamp = DateTime.UtcNow,
            LogsGenerated = new[] { "Info", "Warning" }
        });
    }

    /// <summary>
    /// Endpoint que simula un proceso con manejo de errores interno
    /// </summary>
    [HttpPost("simulate-process")]
    public IActionResult SimulateProcess([FromBody] ProcessRequest request)
    {
        try
        {
            _logger.LogInfo("ErrorTest", "Starting process simulation", $"Process: {request.ProcessName}");

            // Simulación de validación
            if (string.IsNullOrEmpty(request.ProcessName))
            {
                _logger.LogWarning("ErrorTest", "Process name is empty");
                return BadRequest(new { Message = "Process name is required" });
            }

            // Simulación de proceso que puede fallar
            if (request.ProcessName.ToLower().Contains("fail"))
            {
                var ex = new InvalidOperationException("Process was designed to fail");
                _logger.LogError("ErrorTest", ex, $"Process failed as expected: {request.ProcessName}");
                throw ex;
            }

            // Simulación de proceso exitoso
            var result = new
            {
                ProcessName = request.ProcessName,
                Status = "Completed",
                StartTime = DateTime.UtcNow.AddMinutes(-1),
                EndTime = DateTime.UtcNow,
                Duration = "1 minute",
                Result = $"Process '{request.ProcessName}' completed successfully"
            };

            _logger.LogInfo("ErrorTest", "Process completed successfully", $"Process: {request.ProcessName}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("ErrorTest", ex, $"Unexpected error in process: {request?.ProcessName}");
            throw; // Re-lanza para el middleware global
        }
    }
}

/// <summary>
/// Modelo para la simulación de proceso
/// </summary>
public class ProcessRequest
{
    public string ProcessName { get; set; } = string.Empty;
    public Dictionary<string, object>? Parameters { get; set; }
}
