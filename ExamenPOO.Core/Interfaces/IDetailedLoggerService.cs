namespace ExamenPOO.Core.Interfaces;

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
