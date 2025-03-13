using StatsdClient;

namespace Companies.API.Extensions;

public static class DatadogExtensions
{
    public static void AddDatadog(this IServiceCollection services)
    {
        ConfigureDatadogMetrics();
    }

    private static void ConfigureDatadogMetrics()
    {
        var dogstatsdConfig = new StatsdConfig
        {
            StatsdServerName = "host.docker.internal",
            StatsdPort = 8125,
            Prefix = "companies.api.metrics"
        };

        DogStatsd.Configure(dogstatsdConfig);
    }
}
