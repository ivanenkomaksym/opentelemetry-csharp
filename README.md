# opentelemetry-csharp

## Dependencies
[Docker Desktop](https://docs.docker.com/desktop/install/windows-install/)
[.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Run locally

1. Execute `run_local_otelcol-contrib.ps1` with PowerShell
2. Execute `dotnet run` inside `WeatherForecast` directory
3. Open WetherForecast Swagger and execute requests

```
https://localhost:7175/swagger/index.html
```

4. Observe logs in `otelcol-contrib_0.128.0_windows_amd64\otel.json`

## Docker Usage

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

## K8s usage

Standalone central (Gateway) collector is used in this case. Telemetry data is then received from agents and processed, and exported to a storage backend.
OpenTelemetry operator is an implementation of a Kubernetes Operator and manages the OpenTelemetry collector and the auto-instrumentation of the workloads using OpenTelemetry instrumentation libraries.

The idea of this test is that all necessary observability deployments are up and runing in k8s cluster, while WebApi application is running locally and sends telemetry data to `http://localhost:4317`. We can then connect locally to observe it from Zipkin and Prometheus.

```
D:\repos\opentelemetry-csharp\OpenTelemetryCollector\k8s>kubectl get services -n opentelemetry-operator-system
NAME                                                        TYPE        CLUSTER-IP      EXTERNAL-IP   PORT(S)    AGE
opentelemetry-operator-controller-manager-metrics-service   ClusterIP   10.96.55.151    <none>        8443/TCP   33m
opentelemetry-operator-webhook-service                      ClusterIP   10.108.138.44   <none>        443/TCP    33m

D:\repos\opentelemetry-csharp\OpenTelemetryCollector\k8s>kubectl get services
NAME                            TYPE           CLUSTER-IP      EXTERNAL-IP   PORT(S)                                        AGE
kubernetes                      ClusterIP      10.96.0.1       <none>        443/TCP                                        7d
prometheus-service              LoadBalancer   10.99.183.226   127.0.0.1     9090:32183/TCP                                 33m
simplest-collector              LoadBalancer   10.104.31.204   127.0.0.1     4317:31644/TCP,4318:31001/TCP,8889:30608/TCP   33m
simplest-collector-headless     ClusterIP      None            <none>        4317/TCP,4318/TCP,8889/TCP                     33m
simplest-collector-monitoring   LoadBalancer   10.110.251.49   127.0.0.1     8888:30849/TCP                                 33m
zipkin-service                  LoadBalancer   10.111.73.188   127.0.0.1     9411:30034/TCP                                 33m
```

1. Start observability deployments (OpenTelementry, Zipkin and Prometheus)

```powershell
cd \OpenTelemetryCollector\k8s
run_observability.ps1
```

2. Connect to LoadBalancer services

```powershell
minikube tunnel
```

3. Start WeatherForecast app:

```bash
cd WeatherForecast
dotnet run
```

4. Open WetherForecast Swagger and execute requests

```
https://localhost:7175/swagger/index.html
```

5. Observe in Zipkin

```
http://127.0.0.1:9411/
```

![image](https://github.com/ivanenkomaksym/opentelemetry-csharp/assets/5527051/f61ff925-40b3-4c93-ad1f-db5f758cff8a)

6. Observe in Prometheus

```
http://127.0.0.1:9090/graph?g0.expr=weather_days_freezing_The_number_of_days_where_the_temperature_is_below_freezing_total&g0.tab=0&g0.stacked=0&g0.show_exemplars=0&g0.range_input=1h
```

![image](https://github.com/ivanenkomaksym/opentelemetry-csharp/assets/5527051/29a06c94-e638-46b2-8e5e-b3de37f6e9a7)

## References
[OpenTelemetry Operator](https://medium.com/@magstherdev/opentelemetry-operator-d3d407354cbf)
