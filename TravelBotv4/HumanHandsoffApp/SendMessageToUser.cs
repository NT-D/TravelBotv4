using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Bot.Schema;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using HumanHandsoffApp.Models;

namespace HumanHandsoffApp
{
    public static class SendMessageToUser
    {
        public static IConfiguration configuration { get; set; }

        [FunctionName(nameof(SendMessageToUser))]
        public async static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, 
            [Table(tableName:"ConversationInformation", partitionKey: "ConversationInformation", Connection = "HumanHandsoffStorage")]IQueryable<ConversationInformation> conversationInformationList,
            TraceWriter log)
        {
            //TODO: Need to refactor.
            log.Info("C# HTTP trigger function processed a request.");

            //Parse body from ChatPlus' webhook
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            ChatPlusInformation webhookResponse = JsonConvert.DeserializeObject<ChatPlusInformation>(requestBody);
            string name = webhookResponse.visitor.visitor_id;

            //Get conversation reference from input binding
            ConversationReference reference = JsonConvert.DeserializeObject<ConversationReference>(conversationInformationList.OrderByDescending(r => r.Timestamp).First().ConversationReference);
            //ConversationReference reference = JsonConvert.DeserializeObject<ConversationReference>(conversationInformation.ConversationReference);

            //Create Connector Client
            //var appCredential = new MicrosoftAppCredentials("<Your MicrosoftAppId which you set in application.json in TravelBot v4>", "<Your MicrosoftAppPassword which you set in application.json in TravelBot v4>");
            string MicrosoftAppId = configuration["MicrosoftAppId"];
            string MicrosoftAppPassword = configuration["MicrosoftAppPassword"];
            var appCredential = new MicrosoftAppCredentials(MicrosoftAppId, MicrosoftAppPassword);
            MicrosoftAppCredentials.TrustServiceUrl(reference.ServiceUrl);
            var connector = new ConnectorClient(new Uri(reference.ServiceUrl), appCredential);

            //Compose activity
            var proactiveMessage = Activity.CreateMessageActivity();
            proactiveMessage.From = reference.Bot;
            proactiveMessage.Conversation = reference.Conversation;
            proactiveMessage.Text = webhookResponse.message.text;

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