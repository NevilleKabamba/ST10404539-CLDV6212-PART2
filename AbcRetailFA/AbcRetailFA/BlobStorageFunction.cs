using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class BlobStorageFunction
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageFunction(IConfiguration configuration)
    {
        _blobServiceClient = new BlobServiceClient(configuration["AzureWebJobsStorage"]);
    }

    // Use BlobTrigger to automatically trigger on blob creation in the "product-images" container
    [Function("BlobStorageFunction")]
    public async Task Run([BlobTrigger("product-images/{name}")] BlobClient blobClient, string name, FunctionContext context)
    {
        var logger = context.GetLogger("BlobStorageFunction");

        // Log file name when a new blob is created in the "product-images" container
        logger.LogInformation($"Blob succesfully uploaded in 'product-images': {name}");

        
    }
}
