using Companies.Importer.Extensions;
using Companies.Importer.Services;
using Companies.Importer.Workers;
using Microsoft.Extensions.Azure;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

builder.Services.ConfigureDatadog(configuration);
builder.Services.ConfigureSerilog(configuration);
builder.Services.AddAzureClients(clientBuilder =>
    clientBuilder.AddServiceBusClient(configuration.GetConnectionString("ServiceBus"))
);

builder.Services.AddTransient<CompanyProcessor>();

builder.Services.AddHttpClient<CompanyProcessor>(client =>
{
    client.BaseAddress = new Uri(configuration["Clients:CompaniesApi:BaseUrl"]!);
});

builder.Services.AddHostedService<CompaniesImporterWorker>();

var host = builder.Build();
host.Run();