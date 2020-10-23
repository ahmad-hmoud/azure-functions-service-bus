using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ServiceBus
{
    public static class Functions
    {
        [FunctionName("FunctionOne")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello from slot, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
        
        // myQueueItem can be of type string, in this case it will contain the message content
        [FunctionName("ServiceBusTrigger")]
        public static void Run([ServiceBusTrigger("%queueName%", Connection = "sbConnection")]
        Message myQueueItem,
        int deliveryCount,
        DateTime enqueuedTimeUtc,
        string messageId,
        ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {Encoding.UTF8.GetString(myQueueItem.Body)}");
            log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
            log.LogInformation($"DeliveryCount={deliveryCount}");
            log.LogInformation($"MessageId={messageId}");
            log.LogInformation($"User Properties");
            foreach (var property in myQueueItem.UserProperties)
            {
                log.LogInformation($"key'{property.Key}' with value '{property.Value}'");
            }
        }

        [FunctionName("ServiceBusSender")]
        // Connection must be an app setting with Service Bus connection string
        // %queueName% must be an app setting with Service Bus queue name string
        // Removing the % Functions runtime will be looking for a queue name queueName
        // i.e. look for the literal name
        [return: ServiceBus("%queueName%", Connection = "sbConnection")]
        public static string ServiceBusOutput([HttpTrigger] dynamic input, ILogger log)
        {
            return $"Message send to Service Bus Queue at {DateTime.Now}";
        }
    }
}
