using FuelPricePipeline.Domain;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace FuelPricePipeline.Infra.Eia
{
    public class Client
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Client> _logger;

        public Client(string apiKey, IHttpClientFactory httpClientFactory, ILogger<Client> logger)
        {
            _apiKey = apiKey;
            _httpClient = httpClientFactory.CreateClient("EiaClient");
            _logger = logger;
        }

        private static readonly HashSet<string> ValidAreas = new HashSet<string>()
        {
            "NUS", // U.S. National
            "PADD1", "PADD1A", "PADD1B", "PADD1C", // East Coast regions
            "PADD2", // Midwest
            "PADD3", // Gulf Coast
            "PADD4", // Rocky Mountain
            "PADD5" // West Coast
        };

        public async Task<DieselFuelPrice?> FetchLatestDieselAsync(string area = "NUS")
        {
            if (ValidAreas.Contains(area) == false)
            {
                _logger.LogError("Invalid area code: {Area}", area);
                throw new ArgumentException($"Invalid area code: {area}. Valid codes: {string.Join(", ", ValidAreas)}", nameof(area));
            }

            try
            {
                var url = BuildUrl(area);
                _logger.LogDebug("Fetching fuel rate from EIA API: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("EIA API returned {StatusCode} for area {Area}",
                        response.StatusCode, area);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                return ParseResponse(json);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error getting fuel rate for area {Area}", area);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse EIA response for area {Area}", area);
                return null;
            }
        }

        private string BuildUrl(string area)
        {
            var builder = new StringBuilder("https://api.eia.gov/v2/petroleum/pri/gnd/data/?");
            builder.Append("frequency=monthly");
            builder.Append("&data[0]=value");
            builder.Append("&facets[product][]=EPD2D");
            builder.Append($"&facets[duoarea][]={Uri.EscapeDataString(area)}");
            builder.Append("&sort[0][column]=period&sort[0][direction]=desc");
            builder.Append("&offset=0&length=1");
            builder.Append($"&api_key={_apiKey}");
            return builder.ToString();
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private DieselFuelPrice? ParseResponse(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json, new JsonDocumentOptions
                {
                    AllowTrailingCommas = true
                });

                if (!doc.RootElement.TryGetProperty("response", out var response) ||
                    !response.TryGetProperty("data", out var data) ||
                    data.GetArrayLength() == 0)
                {
                    _logger.LogWarning("EIA response contains no data");
                    return null;
                }

                var firstItem = data[0];

                return new DieselFuelPrice
                {
                    ProductCode = firstItem.GetProperty("product").GetString() ?? "",
                    ProductName = firstItem.GetProperty("product-name").GetString() ?? "",
                    AreaCode = firstItem.GetProperty("duoarea").GetString() ?? "",
                    AreaName = firstItem.GetProperty("area-name").GetString() ?? "",
                    Period = DateTime.Parse(firstItem.GetProperty("period").GetString() ?? DateTime.UtcNow.ToString("yyyy-MM")),
                    Value = decimal.Parse(firstItem.GetProperty("value").GetString() ?? "0"),
                    Unit = firstItem.GetProperty("units").GetString() ?? ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing EIA response");
                throw new JsonException("Failed to parse EIA API response", ex);
            }
        }
    }
}