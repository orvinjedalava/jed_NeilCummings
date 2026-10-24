-- Create an Azure identity for GitHub --

Azure CLI command:
az ad app create --display-name github-deploy

Creates an app registration in Microsoft Entra ID (formerly Azure AD) named github-deploy. This is the identity GitHub will act as. The output includes an appId, which you use in the next commands.

Azure CLI command:
az ad sp create --id <appId>

Creates a service principal, which is the app's presence in your tenant. Azure role assignments apply to service principals, so this step is needed before you can grant access.

Azure CLI command:
az role assignment create --assignee <appId> --role Contributor --scope /subscriptions/<subId>/resourceGroups/<rgName>

Grants the identity the Contributor role, which lets it create and change resources, but only inside the named resource group. The --scope limits the access. You need Owner or User Access Administrator rights on the resource group to run this.


-- Trust your repo ( federated credential ) --

Azure CLI command:
az ad app federated-credential create --id <appId> --parameters '{
  "name": "github-main",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:<owner>/<repo>:ref:refs/heads/main",
  "audiences": ["api://AzureADTokenExchange"]
}'

This command tells Azure to trust login requests from your GitHub repo, so the workflow can log in without a stored secret.

* --id <appId> selects the app registration from the first step.
* name is a label for this trust rule, here github-main.
* issuer is GitHub's token service, https://token.actions.githubusercontent.com. Azure accepts tokens signed by this issuer.
* subject identifies which GitHub runs are trusted. repo:<owner>/<repo>:ref:refs/heads/main means only runs from the main branch of that repo. A run from any other branch will fail to log in.
* audiences is the value the token must be issued for. api://AzureADTokenExchange is the standard value for Azure's token exchange, so you should keep it as written.
If you later use a GitHub environment such as prod, the subject changes to repo:<owner>/<repo>:environment:prod, and you would add a separate federated credential for it.



-- Add three GitHub secrets ( Settings > Secrets and variables > Actions )

AZURE_CLIENT_ID is the app ID
AZURE_TENANT_ID is your tenant ID
AZURE_SUBSCRIPTION_ID is your subscription ID


-- Add the login step to manual-deploy.yaml

- uses: azure/login@v2
  with:
    client-id: ${{ secrets.AZURE_CLIENT_ID }}
    tenant-id: ${{ secrets.AZURE_TENANT_ID }}
    subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}

 
