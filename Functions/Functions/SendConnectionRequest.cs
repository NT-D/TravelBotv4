using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Schema;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Functions.Models;

namespace Functions
{
    public static class SendConnectionRequest
    {
        [FunctionName("SendConnectionRequest")]
        public static void Run([QueueTrigger("connection-request", Connection = "HumanHandsoffStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

            // TODO hiroaki-honda Implement logic to send connection request to ChatPlus
            var data = JsonConvert.DeserializeObject<ConversationInformation>(myQueueItem);
        }
    }
}
