using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Storage.Blobs;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = new HostBuilder()
    .ConfigureFunctionsWebApplication() // Configures the Azure Functions worker in isolated mode
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // Register Azure Storage services with dependency injection

        // BlobServiceClient for Azure Blob Storage
        services.AddSingleton(new BlobServiceClient(configuration["AzureWebJobsStorage"]));

        // TableServiceClient for Azure Table Storage
        services.AddSingleton(new TableServiceClient(configuration["AzureWebJobsStorage"]));

        // QueueServiceClient for Azure Queue Storage
        services.AddSingleton(new QueueServiceClient(configuration["AzureWebJobsStorage"]));

        // ShareServiceClient for Azure File Storage
        services.AddSingleton(new ShareServiceClient(configuration["AzureWebJobsStorage"]));
    })
    .Build();

        host.Run(); // Starts the function host
    }
}