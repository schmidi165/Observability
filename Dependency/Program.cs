using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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
        .AddOtlpExporter("dashboard", opt =>
        {
            opt.Endpoint = dashboardEndpoint;
            opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }))
    .ConfigureResource(x => x.AddAttributes(attributes));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
