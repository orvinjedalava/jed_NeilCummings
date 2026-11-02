param location string = resourceGroup().location // inherits the region of the target resource group
param acrName string // existing registry in the same resource group
param envName string // existing environment; provision it first with the env workflow
param appName string
param image string = 'mcr.microsoft.com/dotnet/samples:aspnetapp' // placeholder that listens on 8080 like AuctionService
param targetPort int = 8080 // must match ASPNETCORE_HTTP_PORTS in the Dockerfile

param externalIngress bool = true
param minReplicas int = 0 // scale to zero when idle
param maxReplicas int = 1 // caps cost; default value is 10

// 0.25 vCPU pairs with 0.5Gi
param cpu string = '0.25'
param memory string = '0.5Gi'

var acrPullRoleId = '7f951dda-4ed3-4680-a7ca-43fe172d538d' // built-in AcrPull role

// 'existing' references a resource without creating or changing it
resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: acrName
}

resource env 'Microsoft.App/managedEnvironments@2024-03-01' existing = {
  name: envName
}

// Standalone identity so AcrPull can be granted before the app exists
resource pullIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${appName}-pull'
  location: location
}

// Needs "Role Based Access Control Administrator" on the resource group for the deploying identity
resource acrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: acr // grants the role on the registry, not the resource group
  name: guid(acr.id, pullIdentity.id, acrPullRoleId) // role assignment names must be GUIDs; deterministic so re-runs are idempotent
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', acrPullRoleId)
    principalId: pullIdentity.properties.principalId
    principalType: 'ServicePrincipal' // avoids replication delay errors for newly created identities
  }
}

resource app 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  dependsOn: [ acrPull ] // role must exist before the registry config is validated
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: { '${pullIdentity.id}': {} } // empty object is required; the resource ID is the key
  }
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      ingress: { external: externalIngress, targetPort: targetPort } // external = reachable from the internet
      registries: [
        { server: acr.properties.loginServer, identity: pullIdentity.id } // pull with the managed identity, no password
      ]
    }
    template: {
      containers: [
        { 
          name: appName
          image: image 
          resources: { cpu: json(cpu), memory: memory } // json() is required because Bicep has no float literal
        } // re-running this template resets the image to the placeholder
      ]
      scale: { minReplicas: minReplicas, maxReplicas: maxReplicas } // HTTP scale rules is added automatically
    }
  }
}
