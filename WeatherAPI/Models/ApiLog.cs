using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models;

public class ApiLog
{
    [Key]
    public Guid Id { get; set; }

    public string Endpoint { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public int Duration { get; set; }

    public DateTime Timestamp { get; set; }
}
