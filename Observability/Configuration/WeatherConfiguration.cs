namespace Observability.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Observability.Domain;
using Observability.Seed;

public sealed class WeatherConfiguration : IEntityTypeConfiguration<Weather>
{
    public void Configure(EntityTypeBuilder<Weather> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasData(WeatherSeed.Data);
    }
}
