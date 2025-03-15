using Companies.API.Constants;

namespace Companies.API.Middleware
{
    public class CorrelationMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers[CorrelationConstants.CORRELATION_HEADER].FirstOrDefault();

            if (correlationId is null)
                await next(context);

            context.Items[CorrelationConstants.CORRELATION_HEADER] = correlationId;

            context.Response.Headers[CorrelationConstants.CORRELATION_HEADER] = correlationId;

            await next(context);
        }
    }
}
