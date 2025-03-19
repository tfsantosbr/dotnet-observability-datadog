using System.Text;
using System.Text.Json;

namespace Companies.Importer.Services;

public class CompanyProcessor(HttpClient httpClient, ILogger<CompanyProcessor> logger)
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task ProcessMessageAsync(CompanyMessage[] companies)
    {
        foreach (var company in companies)
        {
            var requestUrl = "companies";

            var companyJson = JsonSerializer.Serialize(company, jsonSerializerOptions);

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(companyJson, Encoding.UTF8, "application/json")
            };

            // TODO add correlation Id
            //request.Headers.Add("Correlation-Id", "123e4567-e89b-12d3-a456-426614174000");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            logger.LogInformation("Company '{@ProcessedCompany}' processed with success!", company);
        }
    }
}

public record CompanyMessage
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
