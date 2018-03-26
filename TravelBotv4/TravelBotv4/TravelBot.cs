using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.LUIS;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBotv4
{
    public class TravelBot : IBot
    {
        public async Task OnReceiveActivity(IBotContext botContext)
        {
            if (botContext.Request.Type is ActivityTypes.Message)
            {
                //var result = botContext.Get<RecognizerResult>(NTLuisMiddleware.LuisRecognizerResultKey);
                await botContext.SendActivity("Hi");
            }
        }

    }
}