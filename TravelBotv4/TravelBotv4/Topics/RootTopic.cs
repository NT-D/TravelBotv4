using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using PromptlyBot;
using PromptlyBot.Prompts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBotv4.Services;
using TravelBotv4.Services.Models;

namespace TravelBotv4.Topics
{
    public class RootTopicState : ConversationTopicState
    {

    }

    public class RootTopic : TopicsRoot<BotConversationState, RootTopicState>
    {
        private const string SELECT_QUESTION_TOPIC = "SelectQuestionTopic";
        private const string SELECT_QUESTION_PROMPT = "SelectQuestionPrompt";
        private static bool SearcherFeedbackStaet = false;
        private static bool SelectQuestionState = false;
        private static string qnaanswer = string.Empty;
        private static string[] questionlist = null;

        public RootTopic(IBotContext context) : base(context)
        {
            // User state initialization should be done once in the welcome 
            //  new user feature. Placing it here until that feature is added.
            this.SubTopics.Add("SelectQuestionPrompt", (object[] args) =>
            {
                var SelectQuesttionPrompt = new Prompt<string>();

                SelectQuesttionPrompt.Set
                    .OnPrompt(qnaanswer)
                    .MaxTurns(2)
                    .OnSuccess((ctx, value) =>
                    {
                        this.ClearActiveTopic();

                        //this.State.Name = value;
                        context.SendActivity(qnaanswer);

                        this.OnReceiveActivity(context);
                    })
                    .OnFailure((ctx, reason) =>
                    {
                        this.ClearActiveTopic();

                        context.SendActivity("I'm sorry I'm having issues understanding you.");

                        this.OnReceiveActivity(context);
                    });

                return SelectQuesttionPrompt;
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
                if (!SearcherFeedbackStaet) {
                    await context.SendActivity("Got it!");
                }

                // CHIT
                var chitchater = new ChitChater();
                var answer = await chitchater.SearchChitChat(message.Text);
                if (answer != null)
                {
                    await context.SendActivity(answer);
                    return;
                }
                
                // Feedback
                if (SearcherFeedbackStaet)
                {
                    SearcherFeedbackStaet = false;
                    var feedbacker = new Feedbacker();
                    var feedback = await feedbacker.SearchAsync(message.Text);
                    if (feedback == Feedbacker.INTENT.FEEDBACK_NEGATIVE) {
                        await context.SendActivity("Sorry, but could you try agein using another term?");
                        return;
                    } else if (feedback == Feedbacker.INTENT.FEEDBACK_POSITIVE)
                    {
                        await context.SendActivity("No problem!");
                        return;
                    }
                    // Not reterun and continue next line when you get NOEN intent.
                }

                // QnA
                var qnamaker = new QnaMaker();
                var queryresults = await qnamaker.SearchQnaMaker(message.Text);

                if (queryresults != null)
                    if (queryresults.First().Questions.Count() == 1)
                    {
                        await context.SendActivity(queryresults.First().Answer);
                        return;
                    }
                    else
                    {
                        SearcherFeedbackStaet = true;
                        var messages = "Did you mean? Please input number(1 - 4)";

                        foreach (var q in queryresults.First().Questions.Select((value, index) => new { value, index }))
                        {
                            if (q.index > 2)
                            {
                                messages += "\n\n" + "\n\n" + (q.index + 1) + ".None of adove";
                                break;
                            }
                            messages += "\n\n" + "\n\n" + (q.index + 1) + "." + queryresults.First().Questions[q.index].ToString();
                        }
                        qnaanswer = messages;

                        await this.SetActiveTopic(SELECT_QUESTION_PROMPT)
                                .OnReceiveActivity(context);
                        return;
                    }

                // Search
                var finder = new Finder();
                var result = await finder.SearchAsync(message.Text);
                if (result != null) {
                    SearcherFeedbackStaet = true;
                    var activity = createReply(context, result);
                    await context.SendActivity(activity);
                    await context.SendActivity("Did you find what you ware looking for?");
                    return;
                }
                await context.SendActivity("Sorry, but I didn't understand that. Could you try saying that another way?");
            }
        }
        private Activity createReply(IBotContext context, BaseSearchResult result)
        {
            var reply = context.Request.CreateReply();
            if (result.GetType() == typeof(SpotsResult))
            {
                reply.Attachments = result.Attachments;
            }
            else if (result.GetType() == typeof(PlansResult))
            {
                // TODO: 
            }
            return reply;
        }
    }
}
