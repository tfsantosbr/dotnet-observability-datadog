using Companies.API.Endpoints;
using Companies.API.Extensions;
using Companies.API.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureDatadog();
builder.Services.ConfigureSerilog(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<CorrelationMiddleware>();

app.MapCompanyEndpoints();

app.Run();
