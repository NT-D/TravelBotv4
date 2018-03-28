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

            // Handle the message from functions contains ChatPlus's webhook response
            if (activity.Type == ActivityTypes.Event)
            {
                string userId = Deserialize<Visitor>("visitor", (Activity)activity).visitor_id;
                ConversationReference conversationReference = GetConversationReferenceByUserId(userId);

                switch (activity.From.Id)
                {
                    case "WebhookStartChat":
                        await SendProactiveMessage(context, conversationReference);
                        break;
                    case "WebhookSendMessage":
                        await SendProactiveMessage(context, conversationReference);
                        break;
                    default:
                        throw new Exception("unexpected event type message");
                }
            }

            var connectionState = context.GetConversationState<ConnectionState>();
            if (connectionState.IsConnectedToAgent)
            {
                // TODO hiroaki-honda Hook the function which send message to agent
            }

            // Request to make a connection between user and agent
            if (!string.IsNullOrEmpty(activity.Text)
                && activity.Text.ToLower().Contains(Commands.CommandRequestConnection))
            {
                // Store conversation reference
                var conversationReference = GetConversationReference(context.Request);
                var conversationInformation = new ConversationInformation()
                {
                    PartitionKey = "ConversationInformation",
                    RowKey = conversationReference.User.Id,
                    conversationReference = conversationReference,
                    MessageFromUser = context.Request.Text
                };
                var storageConnectionString = Startup.Settings["KeyRoutingDataStorageConnectionString"];
                CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
                CloudTableClient tableClient = account.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("ConversationInformation");
                await table.CreateIfNotExistsAsync();
                TableOperation insertOperation = TableOperation.Insert(conversationInformation);

                // TODO hiroaki-honda Implement the logic to hook the function which request connection to ChatPlus
                // Status: Ask chatplus to prepare the API which receive the request to connect to agent

                // Set connecting state true 
                var state = context.GetConversationState<ConnectionState>();
                state.IsConnectedToAgent = true;
            }
            else
            {
                await next();
            }
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

        private async Task SendProactiveMessage(IBotContext context, ConversationReference conversationReference)
        {
            // TODO hiroaki-honda Implement logic to send proactive message to user
            //context.Adapter.ContinueConversation()
        }

        private T Deserialize<T>(string name, Activity activity)
        {
            return JsonConvert.DeserializeObject<T>(((JObject)activity.Value).GetValue(name).ToString());
        }

        private ConversationReference GetConversationReferenceByUserId(string userId)
        {
            // TODO hiroaki-honda Implement the logic to get ConversationReferenc from table storage using userId as key
            return null;
        }
    }
}
