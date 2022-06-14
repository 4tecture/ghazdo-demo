@description('Base name of the resource such as web app name and app service plan ')
@minLength(2)
param webAppName string = 'secretsdemo'

@description('The SKU of App Service Plan ')
param sku string = 'F1'

@description('The Runtime stack of current web app')
param linuxFxVersion string = 'DOTNETCORE|6.0'

@description('Location for all resources.')
param location string = resourceGroup().location

var webAppPortalName_var = '${webAppName}-webapp'
var appServicePlanName_var = 'AppServicePlan-${webAppName}'

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: appServicePlanName_var
  location: location
  sku: {
    name: sku
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2020-06-01' = {
  name: webAppPortalName_var
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: linuxFxVersion
      webSocketsEnabled: true
    }
  }
}

resource slotStaging 'Microsoft.Web/sites/slots@2021-03-01' = {
  parent: webApp
  name: 'staging'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
  }
}

resource slotTesting 'Microsoft.Web/sites/slots@2021-03-01' = {
  parent: webApp
  name: 'testing'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
  }
}

