using Companies.API.Endpoints;
using Companies.API.Extensions;
using Companies.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDatadog();

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
