using Companies.API.Endpoints;
using Companies.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddOpenApi();
builder.Services.ConfigureDatadog();
builder.Services.ConfigureSerilog(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
//app.UseMiddleware<CorrelationIdMiddleware>();
//app.UseMiddleware<DatadogTracingMiddleware>();

app.MapCompanyEndpoints();

app.Run();
