using TravelBotv4.Models;
using Microsoft.Bot.Builder;
using PromptlyBot;
using PromptlyBot.Prompts;
using PromptlyBot.Validator;
using System.Threading.Tasks;

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

        public override Task OnReceiveActivity(IBotContext context)
        {
            if (HasActiveTopic)
            {
                ActiveTopic.OnReceiveActivity(context);
                return Task.CompletedTask;
            }



            // LUIS実行
            context.SendActivity("got it!");


            // LUISの戻り値に応じて呼び出すAPIを変更する
            /*
            if (this.State.Alarm.Title == "Searched") {
                this.SetActiveTopic(SEARCH_PROMPT)
                    .OnReceiveActivity(context);
                return Task.CompletedTask;

            }
            */

            if (this.State.Alarm.Title == null) // spotの場合
            {
                context.SendActivity("スポット表示");
                this.State.Alarm.Title = "Searched";

                // APIの戻り値表示

                // next feed back prompt
                this.SetActiveTopic(FEEDBACK_PROMPT)
                    .OnReceiveActivity(context);
                return Task.CompletedTask;
            }
            else {
                context.SendActivity("スポット表示失敗");
            }
            context.SendActivity("すみません、お役に立てなくて");
            this.OnSuccess(context, null);


            context.SendActivity("ここまでくれば終了で抜けます");
            this.OnSuccess(context, this.State.Alarm);
            return Task.CompletedTask;
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
