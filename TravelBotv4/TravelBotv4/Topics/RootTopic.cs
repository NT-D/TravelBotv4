using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using PromptlyBot;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace TravelBotv4.Topics
{
    public class RootTopicState : ConversationTopicState
    {

    }

    public class RootTopic : TopicsRoot<BotConversationState, RootTopicState>
    {
        private const string SPOT_TOPIC = "searchSpotTopic";
        private const string PLAN_TOPIC = "searchPlanTopic";

        public RootTopic(IBotContext context) : base(context)
        {
            /*
            // User state initialization should be done once in the welcome 
            //  new user feature. Placing it here until that feature is added.
            if (context.GetUserState<BotUserState>().Alarms == null)
            {
                context.GetUserState<BotUserState>().Alarms = new List<Alarm>();
            }

            this.SubTopics.Add(ADD_ALARM_TOPIC, (object[] args) =>
            {
                var addAlarmTopic = new AddAlarmTopic();

                addAlarmTopic.Set
                    .OnSuccess((ctx, alarm) =>
                    {
                        this.ClearActiveTopic();

                        ctx.GetUserState<BotUserState>().Alarms.Add(alarm);

                        context.SendActivity($"Added alarm named '{ alarm.Title }' set for '{ alarm.Time }'.");
                    })
                    .OnFailure((ctx, reason) =>
                    {
                        this.ClearActiveTopic();

                        context.SendActivity("Let's try something else.");

                        this.ShowDefaultMessage(ctx);
                    });

                return addAlarmTopic;
            });

            this.SubTopics.Add(DELETE_ALARM_TOPIC, (object[] args) =>
            {
                var alarms = (args.Length > 0) ? (List<Alarm>)args[0] : null;

                var deleteAlarmTopic = new DeleteAlarmTopic(alarms);

                deleteAlarmTopic.Set
                    .OnSuccess((ctx, value) =>
                    {
                        this.ClearActiveTopic();

                        if (!value.DeleteConfirmed)
                        {
                            context.SendActivity($"Ok, I won't delete alarm '{ value.Alarm.Title }'.");
                            return;
                        }

                        ctx.GetUserState<BotUserState>().Alarms.RemoveAt(value.AlarmIndex);

                        context.SendActivity($"Done. I've deleted alarm '{ value.Alarm.Title }'.");
                    })
                    .OnFailure((ctx, reason) =>
                    {
                        this.ClearActiveTopic();

                        context.SendActivity("Let's try something else.");

                        this.ShowDefaultMessage(context);
                    });

                return deleteAlarmTopic;
            });
            */
        }

        
        public override Task OnReceiveActivity(IBotContext context)
        {
            context.SendActivity("Got it!");
         
            if ((context.Request.Type == ActivityTypes.Message) && (context.Request.AsMessageActivity().Text.Length > 0))
            {
                var message = context.Request.AsMessageActivity();

                // If there is an active topic, let it handle this turn until it completes.
                if (HasActiveTopic)
                {
                    ActiveTopic.OnReceiveActivity(context);
                    return Task.CompletedTask;
                }

                // 通常処理
            }
            
            return Task.CompletedTask;   
        }
               
    }

}
