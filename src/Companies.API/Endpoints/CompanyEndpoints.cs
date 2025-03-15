using Companies.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using StatsdClient;

namespace Companies.API.Endpoints
{
    public static class CompanyEndpoints
    {
        public static void MapCompanyEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("/companies", CreateCompany);
        }

        private static IResult CreateCompany(CreateCompany createCompany, ILogger<Program> logger)
        {
            logger.LogInformation("Creating company {@CreateCompany}", createCompany);

            var companyCreated = new CompanyCreated(
                createCompany.Name,
                createCompany.Country,
                createCompany.Email,
                Guid.NewGuid());

            logger.LogInformation("Company created {@CompanyCreated}", companyCreated);

            DogStatsd.Increment("companies.created.total");

            return Results.Created($"companies/{companyCreated.Id.ToString()}", companyCreated);
        }
    }
}
