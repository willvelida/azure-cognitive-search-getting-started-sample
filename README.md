# Getting started with Azure Cognitive Search in C#

Sample showcasing how to deploy an Azure Cognitive Search service using Bicep and how to work with the service in .NET. This sample accompanies my blog post on [Getting started with Azure Cognitive Search in C#](https://www.willvelida.com/posts/getting-started-azure-cognitive-search/).

This sample showcases the following functionality:

- Deploying an Azure Cognitive Search service in Bicep
- Interacting with the Search service in a C# Console application, including:
    - Creating an ```SearchIndexClient``` and a ```SearchIndexerClient``` to interact with both indexes and indexers in Azure Cognitive Search.
    - Creating an index in Azure Cognitive Search.
    - Creating an indexer that pulls data from Azure Blob Storage into a Cognitive Search index.
    - Performing basic queries against Cognitive Search using .NET.

## Required software

To create this sample, I used the following software and SDKs:

- [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Bicep CLI version 0.21.1](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#azure-cli)
- [AZ CLI version 2.52.0](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli)
- [Azure Subscription](https://azure.microsoft.com/en-au/free)
- [Visual Studio Code with the Bicep extension installed.](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#visual-studio-code-and-bicep-extension)

## Deploying an running the sample

To deploy the Azure Cognitive Search resource, navigate to the ```deploy``` folder and run the following commands:

```bash
# Create the resource group
az group create --name <name-of-your-resource-group> --location <azure-region-close-to-you>

# Deploy the Bicep template
az deployment group create --resource-group <name-of-your-resource-group> --template-file .\deploy\main.bicep
```

Once the Bicep template has been deployed successfully, you will need to populate the ```appsettings.json``` file with the following settings to run the sample:

```json
{
  "SearchServiceUri": "",
  "SearchServiceApiKey": "",
  "BlobStorageAccountName": "",
  "BlobStorageConnectionString": ""
}
```

To run the sample, navigate to the ```src/CognitiveSearchDemo``` folder and run the following:

```dotnet
dotnet run
```
