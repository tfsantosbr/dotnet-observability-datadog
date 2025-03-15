namespace Companies.API.Middleware
{
    public class CorrelationLoggingMiddleware(RequestDelegate next, ILogger<CorrelationLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Items["Correlation-Id"]?.ToString();

            if (correlationId is null)
            {
                await next(context);
                return;
            }

            using (logger.BeginScope(new Dictionary<string, string> { ["CorrelationId"] = correlationId }))
            {
                await next(context);
            }
        }
    }

}
