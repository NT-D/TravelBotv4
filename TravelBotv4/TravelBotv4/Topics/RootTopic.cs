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
        private const string SEARCH_TOPIC = "SearchTopic";
        private const string CHIT_CHAT_TOPIC = "chitChatTopic";
        private const string ChitChatKey = "chitChatKey";
        private QnAMaker qnAMaker;

        public RootTopic(IBotContext context) : base(context)
        {
            // User state initialization should be done once in the welcome 
            //  new user feature. Placing it here until that feature is added.
            //if (context.GetUserState<BotUserState>().Alarms == null)
            //{
            //    context.GetUserState<BotUserState>().Alarms = new List<Alarm>();
            //}

            this.SubTopics.Add(CHIT_CHAT_TOPIC, (object[] args) =>
            {
                var chitChatTopic = new ChitChatTopic();

                chitChatTopic.Set
                    .OnSuccess((ctx, alarm) =>
                    {
                        this.ClearActiveTopic();

                        //ctx.GetUserState<BotUserState>().Alarms.Add(alarm);

                        //context.SendActivity("test");
                    })
                    .OnFailure((ctx, reason) =>
                    {
                        this.ClearActiveTopic();

                        context.SendActivity("FailureMessage");

                        //this.ShowDefaultMessage(ctx);
                    });

                return chitChatTopic;
            });


            // Searcher
            this.SubTopics.Add(SEARCH_TOPIC, (object[] args) =>
            {
                var searchTopic = new SearchTopic();

                searchTopic.Set
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
                return searchTopic;
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
                }
                // Feedbackに該当しない場合は、自動的に下記処理に流れる


                // APIの結果に応じて分岐する

                // CHITかどうかの判定
                // if() {
                await this.SetActiveTopic(CHIT_CHAT_TOPIC)
                .OnReceiveActivity(context);
                // }

                // QnA
                // if() {
                //await this.SetActiveTopic()
                //.OnReceiveActivity(context);
                // }

                // Searchf
                // if() {
                await this.SetActiveTopic(SEARCH_TOPIC)
                    .OnReceiveActivity(context);
                //}
                // Feedbackは、SearchのSubPromptのためここには不要

            }
        }
    }
}
