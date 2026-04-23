using System.Diagnostics;
using System.Text;

namespace BusBookingAPI.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString("N").Substring(0, 8);
        context.Items["RequestId"] = requestId;

        var stopwatch = Stopwatch.StartNew();

        // Log incoming request
        await LogRequestAsync(context, requestId);

        // Capture original response stream
        var originalBodyStream = context.Response.Body;
        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[{RequestId}] Unhandled exception occurred. Duration: {Duration}ms", requestId, stopwatch.ElapsedMilliseconds);
                throw;
            }

            stopwatch.Stop();

            // Log response
            await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);

            // Copy response to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        var request = context.Request;
        var body = string.Empty;

        // Only log body for POST, PUT, PATCH requests
        if (request.Method != "GET" && request.Method != "DELETE")
        {
            request.EnableBuffering();
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }
        }

        var logMessage = new StringBuilder();
        logMessage.AppendLine($"[{requestId}] ===== HTTP REQUEST =====");
        logMessage.AppendLine($"Method: {request.Method}");
        logMessage.AppendLine($"Path: {request.Path}{request.QueryString}");
        logMessage.AppendLine($"Host: {request.Host}");
        logMessage.AppendLine($"Scheme: {request.Scheme}");

        if (request.Headers.Count > 0)
        {
            logMessage.AppendLine("Headers:");
            foreach (var header in request.Headers)
            {
                // Don't log sensitive headers
                if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                {
                    logMessage.AppendLine($"  {header.Key}: [REDACTED]");
                }
                else
                {
                    var headerValues = header.Value.ToArray();
                    logMessage.AppendLine($"  {header.Key}: {string.Join(", ", headerValues)}");
                }
            }
        }

        if (!string.IsNullOrEmpty(body))
        {
            logMessage.AppendLine($"Body: {body}");
        }

        logMessage.AppendLine($"Remote IP: {context.Connection.RemoteIpAddress}");
        logMessage.AppendLine($"User: {context.User?.Identity?.Name ?? "Anonymous"}");

        _logger.LogInformation(logMessage.ToString());
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, long duration)
    {
        var response = context.Response;
        var body = string.Empty;

        if (response.Body.CanSeek)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
            }
        }

        var logMessage = new StringBuilder();
        logMessage.AppendLine($"[{requestId}] ===== HTTP RESPONSE =====");
        logMessage.AppendLine($"Status Code: {response.StatusCode}");
        logMessage.AppendLine($"Duration: {duration}ms");

        if (response.Headers.Count > 0)
        {
            logMessage.AppendLine("Headers:");
            foreach (var header in response.Headers)
            {
                var headerValues = header.Value.ToArray();
                logMessage.AppendLine($"  {header.Key}: {string.Join(", ", headerValues)}");
            }
        }

        if (!string.IsNullOrEmpty(body) && body.Length < 5000) // Only log if body is not too large
        {
            logMessage.AppendLine($"Body: {body}");
        }
        else if (!string.IsNullOrEmpty(body))
        {
            logMessage.AppendLine($"Body: [Response body too large - {body.Length} bytes]");
        }

        var logLevel = response.StatusCode >= 500 ? LogLevel.Error :
                       response.StatusCode >= 400 ? LogLevel.Warning :
                       LogLevel.Information;

        _logger.Log(logLevel, logMessage.ToString());
    }
}
