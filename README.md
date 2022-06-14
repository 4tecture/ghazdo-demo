# GitHub Actions Demo

## Setup

Perform the following action to setup resource groups and secrets fo the GitHub Actions Workflows

- Create a resource group

  ```
  az group create -n demo-devsecops -l westeurope
  ```

- Create a service principal

  ```
  az ad sp create-for-rbac --name devsecops-demo --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/demo-devsecops --sdk-auth
  ```

- Create secrets in GitHub repository
  - AZURE_CREDENTIALS --> paste the entire json from the command above
  - AZURE_RG --> resource group name
  - AZURE_SUBSCRIPTION --> subscription id