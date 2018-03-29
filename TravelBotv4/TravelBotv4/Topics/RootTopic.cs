using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using PromptlyBot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TravelBotv4.Topics
{
    public class RootTopicState : ConversationTopicState
    {

    }

    public class RootTopic : TopicsRoot<BotConversationState, RootTopicState>
    {
        private const string SELECT_QUESTION_TOPIC = "SelectQuestionTopic";
        private QnAMaker qnAMaker;

        public RootTopic(IBotContext context) : base(context)
        {
            // User state initialization should be done once in the welcome 
            //  new user feature. Placing it here until that feature is added.

            this.SubTopics.Add(SELECT_QUESTION_TOPIC, (object[] args) =>
            {
                var SelectQuestionTopic = new SelectQuestionTopic();

                SelectQuestionTopic.Set
                    .OnSuccess((ctx, alarm) =>
                    {
                        this.ClearActiveTopic();
                        context.SendActivity($"TopicのOnSuccess");
                    })
                    .OnFailure((ctx, reason) =>
                    {
                        this.ClearActiveTopic();
                        context.SendActivity($"TopicのOnFailure"); ;
                    });
                return SelectQuestionTopic;
            });
        }


        public async override Task OnReceiveActivity(IBotContext context)
        {
            if ((context.Request.Type == ActivityTypes.Message) && (context.Request.AsMessageActivity().Text.Length > 0))
            {
                var message = context.Request.AsMessageActivity();

                // If there is an active topic, let it handle this turn until it completes.
                if (HasActiveTopic)
                {
                    await ActiveTopic.OnReceiveActivity(context);
                    return;
                }

                // CHIT
                // if() {
                // }

                // QnA
                // if() {
                // }

                // Searchf
                // if() {
                //}

                // Feedback
                // if() {
                //}

            }
        }
    }
}
