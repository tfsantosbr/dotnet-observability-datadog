FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /build
COPY src/Companies.Importer/Companies.Importer.csproj src/Companies.Importer/
RUN dotnet restore "src/Companies.Importer/Companies.Importer.csproj"
COPY . .
WORKDIR "/build/src/Companies.Importer"
RUN dotnet build "Companies.Importer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Companies.Importer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

# Enable Datadog automatic instrumentation
# App is being copied to /app, so Datadog assets are at /app/datadog
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/app/datadog/linux-x64/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/app/datadog

# Run the createLogPath script on Linux to ensure the automatic instrumentation logs are generated without permission issues
RUN /app/datadog/createLogPath.sh

ENTRYPOINT ["dotnet", "Companies.Importer.dll"]