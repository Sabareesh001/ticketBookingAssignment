namespace BusBookingAPI.Utilities;

public static class LoggingHelper
{
    /// <summary>
    /// Logs method entry with parameters
    /// </summary>
    public static void LogMethodEntry(ILogger logger, string methodName, Dictionary<string, object?>? parameters = null)
    {
        if (parameters == null || parameters.Count == 0)
        {
            logger.LogDebug("→ Entering method: {MethodName}", methodName);
        }
        else
        {
            var paramString = string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}"));
            logger.LogDebug("→ Entering method: {MethodName} with parameters: {Parameters}", methodName, paramString);
        }
    }

    /// <summary>
    /// Logs method exit with result
    /// </summary>
    public static void LogMethodExit(ILogger logger, string methodName, object? result = null)
    {
        if (result == null)
        {
            logger.LogDebug("← Exiting method: {MethodName}", methodName);
        }
        else
        {
            logger.LogDebug("← Exiting method: {MethodName} with result: {Result}", methodName, result);
        }
    }

    /// <summary>
    /// Logs database operation
    /// </summary>
    public static void LogDatabaseOperation(ILogger logger, string operation, string entity, string? details = null)
    {
        if (string.IsNullOrEmpty(details))
        {
            logger.LogDebug("📊 Database operation: {Operation} on {Entity}", operation, entity);
        }
        else
        {
            logger.LogDebug("📊 Database operation: {Operation} on {Entity} - {Details}", operation, entity, details);
        }
    }

    /// <summary>
    /// Logs validation error
    /// </summary>
    public static void LogValidationError(ILogger logger, string fieldName, string errorMessage)
    {
        logger.LogWarning("⚠️ Validation error on field '{FieldName}': {ErrorMessage}", fieldName, errorMessage);
    }

    /// <summary>
    /// Logs business logic error
    /// </summary>
    public static void LogBusinessLogicError(ILogger logger, string operation, string reason)
    {
        logger.LogWarning("❌ Business logic error during {Operation}: {Reason}", operation, reason);
    }

    /// <summary>
    /// Logs successful operation
    /// </summary>
    public static void LogSuccess(ILogger logger, string operation, string? details = null)
    {
        if (string.IsNullOrEmpty(details))
        {
            logger.LogInformation("✅ {Operation} completed successfully", operation);
        }
        else
        {
            logger.LogInformation("✅ {Operation} completed successfully - {Details}", operation, details);
        }
    }

    /// <summary>
    /// Logs exception with context
    /// </summary>
    public static void LogException(ILogger logger, Exception ex, string operation, Dictionary<string, object?>? context = null)
    {
        if (context == null || context.Count == 0)
        {
            logger.LogError(ex, "❌ Exception occurred during {Operation}", operation);
        }
        else
        {
            var contextString = string.Join(", ", context.Select(c => $"{c.Key}={c.Value}"));
            logger.LogError(ex, "❌ Exception occurred during {Operation} with context: {Context}", operation, contextString);
        }
    }

    /// <summary>
    /// Logs performance metric
    /// </summary>
    public static void LogPerformance(ILogger logger, string operation, long durationMs)
    {
        var level = durationMs > 1000 ? LogLevel.Warning :
                    durationMs > 500 ? LogLevel.Information :
                    LogLevel.Debug;

        logger.Log(level, "⏱️ {Operation} took {Duration}ms", operation, durationMs);
    }
}
