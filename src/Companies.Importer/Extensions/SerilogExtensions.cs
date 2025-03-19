using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Companies.Importer.Extensions;

public static class SerilogExtensions
{
    public static void ConfigureSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        var logFilePath = configuration["LogFilePath"]!;

        services.AddSerilog((services, lc) => lc
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.With(new CorrelationEnricher())
            .WriteTo.Console()
            .WriteTo.File(new JsonFormatter(renderMessage: true), logFilePath)
        );
    }
}

public class CorrelationEnricher() : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // TODO: Implement correlation enricher
        // var correlationId = httpContextAccessor.HttpContext?
        //     .Items[CorrelationConstants.CORRELATION_HEADER]?.ToString();

        // if (!string.IsNullOrEmpty(correlationId))
        // {
        //     var correlationProperty = propertyFactory.CreateProperty(
        //         CorrelationConstants.CORRELATION_PROPERTY, correlationId);

        //     logEvent.AddPropertyIfAbsent(correlationProperty);
        // }
    }
}
