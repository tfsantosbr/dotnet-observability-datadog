using System.Text;
using System.Text.Json;

namespace Companies.Importer.Services;

public class CompanyProcessor(HttpClient httpClient, ILogger<CompanyProcessor> logger)
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task ProcessCompaniesAsync(
        CompanyMessage[] companies, string correlationId, CancellationToken cancellationToken = default)
    {
        foreach (var company in companies)
        {
            logger.LogInformation("Processing company '{@Company}'", company);

            using var request = new HttpRequestMessage(HttpMethod.Post, "companies")
            {
                Content = new StringContent(
                    content: JsonSerializer.Serialize(company, jsonSerializerOptions),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json")
            };

            request.Headers.Add("Correlation-Id", correlationId);

            var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            logger.LogInformation("Company '{@Company}' processed with success!", company);
        }
    }
}

public record CompanyMessage
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
