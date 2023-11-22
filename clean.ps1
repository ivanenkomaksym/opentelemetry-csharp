cd .\OpenTelemetryCollector\k8s

kubectl delete -f otel-collector.yaml
kubectl delete -f simplest.yaml
kubectl delete -f namespace.yaml
kubectl delete -f https://github.com/jaegertracing/jaeger-operator/releases/download/v1.36.0/jaeger-operator.yaml
kubectl delete -f https://github.com/cert-manager/cert-manager/releases/download/v1.9.1/cert-manager.yaml

cd ..\..\