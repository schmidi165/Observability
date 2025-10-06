using Observability.Domain;

namespace Observability.Seed;

public static class WeatherSeed
{
    public static readonly List<Weather> Data = [
        new() { Id = new("dda00da3-9379-497b-bd2f-bc2b479f561e"), Temperature = 25, City = "New York" },
        new() { Id = new("e38eedaf-092c-4376-a606-45ea14b41aab"), Temperature = 30, City = "Los Angeles" },
        new() { Id = new("0ee64386-08eb-42b6-8b56-5e07fb009ec0"), Temperature = 20, City = "Chicago" },
        new() { Id = new("47b4fd08-495d-4b88-9c15-d49e29d7d07a"), Temperature = 15, City = "Houston" },
        new() { Id = new("ab59ab63-3568-4b7a-a6a1-ca84cba2f677"), Temperature = 10, City = "Phoenix" },
        new() { Id = new("aef610f8-9364-4a63-8374-7598df451027"), Temperature = 5, City = "Philadelphia" },
        new() { Id = new("93719b20-bafd-4243-9cc7-8c4cc86046f2"), Temperature = 0, City = "San Antonio" },
    ];
}
