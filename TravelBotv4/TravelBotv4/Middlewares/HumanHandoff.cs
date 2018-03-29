using System.Threading.Tasks;
using System;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Core;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Underscore.Bot.MessageRouting;
using TravelBotv4.MessageRouting;
using TravelBotv4.CommandHandling;
using TravelBotv4.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TravelBotv4.Middlewares
{
    public class HumanHandoff : IMiddleware
    {
        public async Task OnProcessRequest(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            var activity = context.Request.AsMessageActivity();

            if (activity == null)
            {
                await next();
            }

            // Handle the message from functions contains ChatPlus's webhook response
            if (activity.Type == ActivityTypes.Event)
            {
                string userId = "default-user"; // TODO hiroaki-honda remove this line and replace userId used as key to extract ConversationInformation from table storage. ("default-user" is the userId just for Hackfest).
                // string userId = Deserialize<Visitor>("visitor", (Activity)activity).visitor_id;
                ConversationReference conversationReference = await GetConversationReferenceByUserId(userId);

                switch (activity.From.Id)
                {
                    case "WebhookStartChat":
                        string messageForSuccessToConnect = "Success to make a connection with call center agent. Please let us know what makes you in trouble.";
                        await SendProactiveMessage(context, conversationReference, messageForSuccessToConnect);
                        break;
                    case "WebhookSendMessage":
                        string messageFromAgent = JsonConvert.DeserializeObject<ChatPlusInformation>(activity.Value.ToString()).message.text;
                        await SendProactiveMessage(context, conversationReference, messageFromAgent);
                        break;
                    default:
                        throw new Exception("unexpected event type message");
                }
            }
            
            // Enqueue the message to hook the function which passes the message to agent if "IsConnectedToAgent" is true.
            var connectionState = context.GetUserState<ConnectionState>();
            if (connectionState != null && connectionState.IsConnectedToAgent)
            {
                CloudStorageAccount account = buildStorageAccount();
                CloudQueueClient cloudQueueClient = account.CreateCloudQueueClient();
                CloudQueue queue = cloudQueueClient.GetQueueReference("message-from-user");
                var item = new ConversationInformation()
                {
                    conversationReference = JsonConvert.SerializeObject(GetConversationReference((Activity)activity)),
                    MessageFromUser = context.Request.Text
                };
                var message = new CloudQueueMessage(JsonConvert.SerializeObject(item));
                await queue.AddMessageAsync(message);
                return;
            }

            // Request to make a connection between user and agent
            if (!string.IsNullOrEmpty(activity.Text)
                && activity.Text.ToLower().Contains(Commands.CommandRequestConnection))
            {
                // Store conversation reference (Use this info when send a proactive message to user after).
                await StoreConversationInformation(context);

                // TODO hiroaki-honda Implement the logic to hook the function which request connection to ChatPlus
                // Status: Ask chatplus to prepare the API which receive the request to connect to agent

                // Set connecting state true
                var state = context.GetUserState<ConnectionState>();
                state.IsConnectedToAgent = true;
            }
            else
            {
                await next();
            }
        }

        private async Task StoreConversationInformation(IBotContext context)
        {
            var conversationReference = GetConversationReference(context.Request);
            var conversationInformation = new ConversationInformation()
            {
                PartitionKey = "ConversationInformation",
                RowKey = conversationReference.User.Id,
                conversationReference = JsonConvert.SerializeObject(conversationReference),
                MessageFromUser = context.Request.Text
            };
            CloudStorageAccount account = buildStorageAccount();
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("ConversationInformation");
            await table.CreateIfNotExistsAsync();
            TableOperation insertOperation = TableOperation.Insert(conversationInformation);
            await table.ExecuteAsync(insertOperation);
        }

        public static ConversationReference GetConversationReference(Activity activity)
        {
            BotAssert.ActivityNotNull(activity);

            ConversationReference r = new ConversationReference
            {
                ActivityId = activity.Id,
                User = activity.From,
                Bot = activity.Recipient,
                Conversation = activity.Conversation,
                ChannelId = activity.ChannelId,
                ServiceUrl = activity.ServiceUrl
            };

            return r;
        }

        private async Task SendProactiveMessage(IBotContext context, ConversationReference conversationReference, string messageFromAgent)
        {
            // TODO hiroaki-honda Implement logic to send proactive message to user
            await context.Adapter.ContinueConversation(conversationReference, (IBotContext _context) => PassTheMessageToUserFromAgent(_context, messageFromAgent));
        }

        public async Task PassTheMessageToUserFromAgent(IBotContext context, string message)
        {
            await context.SendActivity(message);
        }

        private T Deserialize<T>(string name, Activity activity)
        {
            return JsonConvert.DeserializeObject<T>(((JObject)activity.Value).GetValue(name).ToString());
        }

        private async Task<ConversationReference> GetConversationReferenceByUserId(string userId)
        {
            CloudStorageAccount account = buildStorageAccount();
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("ConversationInformation");
            TableOperation getConversationInformation = TableOperation.Retrieve("ConversationInformation", userId);
            var res = await table.ExecuteAsync(getConversationInformation);
            var conversationInformation = (ConversationInformation)res.Result;
            return JsonConvert.DeserializeObject<ConversationReference>(conversationInformation.conversationReference);
        }

        private CloudStorageAccount buildStorageAccount()
        {
            var storageConnectionString = Startup.Settings.ConnectionString;
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            return account;
        }
    }
}
