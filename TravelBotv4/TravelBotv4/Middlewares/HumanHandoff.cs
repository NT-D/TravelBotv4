using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Underscore.Bot.MessageRouting;
using TravelBotv4.MessageRouting;
using TravelBotv4.CommandHandling;


namespace TravelBotv4.Middlewares
{
    public class HumanHandoff : IMiddleware
    {
        public async Task OnProcessRequest(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            var activity = context.Request.AsMessageActivity();

            if (activity.Type == ActivityTypes.Message)
            {
                MessageRouterManager messageRouterManager = Startup.MessageRouterManager;
                MessageRouterResultHandler messageRouterResultHandler = Startup.MessageRouterResultHandler;
                bool rejectConnectionRequestIfNoAggregationChannel =
                    Startup.Settings.RejectConnectionRequestIfNoAggregationChannel;

                messageRouterManager.MakeSurePartiesAreTracked(activity);

                // First check for commands (both from back channel and the ones directly typed)
                MessageRouterResult messageRouterResult =
                    Startup.BackChannelMessageHandler.HandleBackChannelMessage((Activity)activity);

                if (messageRouterResult.Type != MessageRouterResultType.Connected
                    && await Startup.CommandMessageHandler.HandleCommandAsync((Activity)activity) == false)
                {
                    // No valid back channel (command) message or typed command detected

                    // Let the message router manager instance handle the activity
                    messageRouterResult = await messageRouterManager.HandleActivityAsync(
                        activity, false, rejectConnectionRequestIfNoAggregationChannel);

                    if (messageRouterResult.Type == MessageRouterResultType.NoActionTaken)
                    {
                        // No action was taken by the message router manager. This means that the
                        // user is not connected (in a 1:1 conversation) with a human
                        // (e.g. customer service agent) yet.
                        //
                        // You can, for example, check if the user (customer) needs human
                        // assistance here or forward the activity to a dialog. You could also do
                        // the check in the dialog too...
                        //
                        // Here's an example:
                        if (!string.IsNullOrEmpty(activity.Text)
                            && activity.Text.ToLower().Contains(Commands.CommandRequestConnection))
                        {
                            messageRouterResult = messageRouterManager.RequestConnection(
                                activity, rejectConnectionRequestIfNoAggregationChannel);
                        }
                        else
                        {
                            await next();
                        }
                    }
                }

                // Uncomment to see the result in a reply (may be useful for debugging)
                //await MessagingUtils.ReplyToActivityAsync(activity, messageRouterResult.ToString());

                // Handle the result, if required
                await messageRouterResultHandler.HandleResultAsync(messageRouterResult);
            }
        }
    }
}
