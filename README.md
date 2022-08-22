# opentelemetry-csharp

## Dependencies
[Docker Desktop](https://docs.docker.com/desktop/install/windows-install/)
[.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Usage

1. Start OpenTelemetry docker

```bash
cd \OpenTelemetryCollector
docker-compose up
```

2. Start WeatherForecast app:

```bash
cd WeatherForecast
dotnet run
```

3. Open WetherForecast Swagger and execute requests

```
https://localhost:7175/swagger/index.html
```

4. Observe logs.json in OpenTelemetryCollector\output

5. Observe in Zipkin

```
http://localhost:9411/
```

![image](https://user-images.githubusercontent.com/5527051/185874201-c53a20d3-7866-4529-a1b1-e9043bb46d21.png)

6. Observe in Prometheus

```
http://localhost:9090/graph?g0.expr=http_server_duration_bucket&g0.tab=0&g0.stacked=0&g0.show_exemplars=0&g0.range_input=5m
```

![image](https://user-images.githubusercontent.com/5527051/185874342-44dfdc12-9673-4273-a957-f33938389d74.png)
