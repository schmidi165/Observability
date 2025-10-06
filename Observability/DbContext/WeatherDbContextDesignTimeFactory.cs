namespace Observability.DbContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

#if DEBUG

public sealed class WeatherDbContextDesignTimeFactory: IDesignTimeDbContextFactory<WeatherDbContext>
{
    public WeatherDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WeatherDbContext>()
            .UseNpgsql("Server=localhost;Port=5501;Database=Weather;User Id=postgres;Password=mysupersecretpassword;");

        return new WeatherDbContext(optionsBuilder.Options);
    }
}

#endif
