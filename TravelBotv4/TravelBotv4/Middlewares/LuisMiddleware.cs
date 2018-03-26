using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.LUIS;
using Microsoft.Cognitive.LUIS;
using Microsoft.Bot.Schema;

namespace TravelBotv4.Middlewares
{
    public class LuisMiddleware : IMiddleware
    {
        public const string LuisRecognizerResultKey = "LuisRecognizerResult";
        private readonly IRecognizer luisRecognizer;

        public LuisMiddleware(ILuisModel luisModel, ILuisRecognizerOptions luisRecognizerOptions = null, ILuisOptions luisOptions = null)
        {
            if (luisModel == null)
                throw new ArgumentNullException(nameof(luisModel));
            luisRecognizer = new LuisRecognizer(luisModel, luisRecognizerOptions, luisOptions);
        }

        public async Task OnProcessRequest(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            BotAssert.ContextNotNull(context);
            if (context.Request.Type == ActivityTypes.Message)
            {
                var utterance = context.Request.AsMessageActivity().Text;
                var result = await luisRecognizer.Recognize(utterance, CancellationToken.None);
                context.Set(LuisRecognizerResultKey, result);
            }
            await next();
        }
    }
}
