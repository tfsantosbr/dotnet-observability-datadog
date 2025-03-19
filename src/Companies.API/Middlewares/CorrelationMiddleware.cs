using Companies.API.Constants;

namespace Companies.API.Middlewares;

public class CorrelationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationConstants.CORRELATION_HEADER, out var correlationValues))
        {
            await next(context);
            return;
        }

        var correlationId = correlationValues.First();

        context.Items[CorrelationConstants.CORRELATION_HEADER] = correlationId;

        context.Response.Headers[CorrelationConstants.CORRELATION_HEADER] = correlationId;

        await next(context);
    }
}
