using TravelBotv4.Models;
using Microsoft.Bot.Builder;
using PromptlyBot;
using PromptlyBot.Prompts;
using PromptlyBot.Validator;
using System.Threading.Tasks;

namespace TravelBotv4.Topics
{
    public class AddAlarmTopicState : ConversationTopicState
    {
    }

    public class AddAlarmTopic : ConversationTopic<AddAlarmTopicState, Alarm>
    {
        private const string TITLE_PROMPT = "titlePrompt";

        public AddAlarmTopic() : base()
        {
            this.SubTopics.Add(TITLE_PROMPT, (object[] args) =>
            {
                var titlePrompt = new Prompt<string>();

                titlePrompt.Set
                    .OnPrompt((context, lastTurnReason) =>
                    {
                        context.SendActivity("Prompt");
                    })
                    .Validator(new DummyValidator())
                    .MaxTurns(2)
                    .OnSuccess((context, value) =>
                    {
                        this.ClearActiveTopic();
                        this.OnReceiveActivity(context);
                        context.SendActivity("OnFailure");

                    })
                    .OnFailure((context, reason) =>
                    {
                        this.ClearActiveTopic();
                        this.OnReceiveActivity(context);
                        context.SendActivity("OnFailure");
                    });

                return titlePrompt;
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


            // LUISの戻り値に応じて呼び出すAPIを変更する

            if (true) //　planの場合
            {
                this.SetActiveTopic(TITLE_PROMPT)
                    .OnReceiveActivity(context);
                return Task.CompletedTask;
            }
            
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
