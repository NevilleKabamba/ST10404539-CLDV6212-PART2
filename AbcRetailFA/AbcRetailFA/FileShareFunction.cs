using Azure;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

public class FileShareFunction
{
    private readonly ShareServiceClient _shareServiceClient;

    public FileShareFunction(IConfiguration configuration)
    {
        _shareServiceClient = new ShareServiceClient(configuration["AzureWebJobsStorage"]);
    }

    // Use HTTP trigger to upload a file to the "contracts-logs" file share
    [Function("FileShareFunction")]
    public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "uploadfile")] HttpRequestData req, FunctionContext context)
    {
        var logger = context.GetLogger("FileShareFunction");

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
}
