using System.Diagnostics;
using System.Text.Json;
using WeatherAPI.Data;
using WeatherAPI.Models;

namespace WeatherAPI.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherForecastService> _logger;
    private readonly ApplicationDbContext _dbContext;
    private const string TwoHourForecastUrl = "https://api-open.data.gov.sg/v2/real-time/api/two-hr-forecast";

    public WeatherForecastService(IHttpClientFactory httpClientFactory, ILogger<WeatherForecastService> logger, ApplicationDbContext dbContext)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<TwoHourForecastResponse?> GetTwoHourForecastAsync(string? area = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var endpoint = $"api/weatherforecast/two-hour-forecast{(string.IsNullOrWhiteSpace(area) ? "" : $"?area={area}")}";
        var statusCode = 200;

        try
        {
            var response = await _httpClient.GetAsync(TwoHourForecastUrl);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

            var forecastResponse = JsonSerializer.Deserialize<TwoHourForecastResponse>(jsonString);

            if (forecastResponse?.Data == null)
            {
                statusCode = 404;
                return null;
            }

            // If no area filter is provided, return all items
            if (string.IsNullOrWhiteSpace(area))
            {
                return forecastResponse;
            }

            // Filter items by area (case-insensitive partial match)
            var filteredResponse = new TwoHourForecastResponse
            {
                Code = forecastResponse.Code,
                ErrorMsg = forecastResponse.ErrorMsg,
                Data = new ForecastData
                {
                    AreaMetadata = forecastResponse.Data.AreaMetadata
                        .Where(a => a.Name.Contains(area, StringComparison.OrdinalIgnoreCase))
                        .ToList(),
                    Items = forecastResponse.Data.Items.Select(item => new ForecastItem
                    {
                        UpdateTimestamp = item.UpdateTimestamp,
                        Timestamp = item.Timestamp,
                        ValidPeriod = item.ValidPeriod,
                        Forecasts = item.Forecasts
                            .Where(f => f.Area.Contains(area, StringComparison.OrdinalIgnoreCase))
                            .ToList()
                    }).ToList()
                }
            };

            return filteredResponse;
        }
        catch (HttpRequestException ex)
        {
            statusCode = 500;
            _logger.LogError(ex, "Error calling weather API");
            throw;
        }
        catch (JsonException ex)
        {
            statusCode = 500;
            _logger.LogError(ex, "Error parsing weather data");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            await LogApiCallAsync(endpoint, statusCode, (int)stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task LogApiCallAsync(string endpoint, int statusCode, int duration)
    {
        try
        {
            var apiLog = new ApiLog
            {
                Id = Guid.NewGuid(),
                Endpoint = endpoint,
                StatusCode = statusCode,
                Duration = duration,
                Timestamp = DateTime.UtcNow
            };

            _dbContext.ApiLogs.Add(apiLog);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging API call");
        }
    }
}
