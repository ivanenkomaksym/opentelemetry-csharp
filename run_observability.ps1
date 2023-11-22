cd .\OpenTelemetryCollector\k8s

kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.9.1/cert-manager.yaml
kubectl apply -f namespace.yaml
kubectl create -f https://github.com/jaegertracing/jaeger-operator/releases/download/v1.36.0/jaeger-operator.yaml
kubectl apply -f simplest.yaml
kubectl patch svc simplest-query --type='json' -p='[{"op": "replace", "path": "/spec/type", "value": "LoadBalancer"}]'
kubectl apply -f otel-collector.yaml
minikube tunnel