using Microsoft.EntityFrameworkCore;
using Npgsql;
using Observability.DbContext;
using Observability.Extensions;
using Observability.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// NOTE: thats all you need to configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.AddServiceDefaults();

// NOTE: the following will be needed for open telemetry
List<KeyValuePair<string, object>> attributes = [
    new("service.name", builder.Environment.ApplicationName),
    new("service.version", Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown"),
];

var jaegerEndpoint = builder.Configuration.GetValue<Uri>("JAEGER_ENDPOINT") ?? throw new ArgumentNullException("JAEGER_ENDPOINT must be set!");
var dashboardEndpoint = builder.Configuration.GetValue<Uri>("OTEL_EXPORTER_OTLP_ENDPOINT") ?? throw new ArgumentNullException("OTEL_EXPORTER_OTLP_ENDPOINT must be set!");

builder.Services.AddOpenTelemetry()
    .WithTracing(traces => traces
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddNpgsql()        // needs nuget Npgsql.OpenTelemetry!
        .AddCustomEvent()   // custom event, will be implemented in ObservabilityService
                            // NOTE: usually AddOtlpExporter doesn't need to be set up like this. It will work with the OTEL_EXPORTER_OTLP_ENDPOINT environment
                            //       out of the box. It's only done this way, so we can show the dashboard and jaeger view simultanously. In any other scenario
                            //       we would probably use the dashboard in local development and maybe jaeger for testing and production. 
        .AddOtlpExporter("jaeger", opt =>
        {
            opt.Endpoint = jaegerEndpoint;
            opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })
        .AddOtlpExporter("dashboard", opt =>
        {
            opt.Endpoint = dashboardEndpoint;
            opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter()
        .AddMeter("Observability") // our custom meter in WeatherForecastController
        .AddOtlpExporter("dashboard", opt =>
        {
            opt.Endpoint = dashboardEndpoint;
            opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }))
    .ConfigureResource(x => x.AddAttributes(attributes));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<WeatherDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("WeatherConnection"));
});

builder.Services.AddScoped<WeatherForecastService>();
builder.Services.AddScoped<ObservabilityService>();


var app = builder.Build();

var test = app.Configuration.GetValue<string>("OTEL_EXPORTER_OTLP_ENDPOINT");

// NOTE: this is the cheap way to apply migrations at startup
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
    dbContext.Database.Migrate();
}

app.MapDefaultEndpoints();
// NOTE: adds the /metrics endpoint
app.MapPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
