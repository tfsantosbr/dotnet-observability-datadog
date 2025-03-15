using StatsdClient;

namespace Companies.API.Extensions
{
    public static class DatadogExtensions
    {
        public static void ConfigureDatadog(this IServiceCollection services)
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
}
