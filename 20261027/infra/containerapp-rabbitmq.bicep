param location string = resourceGroup().location
param envName string
param appName string
param image string = 'rabbitmq:4'
param rabbitUser string
@secure()
param rabbitPassword string // 'guest' is only allowed from localhost, so use a real user

resource env 'Microsoft.App/managedEnvironments@2024-03-01' existing = {
  name: envName
}

resource app 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      // AMQP is raw TCP; the default HTTP ingress would not carry it
      ingress: { external: false, transport: 'tcp', targetPort: 5672, exposedPort: 5672 }
      secrets: [ { name: 'rabbit-pass', value: rabbitPassword } ]
    }
    template: {
      containers: [
        {
          name: appName
          image: image
          env: [
            { name: 'RABBITMQ_DEFAULT_USER', value: rabbitUser }
            { name: 'RABBITMQ_DEFAULT_PASS', secretRef: 'rabbit-pass' }
          ]
          resources: { cpu: json('0.5'), memory: '1Gi' }
        }
      ]
      // Keep 1: scale-to-zero drops the broker; max 1: no clustering configured
      scale: { minReplicas: 1, maxReplicas: 1 }
    }
  }
}
