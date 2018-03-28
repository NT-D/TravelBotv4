using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.LUIS;
using Microsoft.Bot.Schema;
using PromptlyBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBotv4.Topics;

namespace TravelBotv4
{
    public class BotConversationState : PromptlyBotConversationState<RootTopicState>
    {
    }

    public class BotUserState : StoreItem
    {
    }
    public class TravelBot : IBot
    {
        /*
        public async Task OnReceiveActivity(IBotContext botContext)
        {
            if (botContext.Request.Type is ActivityTypes.Message)
            {
                //var result = botContext.Get<RecognizerResult>(NTLuisMiddleware.LuisRecognizerResultKey);
                await botContext.SendActivity("Hi");
            }
        }
        */

        public Task OnReceiveActivity(IBotContext context)
        {
            var rootTopic = new Topics.RootTopic(context);

            rootTopic.OnReceiveActivity(context);

            return Task.CompletedTask;
        }



    }
}