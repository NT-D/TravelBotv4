﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using PromptlyBot;
using PromptlyBot.Prompts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBotv4.Middlewares;
using TravelBotv4.Models;
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
        private static bool SearcherFeedbackState = false;
        private static bool SelectQuestionState = false;
        private static string qnaanswer = string.Empty;
        private static List<string> questionlist = new List<string>();
        private static int turn = 0;
        private static int maxturn = 1;
        private static int i;

        public RootTopic(IBotContext context) : base(context)
        {
            // User state initialization should be done once in the welcome 
            //  new user feature. Placing it here until that feature is added.
        }


        public async override Task OnReceiveActivity(IBotContext context)
        {
            // IMAGE
            var image = context.Get<ImageRecognizeResult>(ImageMiddleware.ImageRecognizerResultKey);
            if (image != null)
            {
                await context.SendActivity("Thaks for sending me a photo!\nLet's see...");

                var keyword = image.PrimaryKeyword();
                var finder = new Finder();
                var result = await finder.SearchWithKeywordAsync(keyword);

                if (result != null)
                {
                    SearcherFeedbackState = true;
                    var activity = createReply(context, result);
                    await context.SendActivity(activity);
                    await context.SendActivity("Did you find what you ware looking for?");
                    return;
                }
            }

            if ((context.Request.Type == ActivityTypes.Message) && (context.Request.AsMessageActivity().Text.Length > 0))
            {
                var message = context.Request.AsMessageActivity();
                var qnamaker = new QnaMaker();
                // If there is an active topic, let it handle this turn until it completes.
                if (HasActiveTopic)
                {
                    await ActiveTopic.OnReceiveActivity(context);
                    return;
                }
                if (!SearcherFeedbackState)
                {
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
                if (SearcherFeedbackState)
                {
                    SearcherFeedbackState = false;
                    var feedbacker = new Feedbacker();
                    var feedback = await feedbacker.SearchAsync(message.Text);
                    if (feedback == Feedbacker.INTENT.FEEDBACK_NEGATIVE)
                    {
                        await context.SendActivity("Sorry, but could you try agein using another term?");
                        return;
                    }
                    else if (feedback == Feedbacker.INTENT.FEEDBACK_POSITIVE)
                    {
                        await context.SendActivity("No problem!");
                        return;
                    }
                    // Not reterun and continue next line when you get NOEN intent.
                }

                // SelectQuestion
                if (SelectQuestionState)
                {
                    if (int.TryParse(message.Text, out i) && (i < 4))
                    {
                        var selectquestion = questionlist[i];
                        var selectanswer = await qnamaker.SearchQnaMaker(selectquestion);
                        await context.SendActivity(selectanswer.First().Answer);
                        SelectQuestionState = false;
                        SearcherFeedbackState = true;
                        return;
                    }
                    else if (turn < maxturn)
                    {
                        await context.SendActivity("Sorry,but please input number(1 - 4)");
                        turn += 1;
                        return;
                    }
                    else
                    {
                        SelectQuestionState = false;
                        await context.SendActivity("too many attempts");
                        await context.SendActivity("OK! You may change your mind.");
                        return;
                    }
                }


                // QnA
                qnamaker = new QnaMaker();
                var queryresults = await qnamaker.SearchQnaMaker(message.Text);
                if (queryresults != null)
                {
                    if (queryresults.First().Questions.Count() == 1)
                    {
                        SearcherFeedbackState = true;
                        await context.SendActivity(queryresults.First().Answer);
                        return;
                    }
                    else
                    {
                        SelectQuestionState = true;
                        SearcherFeedbackState = true;
                        var messages = "Did you mean? Please input number(1 - 4)";
                        foreach (var q in queryresults.First().Questions.Select((value, index) => new { value, index }))
                        {
                            if (q.index > 2)
                            {
                                messages += "\n\n" + "\n\n" + (q.index + 1) + ".None of adove";
                                questionlist.Add(queryresults.First().Questions[q.index]);
                                break;
                            }
                            messages += "\n\n" + "\n\n" + (q.index + 1) + "." + queryresults.First().Questions[q.index].ToString();
                            questionlist.Add(queryresults.First().Questions[q.index]);
                        }
                        await context.SendActivity(messages);

                        return;
                    }
                }

                // Search
                var finder = new Finder();
                var result = await finder.SearchAsync(message.Text);
                if (result != null)
                {
                    SearcherFeedbackState = true;
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
