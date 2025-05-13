using System;

namespace ColonySync.WebApp;

public sealed record WeatherForecast(DateOnly Date, string? Summary, int TemperatureC)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}