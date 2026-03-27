using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(IWeatherForecastService weatherForecastService, ILogger<WeatherForecastController> logger)
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    [HttpGet("two-hour-forecast")]
    public async Task<IActionResult> GetTwoHourForecast([FromQuery] string? area = null)
    {
        try
        {
            var forecastResponse = await _weatherForecastService.GetTwoHourForecastAsync(area);

            if (forecastResponse == null)
            {
                return NotFound("No forecast data available");
            }

            return Ok(forecastResponse);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling weather API");
            return StatusCode(500, "Error retrieving weather data");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing weather data");
            return StatusCode(500, "Error parsing weather data");
        }
    }
}
