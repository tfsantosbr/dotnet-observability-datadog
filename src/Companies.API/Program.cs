using Companies.API.Endpoints;
using Companies.API.Extensions;
using Companies.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureDatadog(configuration);
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
