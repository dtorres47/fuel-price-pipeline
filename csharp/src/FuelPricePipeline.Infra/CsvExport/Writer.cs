namespace FuelPricePipeline.Infra.CsvExport;

using FuelPricePipeline.Domain;
using System.Globalization;

public static class Writer
{
    public static async Task WriteAsync(string filePath, DieselFuelPrice fuelRate)
    {
        using var writer = new StreamWriter(filePath, false); // overwrite
        await writer.WriteLineAsync("product_code,product_name,area_code,area_name,period,value,unit,generated_utc");

        await writer.WriteLineAsync(string.Join(",",
            fuelRate.ProductCode,
            fuelRate.ProductName,
            fuelRate.AreaCode,
            fuelRate.AreaName,
            fuelRate.Period.ToString("yyyy-MM"),
            fuelRate.Value.ToString("F4", CultureInfo.InvariantCulture),
            fuelRate.Unit,
            DateTime.UtcNow.ToString("o")));
    }
}
