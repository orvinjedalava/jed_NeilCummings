# Provision a Container Registry
az acr create --resource-group <rgName> --name <acrName> --sku Basic

# Provision a Container Environment and App
az extension add --name containerapp --upgrade
az provider register --namespace Microsoft.App --wait
az provider register --namespace Microsoft.OperationalInsights --wait

# Create the environment that hosts the app
az containerapp env create \
  --name <envName> \
  --resource-group <rgName> \
  --location <location>

# Create the app with a placeholder image; the workflow replaces it on deploy
az containerapp create \
  --name next-match-container-app \
  --resource-group <rgName> \
  --environment <envName> \
  --image mcr.microsoft.com/k8se/quickstart:latest \
  --target-port 3000 \
  --ingress external

# Provision Container App Env and Container App
# 1. Create the environment in Australia East
az containerapp env create \
  --name <envName> \
  --resource-group <rgName> \
  --location australiaeast

# 2. Create the app on the placeholder image, listening on port 3000
az containerapp create \
  --name <appName> \
  --resource-group <rgName> \
  --environment <envName> \
  --image mcr.microsoft.com/k8se/quickstart:latest \
  --target-port 3000 \
  --ingress external

# 3. Give the app permission to pull from your registry

RG="<your-resource-group>"
APP="<your-container-app-name>"
ACR="<your-acr-name>"

# Enable system-assigned managed identity on the Container App
az containerapp identity assign \
  --name "$APP" \
  --resource-group "$RG" \
  --system-assigned

# Get principal ID of that identity
PRINCIPAL_ID=$(az containerapp identity show \
  --name "$APP" \
  --resource-group "$RG" \
  --query principalId -o tsv)

# Get ACR resource ID
ACR_ID=$(az acr show \
  --name "$ACR" \
  --query id -o tsv)

# Grant AcrPull to the Container App identity
az role assignment create \
  --assignee-object-id "$PRINCIPAL_ID" \
  --assignee-principal-type ServicePrincipal \
  --role AcrPull \
  --scope "$ACR_ID"

# Tell the app to use managed identity for registry pulls
LOGIN_SERVER=$(az acr show --name "$ACR" --query loginServer -o tsv)

az containerapp registry set \
  --name "$APP" \
  --resource-group "$RG" \
  --server "$LOGIN_SERVER" \
  --identity system
 
