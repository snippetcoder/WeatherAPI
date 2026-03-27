# Weather API

A .NET 10 Web API that provides weather forecast data for Singapore by integrating with the Singapore government's open data API.

## Overview

This API fetches real-time two-hour weather forecasts from [Singapore's Open Data API](https://api-open.data.gov.sg/v2/real-time/api/two-hr-forecast) and provides filtering capabilities by area.

## Features

- ✅ Fetch two-hour weather forecasts for Singapore
- ✅ Filter forecasts by area (case-insensitive partial match)
- ✅ API call logging to database (endpoint, status code, duration, timestamp)
- ✅ Clean architecture with service layer separation
- ✅ PostgreSQL database integration with Entity Framework Core

## API Endpoints

### Get Two-Hour Forecast

Retrieves the two-hour weather forecast for Singapore with optional area filtering.

**Endpoint:** `GET /api/weatherforecast/two-hour-forecast`

**Query Parameters:**
- `area` (optional): Filter results by area name (case-insensitive partial match)

**Examples:**

```bash
# Get all forecasts
GET https://localhost:7001/api/weatherforecast/two-hour-forecast

# Get forecast for specific area (e.g., "Ang Mo Kio")
GET https://localhost:7001/api/weatherforecast/two-hour-forecast?area=Ang Mo Kio

# Partial match works (e.g., "Ang" will match "Ang Mo Kio")
GET https://localhost:7001/api/weatherforecast/two-hour-forecast?area=Ang
```

**Response (200 OK):**
```json
{
  "code": 200,
  "errorMsg": null,
  "data": {
    "areaMetadata": [
      {
        "name": "Ang Mo Kio",
        "labelLocation": {
          "latitude": 1.375,
          "longitude": 103.839
        }
      }
    ],
    "items": [
      {
        "updateTimestamp": "2024-03-27T12:00:00Z",
        "timestamp": "2024-03-27T12:30:00Z",
        "validPeriod": {
          "start": "2024-03-27T12:30:00Z",
          "end": "2024-03-27T14:30:00Z"
        },
        "forecasts": [
          {
            "area": "Ang Mo Kio",
            "forecast": "Partly Cloudy"
          }
        ]
      }
    ]
  }
}
```

**Error Responses:**
- `404 Not Found`: No forecast data available
- `500 Internal Server Error`: Error retrieving or parsing weather data

## Architecture

### Project Structure
```
WeatherAPI/
├── Controllers/
│   └── WeatherForecastController.cs    # API endpoints
├── Services/
│   ├── IWeatherForecastService.cs      # Service interface
│   └── WeatherForecastService.cs       # Business logic & external API calls
├── Models/
│   ├── TwoHourForecastResponse.cs      # Weather data models
│   └── ApiLog.cs                       # API logging model
├── Data/
│   └── ApplicationDbContext.cs         # EF Core DbContext
└── Migrations/                         # Database migrations
```

### How It Works

1. **Controller Layer** (`WeatherForecastController`)
   - Handles HTTP requests/responses
   - Validates input parameters
   - Returns appropriate status codes

2. **Service Layer** (`WeatherForecastService`)
   - Calls Singapore's Open Data API
   - Deserializes JSON response
   - Filters data by area (if specified)
   - Logs API calls to database
   - Tracks request duration and status codes

3. **Data Layer** (`ApplicationDbContext`)
   - Stores API call logs in PostgreSQL
   - Tracks endpoint, status code, duration, and timestamp

## Setup

### Prerequisites
- .NET 10 SDK
- PostgreSQL database
- Visual Studio 2026 (or later)

### Configuration

1. **Update connection string** in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=weatherapi;Username=your_user;Password=your_password"
  }
}
```

2. **Run database migrations**:
```bash
dotnet ef database update
```

3. **Run the application**:
```bash
dotnet run
```

The API will be available at `https://localhost:7001` (or your configured port).

## Database Schema

### ApiLogs Table
| Column    | Type     | Description                          |
|-----------|----------|--------------------------------------|
| Id        | Guid     | Primary key                          |
| Endpoint  | string   | API endpoint called                  |
| StatusCode| int      | HTTP status code                     |
| Duration  | int      | Request duration in milliseconds     |
| Timestamp | DateTime | When the API call was made (UTC)     |

## External API

This project integrates with Singapore's government open data platform:

**Data Source:** [data.gov.sg](https://data.gov.sg)  
**API Endpoint:** https://api-open.data.gov.sg/v2/real-time/api/two-hr-forecast  
**Documentation:** [API Documentation](https://data.gov.sg/datasets?formats=API)

## Technologies Used

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- HttpClient with IHttpClientFactory
- System.Text.Json

## License

This project uses public data from Singapore's government open data portal.
