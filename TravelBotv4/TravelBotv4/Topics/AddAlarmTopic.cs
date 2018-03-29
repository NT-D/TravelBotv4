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
        private const string SEARCH_PROMPT = "searchPrompt";
        private const string FEEDBACK_PROMPT = "feedbackPrompt";

        public AddAlarmTopic() : base()
        {
            // Search
            this.SubTopics.Add(SEARCH_PROMPT, (object[] args) =>
            {
                var searchPrompt = new Prompt<string>();

                searchPrompt.Set
                    .OnPrompt((context, lastTurnReason) =>
                    {
                        context.SendActivity("SearchPrompt");
                    })
                    .Validator(new DummyValidator())
                    .MaxTurns(2)
                    .OnSuccess((context, value) =>
                    {
                        this.ClearActiveTopic();
                        this.OnReceiveActivity(context);
                        context.SendActivity("OnSuccess! ClearActiveTopic!");

                    })
                    .OnFailure((context, reason) =>
                    {
                        this.ClearActiveTopic();
                        this.OnReceiveActivity(context);
                        context.SendActivity("OnFailure! ClearActiveTopic!");
                    });

                return searchPrompt;
            });

            // Feedback
            this.SubTopics.Add(FEEDBACK_PROMPT, (object[] args) =>
            {
                var feedbackPrompt = new Prompt<string>();

                feedbackPrompt.Set
                    .OnPrompt((context, lastTurnReason) =>
                    {
                        context.SendActivity("feedbackPrompt");
                    })
                    .Validator(new DummyValidator())
                    .MaxTurns(2)
                    .OnSuccess((context, value) =>
                    {
                        this.ClearActiveTopic();
                        this.OnReceiveActivity(context);
                        context.SendActivity("OnSuccess! ClearActiveTopic!");

                    })
                    .OnFailure((context, reason) =>
                    {
                        this.ClearActiveTopic();
                        this.OnReceiveActivity(context);
                        context.SendActivity("OnFailure! ClearActiveTopic!");
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


            // LUISの戻り値に応じて呼び出すAPIを変更する


            if (true) // spotの場合
            {
                // APIの戻り値表示

                // next feed back prompt
                this.SetActiveTopic(FEEDBACK_PROMPT)
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
