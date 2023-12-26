using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Saharaviewpoint.Core.Models.Utilities;
using System.Diagnostics;
using System.Text.Json;

namespace Saharaviewpoint.Core.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            _logger.LogError("Actual Error: {Error}", error);

            response.StatusCode = error switch
            {
                KeyNotFoundException e => StatusCodes.Status404NotFound,// not found error
                _ => StatusCodes.Status500InternalServerError,// unhandled error
            };

            var result = JsonSerializer.Serialize(new ErrorResult
            {
                Success = false,
                Message = error?.Message,
                Status = response.StatusCode,
                Detail = error?.InnerException?.Message,
                Instance = Guid.NewGuid().ToString(),
                Path = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}",
                TraceInfo = GetErrorTraceInfo(error),
            }, _options);

            _logger.LogError("Error: {@result}", result);

            await response.WriteAsync(result);
        }

        // handle unauthorized error
        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            var result = JsonSerializer.Serialize(new ErrorResult
            {
                Success = false,
                Message = "Authentication failed, please log in to access this resource",
                Status = StatusCodes.Status401Unauthorized,
            }, _options);

            // Set the response content type to JSON
            context.Response.ContentType = "application/json";

            // Write the JSON response
            await context.Response.WriteAsync(result);
        }
    }

    private TraceInfo GetErrorTraceInfo(Exception ex)
    {
        //Get a StackTrace object for the exception
        StackTrace st = new StackTrace(ex, true);

        var traceInfo = new List<TraceInfo>();

        List<StackFrame> frames = st.GetFrames().Where(x => x.GetFileName() != null).ToList();

        var frame = frames.FirstOrDefault();

        if (frame == null) return new TraceInfo();

        TraceInfo trace = new TraceInfo
        {
            FileName = frame.GetFileName(),
            MethodName = frame.GetMethod().Name,
            LineNumber = frame.GetFileLineNumber(),
            ColumnNumber = frame.GetFileColumnNumber()
        };

        return trace;
    }
}