namespace Observability.Services;

using Microsoft.EntityFrameworkCore;
using Observability.DbContext;
using Observability.Exceptions;
using System.Diagnostics;

public sealed class WeatherForecastService(
    WeatherDbContext dbContext,
    ILogger<WeatherForecastService> logger
)
{
    public async Task<int> GetTemperatureForCity(string city)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            var count = await dbContext.Weather.CountAsync();
            logger.LogDebug("Looking up temperature in database ItemCount={Count}", count);
        }

        Stopwatch sw = new();
        sw.Start();

        var data = (await dbContext.Weather
            .Where(x => x.City == city)
            .FirstOrDefaultAsync()) ?? throw new EntityNotFoundException();

        sw.Stop();

        logger.LogInformation($"Getting temperature took {sw.ElapsedMilliseconds}ms");

        return data.Temperature;
    }

    public async Task<ICollection<WeatherForecast>> GetWeatherForecast(CancellationToken cancellationToken)
    {
        var data = await dbContext.Weather
            .Select(x => new WeatherForecast()
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                TemperatureC = x.Temperature,
                City = x.City
            }).ToListAsync(cancellationToken);

        return data;
    }
}
