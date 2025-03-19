
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Companies.Importer.Services;

namespace Companies.Importer.Workers;

public class CompaniesImporterWorker : BackgroundService
{
    private readonly ServiceBusReceiver _receiver;
    private readonly ILogger<CompaniesImporterWorker> _logger;
    private readonly CompanyProcessor _companyProcessor;

    public CompaniesImporterWorker(
        ILogger<CompaniesImporterWorker> logger,
        ServiceBusClient serviceBusClient,
        IConfiguration configuration,
        CompanyProcessor companyProcessor)
    {
        string topicName = configuration["ServiceBus:ImportCompanies:Topic"]!;
        string subscriptionName = configuration["ServiceBus:ImportCompanies:Subscription"]!;

        _receiver = serviceBusClient.CreateReceiver(topicName, subscriptionName);
        _logger = logger;
        _companyProcessor = companyProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Aguardando mensagens...");

            while (!stoppingToken.IsCancellationRequested)
            {
                var receivedMessage = await _receiver.ReceiveMessageAsync(cancellationToken: stoppingToken);

                if (receivedMessage != null)
                {
                    string messageBody = receivedMessage.Body.ToString();
                    _logger.LogInformation("Mensagem recebida: {@MessageBody}", messageBody);

                    var companies = JsonSerializer.Deserialize<CompanyMessage[]>(messageBody) ??
                        throw new Exception("Mensagem JSON inválida.");

                    await _companyProcessor.ProcessMessageAsync(companies);

                    await _receiver.CompleteMessageAsync(receivedMessage, stoppingToken);
                }
            }
        }
        finally
        {
            await _receiver.DisposeAsync();
        }
    }
}
