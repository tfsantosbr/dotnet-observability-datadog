using Serilog;
using Serilog.Formatting.Json;

namespace Companies.API.Extensions;

public static class SerilogExtensions
{
    public static void ConfigureSerilog(this IServiceCollection services)
    {
        services.AddSerilog((services, lc) => lc
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(new JsonFormatter(renderMessage: true), "log.json")
        );
    }
}
