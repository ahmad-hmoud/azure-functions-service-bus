using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;

namespace ServiceBus
{
    public static class Functions
    {
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
