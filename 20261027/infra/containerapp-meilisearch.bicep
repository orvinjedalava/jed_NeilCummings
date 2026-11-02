param location string = resourceGroup().location
param envName string
param appName string
param image string = 'getmeili/meilisearch:v1.12' // pin a version; check the latest tag
@secure()
param masterKey string // must be at least 16 bytes in production mode

resource env 'Microsoft.App/managedEnvironments@2024-03-01' existing = {
  name: envName
}

resource app 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      ingress: { external: false, targetPort: 7700 } // only reachable inside the environment
      secrets: [ { name: 'meili-key', value: masterKey } ]
    }
    template: {
      containers: [
        {
          name: appName
          image: image
          env: [
            { name: 'MEILI_ENV', value: 'production' }
            { name: 'MEILI_MASTER_KEY', secretRef: 'meili-key' }
            { name: 'MEILI_NO_ANALYTICS', value: 'true' }
          ]
          resources: { cpu: json('0.25'), memory: '0.5Gi' } // 0.25 vCPU allows up to 0.5Gi; bump to 0.5/1Gi if it gets OOM-killed
        }
      ]
      // Keep 1: scaling to zero would wipe the ephemeral index. Max 1: community edition can't cluster.
      scale: { minReplicas: 1, maxReplicas: 1 }
    }
  }
}
