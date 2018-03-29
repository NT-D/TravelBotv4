using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.LUIS;
using Microsoft.Bot.Schema;
using PromptlyBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TravelBotv4.Services.Models;
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

        // LUIS TEST
        /*
        public async Task OnReceiveActivity(IBotContext botContext)
        {
            if (botContext.Request.Type is ActivityTypes.Message)
            {
                var utterance = botContext.Request.AsMessageActivity().Text;

                await botContext.SendActivity("Start LUIS");

                // finder
                var luisModel = new LuisModel("", "", new System.Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/178e1700-34e6-401b-8d60-f831b0b449ad?subscription-key=50110d00f75b486480efa8fd8b537552&verbose=true&timezoneOffset=0&q="));
                // feedback
                //var luisModel = new LuisModel("", "", new System.Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/3a3abee2-3567-4f85-9fc6-2d17a3189a08?subscription-key=50110d00f75b486480efa8fd8b537552&verbose=true&timezoneOffset=0&q="));

                var luisRecognizer = new LuisRecognizer(luisModel);
                var luisResult = await luisRecognizer.Recognize(utterance, CancellationToken.None);

                var intent = luisResult.Intents.GetValue("Places.FindPlace");
                var entity = luisResult.Entities.GetValue("Places_AbsoluteLocation");
                var entity_keyword = entity.First().ToString();
                await botContext.SendActivity(entity_keyword.ToString());

                // Places.FindPlace
                if (intent != null)
                {
                    await botContext.SendActivity(intent.ToString());
                }
    
                //var result = botContext.Get<RecognizerResult>(NTLuisMiddleware.LuisRecognizerResultKey);
                await botContext.SendActivity("Start Search");

                // LUISの結果でスポット検索
                var service = new Services.SpotSearchService();
                var req = new SpotsRequest();
                req.keyword = entity_keyword;
                var result = await service.Search(req) as SpotsResult;
                await botContext.SendActivity(result.spots.First().name);

                // Replyを作成し表示
                var reply = botContext.Request.CreateReply();
                reply.Attachments = result.Attachments;
                await botContext.SendActivity(reply);
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