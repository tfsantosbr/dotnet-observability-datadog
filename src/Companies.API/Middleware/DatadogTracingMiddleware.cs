using Datadog.Trace;

namespace Companies.API.Middleware;

public class DatadogTracingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Items["Correlation-Id"]?.ToString();

        using var scope = Tracer.Instance.StartActive("http.request");
        scope.Span.Type = SpanTypes.Web;
        scope.Span.ResourceName = $"{context.Request.Method} {context.Request.Path} (custom)";
        scope.Span.SetTag("correlationId", correlationId);
        scope.Span.SetTag("http.method", context.Request.Method);
        scope.Span.SetTag("http.url", context.Request.Path);

        try
        {
            await next(context);
            scope.Span.SetTag("http.status_code", context.Response.StatusCode.ToString());
        }
        catch (Exception ex)
        {
            scope.Span.SetException(ex);
            throw;
        }
    }
}

