namespace Companies.API.Middleware
{
    public class CorrelationIdMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers["Correlation-Id"].FirstOrDefault();

            if (correlationId is null)
                await next(context);

            context.Items["Correlation-Id"] = correlationId;

            context.Response.Headers["Correlation-Id"] = correlationId;

            await next(context);
        }
    }
}
