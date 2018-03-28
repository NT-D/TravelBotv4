using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Underscore.Bot.MessageRouting;
using TravelBotv4.MessageRouting;
using TravelBotv4.CommandHandling;
using TravelBotv4.Models;

namespace TravelBotv4.Middlewares
{
    public class HumanHandoff : IMiddleware
    {
        public async Task OnProcessRequest(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            var activity = context.Request.AsMessageActivity();

            if (activity.Type == ActivityTypes.Event && activity.From.Id == "ConversationOwner")
            {
                // TODO hiroaki-honda Implement logic to send proactive message to user
            }

            if (!string.IsNullOrEmpty(activity.Text)
                && activity.Text.ToLower().Contains(Commands.CommandRequestConnection))
            {
                // Store conversation reference
                var conversationReference = GetConversationReference(context.Request);
                var conversationInformation = new ConversationInformation()
                {
                    conversationReference = conversationReference,
                    MessageFromUser = context.Request.Text
                };
                var storageConnectionString = Startup.Settings["KeyRoutingDataStorageConnectionString"];
                CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
                CloudTableClient tableClient = account.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("ConversationReference");
                await table.CreateIfNotExistsAsync();
                TableOperation insertOperation = TableOperation.Insert(conversationInformation);

                // TODO hiroaki-honda Implement the logic to hook the function which request connection to ChatPlus


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
    }
}
