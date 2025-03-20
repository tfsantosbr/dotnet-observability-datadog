using Datadog.Trace;

namespace Companies.Importer.Tracing;

public class DatadogTracer : IDisposable
{
    private readonly IScope _scope;
    private bool _disposed = false;

    public DatadogTracer(
        string operationName, string resourceName, string spanType, string? spanKing = null)
    {
        _scope = Tracer.Instance.StartActive(operationName);
        _scope.Span.ResourceName = resourceName;
        _scope.Span.Type = spanType;

        SetTag(Tags.SpanKind, spanKing ?? SpanKinds.Client);
    }

    public static DatadogTracer Start(
        string operationName, string resourceName, string spanType, string? spanKing = null)
    {
        return new DatadogTracer(operationName, resourceName, spanType, spanKing);
    }

    public void SetTag(string key, string value)
    {
        _scope.Span.SetTag(key, value);
    }

    public void SetError(Exception ex)
    {
        _scope.Span.SetException(ex);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _scope.Dispose();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}