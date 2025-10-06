using Microsoft.AspNetCore.Mvc;
using Observability.Services;
using System.Diagnostics.Metrics;

namespace Observability.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
    WeatherForecastService weatherForecastService,
    ILogger<WeatherForecastController> logger
) : ControllerBase
{
    static readonly Meter longRunningMeter = new("Observability");
    static readonly Counter<int> longRunningOperationStarted = longRunningMeter.CreateCounter<int>("LongRunningOperationStarted");
    static readonly Histogram<double> longRunningOperationHistogram = longRunningMeter.CreateHistogram<double>("random");

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get(CancellationToken cancellationToken)
    {
        return await weatherForecastService.GetWeatherForecast(cancellationToken);
    }

    [HttpGet("{city}/temperature")]
    public async Task<int> GetTemperatureForCity(string city)
    {
        ArgumentNullException.ThrowIfNull(city);

        logger.LogTrace("Getting temperature for city {City}", city);

        var temperature = await weatherForecastService.GetTemperatureForCity(city);

        logger.LogTrace("Temperature for city {City} is {Temperature}", city, temperature);
        
        return temperature;
    }

    [HttpPost]
    public async Task<IActionResult> TestLongRunningOperation([FromServices] ObservabilityService observabilityService, CancellationToken cancellationToken)
    {
        longRunningOperationStarted.Add(1);

        Random random = new();
        longRunningOperationHistogram.Record(random.NextDouble() * 1000);

        await observabilityService.TestLongRunningOperation(cancellationToken);

        return Ok();
    }

    [HttpGet("dependency")]
    public async Task<IActionResult> TestDependency([FromServices] HttpClient httpClient, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("http://dependency/testing", cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return Ok(content);
    }
}
