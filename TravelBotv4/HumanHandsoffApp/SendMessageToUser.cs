using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Bot.Schema;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using HumanHandsoffApp.Models;

namespace HumanHandsoffApp
{
    public static class SendMessageToUser
    {
        public static CloudTableClient client;
        [FunctionName(nameof(SendMessageToUser))]
        public async static Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            var account = CloudStorageAccount.Parse("<Your Storage Connection String>");
            client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("ConversationInformation");
            await table.CreateIfNotExistsAsync();
            TableOperation getOperation = TableOperation.Retrieve<ConversationInformation>(partitionKey: "ConversationInformation", rowkey: "default-user");
            var info = await table.ExecuteAsync(getOperation);
            var conversationInformation = info.Result as ConversationInformation;
            var conversationReference = JsonConvert.DeserializeObject<ConversationReference>(conversationInformation.ConversationReference);

            //TODO: Need to refactor.
            log.Info("C# HTTP trigger function processed a request.");

            //Convert json as ConversationReference (We assume ConversationReference type json scheme from http post)
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            ConversationReference reference = JsonConvert.DeserializeObject<ConversationReference>(requestBody);
            var name = reference.User.Name;

            //Create Connector Client
            var appCredential = new MicrosoftAppCredentials("<Your MicrosoftAppId which you set in application.json in TravelBot v4>", "<Your MicrosoftAppPassword which you set in application.json in TravelBot v4>");
            MicrosoftAppCredentials.TrustServiceUrl(reference.ServiceUrl);
            var connector = new ConnectorClient(new Uri(reference.ServiceUrl), appCredential);

            //Compose activity
            var proactiveMessage = Activity.CreateMessageActivity();
            proactiveMessage.From = reference.Bot;
            proactiveMessage.Conversation = reference.Conversation;
            proactiveMessage.Text = "proactive message";

            try
            {
                await connector.Conversations.SendToConversationAsync((Activity)proactiveMessage);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            //Return 200 or 400 to web client
            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name} (id:{reference.User.Id})")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}