namespace Observability;

public sealed record WeatherForecast
{
    public DateOnly Date { get; init; }
    
    public int TemperatureC { get; init; }

    public required string City { get; init; }
}
