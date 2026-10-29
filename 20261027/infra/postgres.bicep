param location string = resourceGroup().location // inherits the region of the target resource group
param serverName string // must be globally unique; becomes <serverName>.postgres.database.azure.com
param adminUser string
@secure() // keeps the value out of deployment logs and history
param adminPassword string

resource server 'Microsoft.DBforPostgreSQL/flexibleServers@2024-08-01' = {
  name: serverName
  location: location
  sku: { name: 'Standard_B1ms', tier: 'Burstable' } // cheapest compute option
  properties: {
    version: '16'
    administratorLogin: adminUser // cannot be changed after the server is created
    administratorLoginPassword: adminPassword // re-applied on every deployment
    storage: { storageSizeGB: 32 } // minimum size
    network: { publicNetworkAccess: 'Enabled' } // avoids private networking cost; secured by firewall rules
  }
}

resource db 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2024-08-01' = {
  parent: server
  name: 'auctions' // database the AuctionService connection string will target
}

// 0.0.0.0 - 0.0.0.0 is the special range that allows Azure services
resource allowAzure 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2024-08-01' = {
  parent: server
  name: 'AllowAzureServices'
  dependsOn: [ db ]
  properties: { startIpAddress: '0.0.0.0', endIpAddress: '0.0.0.0' } // lets any Azure service connect, including Container Apps
}

param myIp string = '' // your public IP; leave empty to skip the rule
resource allowMyIp 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2024-08-01' = if (!empty(myIp)) {
  parent: server
  name: 'AllowMyIp'
  dependsOn: [ allowAzure ]
  properties: { startIpAddress: myIp, endIpAddress: myIp }
}
