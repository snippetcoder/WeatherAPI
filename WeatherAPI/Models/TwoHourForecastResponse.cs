using System.Text.Json.Serialization;

namespace WeatherAPI.Models;

public class TwoHourForecastResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("data")]
    public ForecastData? Data { get; set; }

    [JsonPropertyName("errorMsg")]
    public string ErrorMsg { get; set; } = string.Empty;
}

public class ForecastData
{
    [JsonPropertyName("area_metadata")]
    public List<AreaMetadata> AreaMetadata { get; set; } = [];

    [JsonPropertyName("items")]
    public List<ForecastItem> Items { get; set; } = [];
}

public class AreaMetadata
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("label_location")]
    public LabelLocation? LabelLocation { get; set; }
}

public class LabelLocation
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}

public class ForecastItem
{
    [JsonPropertyName("update_timestamp")]
    public DateTime UpdateTimestamp { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("valid_period")]
    public ValidPeriod? ValidPeriod { get; set; }

    [JsonPropertyName("forecasts")]
    public List<AreaForecast> Forecasts { get; set; } = [];
}

public class ValidPeriod
{
    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    [JsonPropertyName("end")]
    public DateTime End { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class AreaForecast
{
    [JsonPropertyName("area")]
    public string Area { get; set; } = string.Empty;

    [JsonPropertyName("forecast")]
    public string Forecast { get; set; } = string.Empty;
}
