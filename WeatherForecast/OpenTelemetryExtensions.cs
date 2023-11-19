using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace WeatherForecast
{
    public static class OpenTelemetryExtensions
    {
        public static void ConfigureOpenTelemetry(this WebApplicationBuilder appBuilder)
        {
            // Configure OpenTelemetry tracing & metrics with auto-start using the
            // AddOpenTelemetry extension from OpenTelemetry.Extensions.Hosting.
            appBuilder.Services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder.AddSource("MyApplicationActivitySource");
                    builder.AddHttpClientInstrumentation();
                    builder.AddAspNetCoreInstrumentation();
                    builder.AddOtlpExporter(otlpOptions =>
                            {
                                // Use IConfiguration directly for Otlp exporter endpoint option.
                                otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                            });
                })
                .WithMetrics(builder =>
                {
                    builder.AddMeter("MyApplicationMetrics");
                    builder.AddHttpClientInstrumentation();
                    builder.AddAspNetCoreInstrumentation();
                    builder.AddOtlpExporter(otlpOptions =>
                    {
                        // Use IConfiguration directly for Otlp exporter endpoint option.
                        otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                    });
                });

            // Clear default logging providers used by WebApplication host.
            appBuilder.Logging.ClearProviders();

            // Configure OpenTelemetry Logging.
            appBuilder.Logging.AddOpenTelemetry(options =>
            {
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
