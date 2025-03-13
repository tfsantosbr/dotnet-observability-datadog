namespace Companies.API.Models;

public record CreateCompany(
    string Name,
    string Country,
    string Email);