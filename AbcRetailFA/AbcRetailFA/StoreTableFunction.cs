using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Azure;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Worker.Http;

public class StoreTableFunction
{
    private readonly TableServiceClient _tableServiceClient;

    public StoreTableFunction(IConfiguration configuration)
    {
        _tableServiceClient = new TableServiceClient(configuration["AzureWebJobsStorage"]);
    }

    // Use HTTP trigger 
    [Function("StoreTableFunction")]
    public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "storetable")] HttpRequestData req, FunctionContext context)
    {
        var logger = context.GetLogger("StoreTableFunction");

        // Assume customer information is coming from the HTTP request body
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
}
