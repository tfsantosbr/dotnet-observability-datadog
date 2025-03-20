
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Companies.Importer.Services;
using Companies.Importer.Tracing;
using Datadog.Trace;
using Serilog;
using Serilog.Context;

namespace Companies.Importer.Workers;

public class CompaniesImporterWorker : BackgroundService
{
    private readonly ServiceBusReceiver _receiver;
    private readonly ILogger<CompaniesImporterWorker> _logger;
    private readonly CompanyProcessor _companyProcessor;
    private readonly string _topicName;
    private readonly string _subscriptionName;

    public CompaniesImporterWorker(
        ILogger<CompaniesImporterWorker> logger,
        ServiceBusClient serviceBusClient,
        IConfiguration configuration,
        CompanyProcessor companyProcessor)
    {
        _topicName = configuration["ServiceBus:ImportCompanies:Topic"]!;
        _subscriptionName = configuration["ServiceBus:ImportCompanies:Subscription"]!;

        _receiver = serviceBusClient.CreateReceiver(_topicName, _subscriptionName);
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

                if (receivedMessage == null)
                    continue;

                using (LogContext.PushProperty("correlation_id", receivedMessage.CorrelationId))
                {
                    using (DatadogTracer tracer = CreateTracer(receivedMessage))
                    {

                        try
                        {
                            string messageBody = receivedMessage.Body.ToString();

                            _logger.LogInformation("Mensagem recebida: {@MessageBody}", messageBody);

                            var companies = JsonSerializer.Deserialize<CompanyMessage[]>(messageBody) ??
                                throw new Exception("Mensagem JSON inválida.");

                            await _companyProcessor.ProcessCompaniesAsync(
                                companies: companies,
                                correlationId: receivedMessage.CorrelationId,
                                cancellationToken: stoppingToken
                                );

                            await _receiver.CompleteMessageAsync(receivedMessage, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro ao processar mensagem.");
                            tracer.SetError(ex);

                            throw;
                        }
                    }
                }


            }
        }
        finally
        {
            await _receiver.DisposeAsync();
        }
    }

    private DatadogTracer CreateTracer(ServiceBusReceivedMessage receivedMessage)
    {
        var tracer = DatadogTracer.Start(
            operationName: "servicebus.messaging",
            resourceName: $"CONSUME: {_topicName}/{_subscriptionName}",
            spanType: SpanTypes.Queue,
            spanKing: SpanKinds.Consumer
        );

        tracer.SetTag("correlation_id", receivedMessage.CorrelationId);
        tracer.SetTag("message.id", receivedMessage.MessageId);
        tracer.SetTag("message.topic", _topicName);
        tracer.SetTag("message.subscription", _subscriptionName);

        return tracer;
    }
}
