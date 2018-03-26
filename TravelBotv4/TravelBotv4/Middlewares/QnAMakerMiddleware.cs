using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai;
using Microsoft.Bot.Schema;

namespace TravelBotv4.Middlewares
{
    public class QnAMakerMiddleware : IMiddleware
    {
        private readonly QnAMaker qnAMaker;
        private readonly QnAMakerMiddlewareOptions qaOptions;

        public QnAMakerMiddleware(QnAMakerMiddlewareOptions options)
        {
            qaOptions = options ?? throw new ArgumentNullException(nameof(options));
            qnAMaker = new QnAMaker(options);
        }

        public async Task OnProcessRequest(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            await context.SendActivity("I'm QnAMakerMiddleware");
            if (context.Request.Type == ActivityTypes.Message)
            {
                var messageActivity = context.Request.AsMessageActivity();
                if (!string.IsNullOrEmpty(messageActivity.Text))
                {
                    var results = await qnAMaker.GetAnswers(messageActivity.Text.Trim());
                    if (results.Any())
                    {
                        if (!string.IsNullOrEmpty(qaOptions.DefaultAnswerPrefixMessage))
                            await context.SendActivity(qaOptions.DefaultAnswerPrefixMessage);

                        await context.SendActivity(results.First().Answer);

                        if (qaOptions.EndActivityRoutingOnAnswer)
                            return;
                    }
                }
            }
            await next();
        }
    }
}
