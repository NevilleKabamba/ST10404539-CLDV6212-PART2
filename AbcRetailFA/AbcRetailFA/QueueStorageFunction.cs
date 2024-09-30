using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

public class QueueStorageFunction
{
    private readonly QueueServiceClient _queueServiceClient;

    public QueueStorageFunction(IConfiguration configuration)
    {
        _queueServiceClient = new QueueServiceClient(configuration["AzureWebJobsStorage"]);
    }

    // Use QueueTrigger to trigger on messages in the "order-processing" queue
    [Function("QueueStorageFunction")]
    public async Task Run([QueueTrigger("order-processing")] byte[] messageBytes, FunctionContext context)
    {
        var logger = context.GetLogger("QueueStorageFunction");

        // Decoding the message as a UTF-8 string
        string message;
        try
        {
            message = Encoding.UTF8.GetString(messageBytes);
            logger.LogInformation($"Message received from 'order-processing' queue: {message}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to decode message as UTF-8: {ex.Message}. Trying Base64 format...");

            // If UTF-8 decoding fails, log the Base64 version of the message
            message = Convert.ToBase64String(messageBytes);
            logger.LogInformation($"Message received from 'order-processing' queue (Base64): {message}");
        }

        // Construct a return message to confirm successful processing
        string returnMessage = $"Processing Order: {message}";

        // Log the return message
        logger.LogInformation(returnMessage);
    }
}


