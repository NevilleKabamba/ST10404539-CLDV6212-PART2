# ABC RETAIL FUNCTIONS APP

## Overview
This  C# project demonstrates how to use Azure Functions with various Azure Storage services. The functions handle the following services:

- **Blob Storage**: Triggered when a new blob is added to a container.
- **File Storage**: Uploads a file to an Azure File Share via HTTP trigger.
- **Queue Storage**: Processes messages from an Azure Queue.
- **Table Storage**: Adds an entity to an Azure Table via HTTP trigger.

## Project Structure
- **BlobStorageFunction.cs**: A function triggered by blob creation in Azure Blob Storage.
- **FileShareFunction.cs**: A function that uploads a file to an Azure File Share using HTTP trigger.
- **QueueStorageFunction.cs**: A function that processes messages from an Azure Queue.
- **StoreTableFunction.cs**: A function that adds entities to an Azure Table using HTTP trigger.
- **Program.cs**: Configures dependency injection for Azure Storage services.

## Prerequisites
- .NET Core SDK (v6.0 or later).
- Azure Functions Core Tools.
- An Azure Storage account.

## Getting Started

### 1. Clone the Repository
Clone this repository to your local machine:
```bash
git clone <repository_url>
cd <repository_directory>
```

### 2. Configure Azure Storage Connection String
Update the `local.settings.json` file (for local development) or Azure Configuration (for production) to include your Azure Storage connection string:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<Your_Azure_Storage_Connection_String>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

### 3. Install Dependencies
Run the following command to restore the necessary packages:
```bash
dotnet restore
```

### 4. Run Locally
Use Azure Functions Core Tools to run the project locally:
```bash
func start
```

## Functions Overview

### 1. BlobStorageFunction
This function triggers when a new blob is uploaded to the `product-images` container in Azure Blob Storage.

**Trigger**:
- Type: `BlobTrigger`
- Container: `product-images`

```csharp
public async Task Run([BlobTrigger("product-images/{name}")] BlobClient blobClient, string name, FunctionContext context)
{
    var logger = context.GetLogger("BlobStorageFunction");
    logger.LogInformation($"Blob successfully uploaded in 'product-images': {name}");
}
```

### 2. FileShareFunction
This function allows the upload of a file to the `contracts-logs` file share using an HTTP POST request.

**Trigger**:
- Type: `HttpTrigger`
- HTTP Verb: `POST`
- Route: `/uploadfile`

```csharp
public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "uploadfile")] HttpRequestData req, FunctionContext context)
{
    var shareClient = _shareServiceClient.GetShareClient("contracts-logs");
    await shareClient.CreateIfNotExistsAsync();

    var directoryClient = shareClient.GetRootDirectoryClient();
    var fileClient = directoryClient.GetFileClient("uploadedfile.txt");

    using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Sample file content")))
    {
        await fileClient.CreateAsync(stream.Length);
        await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
    }

    logger.LogInformation("File uploaded to Azure File Share 'contracts-logs'.");
}
```

### 3. QueueStorageFunction
This function processes messages in the `order-processing` queue. It handles both UTF-8 and Base64 encoded messages.

**Trigger**:
- Type: `QueueTrigger`
- Queue Name: `order-processing`

```csharp
public async Task Run([QueueTrigger("order-processing")] byte[] messageBytes, FunctionContext context)
{
    var logger = context.GetLogger("QueueStorageFunction");
    string message;

    try
    {
        message = Encoding.UTF8.GetString(messageBytes);
        logger.LogInformation($"Message received: {message}");
    }
    catch (Exception ex)
    {
        message = Convert.ToBase64String(messageBytes);
        logger.LogError($"Failed to decode message: {ex.Message}. Logged as Base64: {message}");
    }

    logger.LogInformation($"Processing Order: {message}");
}
```

### 4. StoreTableFunction
This function adds an entity (e.g., a customer profile) to the `CustomerProfiles` Azure Table. The HTTP request contains customer details.

**Trigger**:
- Type: `HttpTrigger`
- HTTP Verb: `POST`
- Route: `/storetable`

```csharp
public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "storetable")] HttpRequestData req, FunctionContext context)
{
    var logger = context.GetLogger("StoreTableFunction");
    var tableClient = _tableServiceClient.GetTableClient("CustomerProfiles");

    await tableClient.CreateIfNotExistsAsync();

    var entity = new TableEntity("PartitionKey", Guid.NewGuid().ToString())
    {
        { "FirstName", "John" },
        { "LastName", "Doe" },
        { "Email", "john.doe@example.com" }
    };

    await tableClient.AddEntityAsync(entity);
    logger.LogInformation($"Entity {entity["FirstName"]} added to table.");
}
```

## Dependency Injection Setup
In the `Program.cs`, the following Azure Storage services are registered with dependency injection:
- BlobServiceClient
- TableServiceClient
- QueueServiceClient
- ShareServiceClient

## Deployment

### 1. Create an Azure Function App
Use the following command to create an Azure Function App:
```bash
az functionapp create --resource-group <ResourceGroupName> --consumption-plan-location <Location> --runtime dotnet --functions-version 4 --name <FunctionAppName> --storage-account <StorageAccountName>
```

### 2. Deploy the Functions
Deploy the functions using:
```bash
func azure functionapp publish <FunctionAppName>
```

Citations:
[1] https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-trigger?pivots=programming-language-python&tabs=python-v2%2Cisolated-process%2Cnodejs-v4
[2] https://learn.microsoft.com/en-us/azure/azure-functions/functions-create-storage-blob-triggered-function
[3] https://learn.microsoft.com/en-us/azure/azure-functions/functions-event-grid-blob-trigger