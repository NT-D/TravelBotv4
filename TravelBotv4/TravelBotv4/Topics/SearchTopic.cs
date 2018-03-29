using TravelBotv4.Models;
using Microsoft.Bot.Builder;
using PromptlyBot;
using PromptlyBot.Prompts;
using PromptlyBot.Validator;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.LUIS;
using System.Threading;
using TravelBotv4.Services.Models;
using System.Linq;

namespace TravelBotv4.Topics
{
    public class SearchTopicState : ConversationTopicState
    {
        public Alarm Alarm = new Alarm();
    }

    public class SearchTopic : ConversationTopic<SearchTopicState, Alarm>
    {
        private const string SEARCH_PROMPT = "searchPrompt";
        private const string FEEDBACK_PROMPT = "feedbackPrompt";

        public SearchTopic() : base()
        {
            // Feedback Prompt
            this.SubTopics.Add(FEEDBACK_PROMPT, (object[] args) =>
            {
                var feedbackPrompt = new Prompt<string>();

                feedbackPrompt.Set
                    .OnPrompt((context, lastTurnReason) =>
                    {
                        context.SendActivity("[feedbackPrompt] OnPrompt. How this Spot? Do You Like this? enter 'YES' or 'NO'!!");
                    })
                    .Validator(new DummyValidator())
                    .MaxTurns(2)
                    .OnSuccess((context, value) =>
                    {
                        this.State.Alarm.Title = "Searched";

                        this.ClearActiveTopic();
                        context.SendActivity("[feedbackPrompt] OnSuccess! ClearActiveTopic!");
                        // SearchTopicを抜ける
                        //this.OnSuccess(context, null);
                        //this.OnSuccess(context, new SearchTopicValue
                        //{
                        //    Alarm = this.State.Alarm,
                        //    AlarmIndex = (int)this.State.AlarmIndex,
                        //    DeleteConfirmed = (bool)this.State.DeleteConfirmed
                        //});
                    })
                    .OnFailure((context, reason) =>
                    {
                        this.ClearActiveTopic();
                        context.SendActivity("[feedbackPrompt] OnFailure! ClearActiveTopic!");
                    });

                return feedbackPrompt;
            });

        }

        private async Task<RecognizerResult> playLuis(IBotContext botContext, string utterance)
        {
            await botContext.SendActivity("Start LUIS");
            
            // finder
            var luisModel = new LuisModel("", "", new System.Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/178e1700-34e6-401b-8d60-f831b0b449ad?subscription-key=50110d00f75b486480efa8fd8b537552&verbose=true&timezoneOffset=0&q="));
            // feedback
            //var luisModel = new LuisModel("", "", new System.Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/3a3abee2-3567-4f85-9fc6-2d17a3189a08?subscription-key=50110d00f75b486480efa8fd8b537552&verbose=true&timezoneOffset=0&q="));

            var luisRecognizer = new LuisRecognizer(luisModel);
            return await luisRecognizer.Recognize(utterance, CancellationToken.None);
        }


        public override async Task OnReceiveActivity(IBotContext context)
        {
            if (HasActiveTopic)
            {
                await ActiveTopic.OnReceiveActivity(context);
            }

            var utterance = context.Request.AsMessageActivity().Text;

            // LUIS実行
            await context.SendActivity("got it!");
            var luisResult = await playLuis(context, utterance);


            // TODO: LUISの戻り値に応じて呼び出すAPIを変更する
            var intent = luisResult.Intents.GetValue("Places.FindPlace");
            var entity = luisResult.Entities.GetValue("Places_AbsoluteLocation");
            var entity_keyword = entity.First().ToString();
            await context.SendActivity(entity_keyword.ToString());

            /*
            if (this.State.Alarm.Title == "Searched") {
                this.SetActiveTopic(SEARCH_PROMPT)
                    .OnReceiveActivity(context);
                return Task.CompletedTask;

            }

            */
            // LUISの結果でスポット検索
            var service = new Services.SpotSearchService();
            var req = new SpotsRequest();
            req.keyword = entity_keyword;
            var result = await service.Search(req) as SpotsResult;
            await context.SendActivity(result.spots.First().name);

            // Replyを作成し表示
            var reply = context.Request.CreateReply();
            reply.Attachments = result.Attachments;
            await context.SendActivity(reply);

            // next feed back prompt
            await this.SetActiveTopic(FEEDBACK_PROMPT)
                .OnReceiveActivity(context);


            /*
            if (this.State.Alarm.Title == null) {// spotの場合
                await context.SendActivity("スポット表示");
                this.State.Alarm.Title = "Searched";

                // APIの戻り値表示

                // next feed back prompt
                await this.SetActiveTopic(FEEDBACK_PROMPT)
                    .OnReceiveActivity(context);
            }
            else {
                await context.SendActivity("スポット表示失敗");
            }
            await context.SendActivity("すみません、お役に立てなくて");
            this.OnSuccess(context, null);


            await context.SendActivity("ここまでくれば終了で抜けます");
            this.OnSuccess(context, this.State.Alarm);
            */
        }
    }

    public class AlarmTitleValidator : Validator<string>
    {
        public override ValidatorResult<string> Validate(IBotContext context)
        {
            return new ValidatorResult<string>
            {
                Value = context.Request.AsMessageActivity().Text
            };
        }
    }

    public class DummyValidator : Validator<string>
    {
        public override ValidatorResult<string> Validate(IBotContext context)
        {
            return new ValidatorResult<string>
            {
                Value = context.Request.AsMessageActivity().Text
            };
        }
    }
}
