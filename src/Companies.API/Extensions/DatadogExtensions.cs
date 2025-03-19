using StatsdClient;

namespace Companies.API.Extensions
{
    public static class DatadogExtensions
    {
        public static void ConfigureDatadog(this IServiceCollection services, IConfiguration configuration)
        {
            var dogstatsdConfig = new StatsdConfig
            {
                StatsdServerName = configuration["Datadog:MetricsAgent"],
                Prefix = configuration["Datadog:MetricsPrefix"],
                StatsdPort = 8125
            };

            DogStatsd.Configure(dogstatsdConfig);
        }
    }
}
