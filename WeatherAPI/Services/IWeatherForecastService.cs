using WeatherAPI.Models;

namespace WeatherAPI.Services;

public interface IWeatherForecastService
{
    Task<TwoHourForecastResponse?> GetTwoHourForecastAsync(string? area = null);
}
