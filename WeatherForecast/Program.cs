using System.Diagnostics.Metrics;
using System.Diagnostics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry;
using OpenTelemetry.Logs;

// This is required if the collector doesn't expose an https endpoint. By default, .NET
// only allows http2 (required for gRPC) to secure endpoints.
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

// Configure metrics
builder.Services.AddOpenTelemetryMetrics(builder =>
{
    builder.AddHttpClientInstrumentation();
    builder.AddAspNetCoreInstrumentation();
    builder.AddMeter("MyApplicationMetrics");
    builder.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
});


// Configure tracing
builder.Services.AddOpenTelemetryTracing(builder =>
{
    builder.AddHttpClientInstrumentation();
    builder.AddAspNetCoreInstrumentation();
    builder.AddSource("MyApplicationActivitySource");
    builder.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
});

// Configure logging
builder.Logging.AddOpenTelemetry(builder =>
{
    builder.IncludeFormattedMessage = true;
    builder.IncludeScopes = true;
    builder.ParseStateValues = true;
    builder.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
var activitySource = new ActivitySource("SampleActivitySource");

var meter = new Meter("MyApplication");

var counter = meter.CreateCounter<int>("Requests");
var histogram = meter.CreateHistogram<float>("RequestDuration", unit: "ms");
meter.CreateObservableGauge("ThreadCount", () => new[] { new Measurement<int>(ThreadPool.ThreadCount) });

var httpClient = new HttpClient();

app.MapGet("/", async (ILogger<Program> logger, string name) =>
{
    logger.LogInformation("Hello {Name}! It is {Time}", name, DateTime.UtcNow);

    // Measure the number of requests
    counter.Add(1, KeyValuePair.Create<string, object?>("name", name));

    var stopwatch = Stopwatch.StartNew();
    await httpClient.GetStringAsync("https://www.meziantou.net");

    // Measure the duration in ms of requests and includes the host in the tags
    histogram.Record(stopwatch.ElapsedMilliseconds,
        tag: KeyValuePair.Create<string, object?>("Host", "www.meziantou.net"));

    // The sampleActivity is automatically linked to the parent activity (the one from
    // ASP.NET Core in this case).
    // You can get the current activity using Activity.Current.
    using (var sampleActivity = activitySource.StartActivity("Sample", ActivityKind.Server))
    {
        // note that "sampleActivity" can be null here if nobody listen events generated
        // by the "SampleActivitySource" activity source.
        sampleActivity?.AddTag("Name", name);
        sampleActivity?.AddBaggage("SampleContext", name);

        // Simulate a long running operation
        await Task.Delay(1000);
    }

    return Results.Ok($"Hello {name}");
});
app.Run();
