#!/bin/bash
#Ensures the script exits on error, undefined variables, or pipeline failures.
set -euo pipefail

# Define backup file name with timestamp
BACKUP_FILE="postgres_backup_$(date +%Y%m%d_%H%M%S).sql"

# Get PostgreSQL pod name
POSTGRES_POD=$(kubectl get pods -n postgres -l app.kubernetes.io/name=postgresql -o jsonpath="{.items[0].metadata.name}")

# Retrieve PostgreSQL password from Kubernetes secret
POSTGRES_SECRET=$(kubectl -n postgres get secret postgres-postgresql -o jsonpath="{.data.postgres-password}" | base64 --decode)

# Perform backup
echo "Backing up PostgreSQL database..."
kubectl exec -it "$POSTGRES_POD" -n postgres -- bash -c "PGPASSWORD=postgres pg_dumpall -U postgres" > "$BACKUP_FILE"

echo "Backup completed: $BACKUP_FILE"

# Stop existing proxy and clean up
pkill -f "kubectl proxy"
rm -f proxy.log

# Delete all Deployments, StatefulSets, DaemonSets, and ReplicaSets
# Delete workloads in order
kubectl delete deployments --all --all-namespaces --grace-period=0 --force
kubectl delete statefulsets --all --all-namespaces --grace-period=0 --force
kubectl delete daemonsets --all --all-namespaces --grace-period=0 --force
kubectl delete replicasets --all --all-namespaces --grace-period=0 --force

# Then delete all pods
kubectl delete pods --all --all-namespaces --grace-period=0 --force
echo "Pods are deleted"

# Proceed with cluster deletion
kind delete clusters --all
echo "Clusters are deleted"

# Create a new cluster with the specified configuration
kind create cluster --config kind-config.yaml

#install k8s dashboard (proxy) 
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml


# Start kubectl proxy in the background
nohup kubectl proxy --port=8001 > proxy.log 2>&1 &

# Wait for initialization
sleep 5

# Add Bitnami repository if not already added
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

# Install RabbitMQ
helm install rabbitmq bitnami/rabbitmq -f k8s_rabbit_custom-values.yaml --namespace rabbitmq --create-namespace

# Install PostgreSQL
helm install postgres bitnami/postgresql -f k8s_postgres_values.yaml --namespace postgres --create-namespace

#Load images

kind load docker-image orderapi:latest 
kind load docker-image warehouseapi:latest 

# Apply all manifests in the specified directory
kubectl apply -f k8s_kind_local_manifests/

# Wait for PostgreSQL pod to be ready
echo "Waiting for PostgreSQL pod to be ready..."
kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=postgresql -n postgres --timeout=120s

# Get the PostgreSQL pod name
POD_NAME=$(kubectl get pods -n postgres -l app.kubernetes.io/name=postgresql -o jsonpath="{.items[0].metadata.name}")

# Retrieve PostgreSQL password from Kubernetes secret
POSTGRES_PASSWORD=$(kubectl get secret --namespace postgres postgres-postgresql -o jsonpath="{.data.postgres-password}" | base64 --decode)

# Restore PostgreSQL data
kubectl cp "$BACKUP_FILE" -n postgres "$POD_NAME":/tmp/"$BACKUP_FILE"
kubectl exec -n postgres -it $POD_NAME -- bash -c "PGPASSWORD=$POSTGRES_PASSWORD psql -U postgres -h 127.0.0.1 -f /tmp/$BACKUP_FILE"

