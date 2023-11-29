kubectl delete -f prometheus_zipkin.yaml
kubectl delete -f central_collector.yaml
kubectl delete -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml
kubectl delete -f https://github.com/cert-manager/cert-manager/releases/download/v1.11.1/cert-manager.yaml