using System.Text.Json;
using Azure.Messaging.ServiceBus;

var connectionString = "Endpoint=sb://host.docker.internal;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
var topicName = "sb-import-companies-topic";

await using var client = new ServiceBusClient(connectionString);
ServiceBusSender sender = client.CreateSender(topicName);

var companyMessages = new[]
{
    new CompanyMessage("Company Test 1", "Brasil", "company1@email.com"),
    new CompanyMessage("Company Test 2", "Brasil", "company2@email.com"),
    new CompanyMessage("Company Test 3", "Brasil", "company3@email.com")
};

var jsonMessage = JsonSerializer.Serialize(companyMessages);

var message = new ServiceBusMessage(jsonMessage)
{
    CorrelationId = Guid.NewGuid().ToString()
};

await sender.SendMessageAsync(message);

Console.WriteLine("Mensagem enviada com sucesso. Correlation Id: '{0}'", message.CorrelationId);

record CompanyMessage(string Name, string Country, string Email);
