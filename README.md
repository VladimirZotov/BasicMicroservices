#Microservices
##Building project
```
cd D:\Sources\Microservices
docker build -t vazotov/platformservice . -f .\PlatformService\Dockerfile
docker push  vazotov/platformservice
```
##Running project
##Restract deployment
```
kubectl rollout restart deployment platforms-depl
```
##Debug commands
```
kubectl get deployments
kubectl get pods
kubectl get services
minikube service list
kubectl get ingress
kubectl get storageclass
```