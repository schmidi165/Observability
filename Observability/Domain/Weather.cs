namespace Observability.Domain;

public sealed class Weather
{
    public Guid Id { get; set; }

    public string City { get; set; } = null!;

    public int Temperature { get; set; }
}
