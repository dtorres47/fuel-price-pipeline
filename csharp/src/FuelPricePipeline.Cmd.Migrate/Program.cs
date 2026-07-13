namespace FuelPricePipeline.Cmd.Migrate;

using FuelPricePipeline.Infra.Postgres;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        var dsn = Environment.GetEnvironmentVariable("FUEL_DSN") ?? "";

        if (string.IsNullOrWhiteSpace(dsn))
        {
            Console.WriteLine("Missing FUEL_DSN environment variable.");
            Environment.Exit(1);
            return;
        }

        // Create a simple console logger
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        var logger = loggerFactory.CreateLogger<Program>();

        try
        {
            await DbInitializer.ApplyMigrationsAsync(dsn, logger);
            Console.WriteLine("Migration applied successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
            Environment.Exit(1);
        }
    }
}