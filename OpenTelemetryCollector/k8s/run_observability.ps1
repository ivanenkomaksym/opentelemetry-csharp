kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.11.1/cert-manager.yaml
Start-Sleep 30
kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml
Start-Sleep 15
kubectl apply -f central_collector.yaml
kubectl patch svc simplest-collector --type='json' -p='[{"op": "replace", "path": "/spec/type", "value": "LoadBalancer"}]'
kubectl patch svc simplest-collector-monitoring --type='json' -p='[{"op": "replace", "path": "/spec/type", "value": "LoadBalancer"}]'
kubectl apply -f prometheus_zipkin.yaml