using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WeatherForecast
{
    public static class OpenTelemetryExtensions
    {
        public static void ConfigureOpenTelemetry(this WebApplicationBuilder appBuilder)
        {
            // Build a resource configuration action to set service information.
            Action<ResourceBuilder> configureResource = r => r.AddService(
                serviceName: appBuilder.Configuration.GetValue("ServiceName", defaultValue: "otel-test")!,
                serviceVersion: typeof(OpenTelemetryExtensions).Assembly.GetName().Version?.ToString() ?? "unknown",
                serviceInstanceId: Environment.MachineName);

            // Create a service to expose ActivitySource, and Metric Instruments
            // for manual instrumentation
            appBuilder.Services.AddSingleton<Instrumentation>();

            // Configure OpenTelemetry tracing & metrics with auto-start using the
            // AddOpenTelemetry extension from OpenTelemetry.Extensions.Hosting.
            appBuilder.Services.AddOpenTelemetry()
                .ConfigureResource(configureResource)
                .WithTracing(builder =>
                {
                    builder.AddSource(Instrumentation.ActivitySourceName)
                        .SetSampler(new AlwaysOnSampler())
                        .AddOtlpExporter(otlpOptions =>
                            {
                                // Use IConfiguration directly for Otlp exporter endpoint option.
                                otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                            });
                })
                .WithMetrics(builder =>
                {
                    builder.AddMeter(Instrumentation.MeterName)
                        .AddOtlpExporter(otlpOptions =>
                    {
                        // Use IConfiguration directly for Otlp exporter endpoint option.
                        otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                    });

                    builder.AddPrometheusExporter();
                });

            // Clear default logging providers used by WebApplication host.
            appBuilder.Logging.ClearProviders();

            // Configure OpenTelemetry Logging.
            appBuilder.Logging.AddOpenTelemetry(options =>
            {
                var resourceBuilder = ResourceBuilder.CreateDefault();
                configureResource(resourceBuilder);
                options.SetResourceBuilder(resourceBuilder);

                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.AddOtlpExporter(otlpOptions =>
                {
                    // Use IConfiguration directly for Otlp exporter endpoint option.
                    otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                });
            });
        }
    }
}
