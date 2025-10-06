var builder = DistributedApplication.CreateBuilder(args);

var postgresUser = builder.AddParameter("postgres-user", "postgres");
var postgresPassword = builder.AddParameter("postgres-password", "mysupersecretpassword");
var postgres = builder.AddPostgres("postgres", postgresUser, postgresPassword, 5501)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithArgs("postgres", "-N", "1000")
    .WithVolume("observability-postgres", "/var/lib/postgresql/data");

var weatherDb = postgres.AddDatabase("weather", databaseName: "Weather");

var seq = builder.AddSeq("seq", port: 5511)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithVolume("observability-seq", "/data")
    .WithEnvironment("ACCEPT_EULA", "Y");

var jaeger = builder.AddContainer("jaeger", "jaegertracing/all-in-one")
    // OTLP gRPC/HTTP für OTel-Export aus deinen Services:
    .WithEndpoint(name: "otlp-grpc", targetPort: 4317, scheme: "http")
    .WithEndpoint(name: "otlp-http", targetPort: 4318, scheme: "http")
    // Jaeger UI:
    .WithHttpEndpoint(name: "ui", port: 16686, targetPort: 16686);

var dependency = builder.AddProject<Projects.Dependency>("dependency")
    .WithEnvironment("JAEGER_ENDPOINT", jaeger.GetEndpoint("otlp-grpc"));

var observability = builder.AddProject<Projects.Observability>("observability")
    .WithReference(weatherDb, "WeatherConnection")
    .WaitFor(weatherDb)

    // NOTE: important so service discovery can resolve "dependency" to the correct container
    .WithReference(dependency)

    .WithEnvironment("Serilog__WriteTo__0__Args__serverUrl", seq)

    .WithEnvironment("JAEGER_ENDPOINT", jaeger.GetEndpoint("otlp-grpc"))
;

var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
    // Konfig einbinden (siehe Abschnitt 2):
    .WithBindMount("./prometheus", "/etc/prometheus", isReadOnly: true)
    // (Optional) OTLP-Receiver von Prometheus aktivieren – praktisch, wenn du OTLP-Metrics pushen willst:
    .WithArgs("--web.enable-otlp-receiver", "--config.file=/etc/prometheus/prometheus.yml")
    .WithHttpEndpoint(name: "http", targetPort: 9090)
    .WithReference(observability);

builder.Build().Run();
