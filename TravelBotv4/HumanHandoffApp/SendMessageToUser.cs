using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Bot.Schema;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using HumanHandoffApp.Models;
using Microsoft.AspNetCore;
using System.Net;
using Microsoft.Extensions.Primitives;

namespace HumanHandsoffApp
{
    public static class SendMessageToUser
    {
        public static CloudTableClient client;
        [FunctionName(nameof(SendMessageToUser))]
        public async static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req,
            TraceWriter log)
        {
            var account = CloudStorageAccount.Parse("<Please insert your storage account key>");
            client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("ConversationInformation");
            await table.CreateIfNotExistsAsync();
            TableOperation getOperation = TableOperation.Retrieve<ConversationInformation>(partitionKey: "ConversationInformation", rowkey: "default-user");
            var info = await table.ExecuteAsync(getOperation);
            var conversationInformation = info.Result as ConversationInformation;
            var reference = JsonConvert.DeserializeObject<ConversationReference>(conversationInformation.ConversationReference);

            //TODO: Need to refactor.
            log.Info("C# HTTP trigger function processed a request.");

            //Parse body from ChatPlus' webhook
            //string requestBody = new StreamReader(req.Body).ReadToEnd();
            //var data = body["data"];
            //var res = JsonConvert.DeserializeObject<WebhookSendMessageResponse>(data);

            var body = await req.ReadFormAsync();
            body.TryGetValue("data", out StringValues bodydata);
            string test = bodydata;
            string decodedContent = WebUtility.UrlDecode(test);
            var res = JsonConvert.DeserializeObject<ChatPlusInformation>(test);

            //ChatPlusInformation webhookResponse = JsonConvert.DeserializeObject<ChatPlusInformation>(requestBody);
            //string name = webhookResponse.visitor.visitor_id;
            string name = "default user";


            //Get conversation reference from input binding
            //ConversationReference reference = JsonConvert.DeserializeObject<ConversationReference>(conversationInformationList.OrderByDescending(r => r.Timestamp).First().ConversationReference);
            //ConversationReference reference = JsonConvert.DeserializeObject<ConversationReference>(conversationInformation.ConversationReference);

            //Create Connector Client
            var appCredential = new MicrosoftAppCredentials("<Microsoft App Id>", "Microsoft App Password");
            //string MicrosoftAppId = configuration["MicrosoftAppId"];
            //string MicrosoftAppPassword = configuration["MicrosoftAppPassword"];
            //var appCredential = new MicrosoftAppCredentials(MicrosoftAppId, MicrosoftAppPassword);
            MicrosoftAppCredentials.TrustServiceUrl(reference.ServiceUrl);
            var connector = new ConnectorClient(new Uri(reference.ServiceUrl), appCredential);

            //Compose activity
            if (res.type != "chat_start")
            {
                var proactiveMessage = Activity.CreateMessageActivity();
                proactiveMessage.From = reference.Bot;
                proactiveMessage.Conversation = reference.Conversation;
                proactiveMessage.Text = res.message.text;

                try
                {
                    await connector.Conversations.SendToConversationAsync((Activity)proactiveMessage);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }

            //Return 200 or 400 to web client
            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name} (id:{reference.User.Id})")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}