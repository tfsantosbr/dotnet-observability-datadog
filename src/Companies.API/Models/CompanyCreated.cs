namespace Companies.API.Models
{
    public record CompanyCreated(
        string Name,
        string Country,
        string Email,
        Guid Id);
}
