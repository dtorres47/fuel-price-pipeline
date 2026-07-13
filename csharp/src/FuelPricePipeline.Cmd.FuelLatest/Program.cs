namespace FuelPricePipeline.Cmd.FuelLatest;

using FuelPricePipeline.Infra.Eia;
using FuelPricePipeline.Infra.Postgres;
using FuelPricePipeline.UseCase.GetLatest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        // Get configuration from environment variables
        var apiKey = Environment.GetEnvironmentVariable("EIA_API_KEY") ?? "";
        var dsn = Environment.GetEnvironmentVariable("FUEL_DSN") ?? "";

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(dsn))
        {
            Console.WriteLine("Missing required environment variables: EIA_API_KEY or FUEL_DSN");
            Environment.Exit(1);
            return;
        }

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Register HttpClient
                services.AddHttpClient("EiaClient", client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

                // Register infrastructure services
                services.AddSingleton(sp => new Client(
                    apiKey,
                    sp.GetRequiredService<IHttpClientFactory>(),
                    sp.GetRequiredService<ILogger<Client>>()));

                services.AddSingleton(new Repo(dsn));

                // Register use case
                services.AddTransient<Executor>();

                // Configure logging
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });
            })
            .Build();

        var executor = host.Services.GetRequiredService<Executor>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        var outputPath = Environment.GetEnvironmentVariable("FUEL_OUT") ?? "fuel-latest.csv";
        var area = Environment.GetEnvironmentVariable("FUEL_AREA") ?? "NUS";

        logger.LogInformation("Starting fuel downloader for area {Area}", area);

        var result = await executor.ExecuteAsync(outputPath, area);

        if (result.IsSuccess && result.Data != null)
        {
            var fr = result.Data;
            Console.WriteLine($" {fr.ProductCode} {fr.Period:yyyy-MM} {fr.Value} {fr.Unit} → {outputPath}");
        }
        else
        {
            logger.LogError("Failed to get fuel rate: {Error}", result.ErrorMessage);
            Console.WriteLine($" {result.ErrorMessage}");
            Environment.Exit(1);
        }
    }
}