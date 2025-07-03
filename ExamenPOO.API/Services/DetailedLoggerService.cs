using System.Diagnostics;

namespace ExamenPOO.API.Services;

/// <summary>
/// Servicio de logging detallado para errores críticos
/// </summary>
public interface IDetailedLoggerService
{
    void LogError(string area, Exception exception, string? additionalInfo = null);
    void LogWarning(string area, string message, string? additionalInfo = null);
    void LogInfo(string area, string message, string? additionalInfo = null);
    void LogCritical(string area, Exception exception, string? additionalInfo = null);
}

public class DetailedLoggerService : IDetailedLoggerService
{
    private readonly ILogger<DetailedLoggerService> _logger;
    private readonly IWebHostEnvironment _environment;

    public DetailedLoggerService(ILogger<DetailedLoggerService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void LogError(string area, Exception exception, string? additionalInfo = null)
    {
        var logMessage = BuildLogMessage(area, exception.Message, additionalInfo);
        
        if (_environment.IsDevelopment())
        {
            Console.WriteLine($"[ERROR] {logMessage}");
            Console.WriteLine($"Stack Trace: {exception.StackTrace}");
            if (exception.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {exception.InnerException.Message}");
            }
        }

        _logger.LogError(exception, logMessage);
    }

    public void LogWarning(string area, string message, string? additionalInfo = null)
    {
        var logMessage = BuildLogMessage(area, message, additionalInfo);
        
        if (_environment.IsDevelopment())
        {
            Console.WriteLine($"[WARNING] {logMessage}");
        }

        _logger.LogWarning(logMessage);
    }

    public void LogInfo(string area, string message, string? additionalInfo = null)
    {
        var logMessage = BuildLogMessage(area, message, additionalInfo);
        
        if (_environment.IsDevelopment())
        {
            Console.WriteLine($"[INFO] {logMessage}");
        }

        _logger.LogInformation(logMessage);
    }

    public void LogCritical(string area, Exception exception, string? additionalInfo = null)
    {
        var logMessage = BuildLogMessage(area, exception.Message, additionalInfo);
        
        // Los errores críticos siempre se muestran en consola
        Console.WriteLine($"[CRITICAL] {logMessage}");
        Console.WriteLine($"Stack Trace: {exception.StackTrace}");
        if (exception.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {exception.InnerException.Message}");
        }

        _logger.LogCritical(exception, logMessage);
    }

    private string BuildLogMessage(string area, string message, string? additionalInfo)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
        var processId = Process.GetCurrentProcess().Id;
        var threadId = Thread.CurrentThread.ManagedThreadId;
        
        var logMessage = $"[{timestamp}] [PID:{processId}] [TID:{threadId}] [{area}] {message}";
        
        if (!string.IsNullOrEmpty(additionalInfo))
        {
            logMessage += $" | Additional Info: {additionalInfo}";
        }

        return logMessage;
    }
}
