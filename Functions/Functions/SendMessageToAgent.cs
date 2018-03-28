using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Functions.Models;
using Functions.Services;
using Newtonsoft.Json;

namespace Functions
{
    public static class SendMessageToAgent
    {
        [FunctionName("SendMessageToAgent")]
        public static async Task Run(
            [QueueTrigger("message-from-user", Connection = "HumanHandsoffStorage")]string myQueueItem, 
            [Table(tableName:"ChatPlusInformation", partitionKey: "ChatPlusInformation", Connection = "HumanHandsoffStorage")]ChatPlusInformation[] chatPlusInformationList,
            TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
            var conversationInformation = JsonConvert.DeserializeObject<ConversationInformation>(myQueueItem);
            var chatPlusInformation = chatPlusInformationList.OrderByDescending(c => c.timestamp).First();
            await ChatPlus.SendMessage(chatPlusInformation, conversationInformation);
        }
    }
}
