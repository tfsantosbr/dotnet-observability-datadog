# dotnet-observability-datadog

## METRICS

1. Configurar o dogstatsd no docker-compose.yml
2. Adicionar a lib `DogStatsD-CSharp-Client`
3. Configurar a lib (exemplo em [DatadogExtensions.ConfigureDatadogMetrics()](src/Companies.API/Extensions/DatadogExtensions.cs))

## TRACING

1. Configurar o service `datadog` no docker-compose.yml
2. Adicionar a lib `Datadog.Trace.Bundle` via nuget
3. Adicionar configurações do Datadog no [Dockerfile da Aplicação (linha 22-30)](src/Companies.API/Dockerfile)
4. Adicionar variaveis de ambientes no service `datadog` e na aplicação `companies-api`

## LOGS

1. Instalar a lib do `Serilog` via nuget
2. Configurar a lib (Exemplo em [SerilogExtensions.cs](src/Companies.API/Extensions//SerilogExtensions.cs))
3. Recomenda-se que os logs devem ser exportados em arquivo no formato json para facilitar a indexação do datadog
4. Configurar um volume para ser um caminho em comum entra a aplicação e o agente que irá ler os logs
5. Adicionar o arquivo de configuração que irá especificar onde os logs devem ser encontrados pelo agende do datadog
6. Configurar o agente do datadog para ler o arquivo de configuração

```yml
# docker-compose.yml
companies-api:
  volumes:
    - ./logs:/app/logs # diretório que a aplicação irá gravar os logs

datadog:
  volumes:
    - ./config/datadog:/conf.d/ # diretório do arquivo de configuração
    - ./logs:/app/logs # diretório com os logs que o agente irá ler
```

```yml
# cofig/datadog/conf.yaml
logs:
  - type: file
    path: /app/logs/logs.json
    service: companies-api
    source: csharp
    sourcecategory: sourcecode
    format: json
```

## CORRELATION ID

1. Criar um middleware para receber o correlation id pelo header e adicionar no contexto do .net ([CorrelationMiddleware.cs](/src/Companies.API/Middlewares/CorrelationMiddleware.cs))
2. Para os traces - Configurar a variavel de ambiente do datadog `DD_TRACE_HEADER_TAGS=Correlation-Id:CorrelationId`, sendo `Correlation-Id` o nome do header que está no context do .net e `CorrelationId` a propriedade que será adicionada nos traces do datadog.
3. Para os logs, criar um Enricher para o `Serilog` e adicionar a propriedade do CorrelationId nos logs (exemplo [CorrelationEnricher](src/Companies.API/Extensions/SerilogExtensions.cs))
