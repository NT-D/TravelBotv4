using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Schema;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using HumanHandoffApp.Models;
using HumanHandoffApp.Services;

namespace HumanHandoffApp
{
    public static class SendConnectionRequest
    {
        [FunctionName("SendConnectionRequest")]
        public static async Task Run([QueueTrigger("connection-request", Connection = "HumanHandsoffStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

            // TODO hiroaki-honda Implement logic to send connection request to ChatPlus
            var conversationInformation = JsonConvert.DeserializeObject<ConversationInformation>(myQueueItem);
            var result = await ChatPlus.SendConnectionRequest(conversationInformation);
        }
    }
}
