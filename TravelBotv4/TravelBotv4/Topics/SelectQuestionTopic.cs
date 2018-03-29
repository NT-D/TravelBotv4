using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai;
using PromptlyBot;
using PromptlyBot.Prompts;
using PromptlyBot.Validator;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace TravelBotv4.Topics
{
    public class SelectQuestionTopicState : ConversationTopicState
    {
        public string answer { get; set; }
    }

    public class SelectQuestionTopic : ConversationTopic<ChitChatTopicState, string>
    {
        private const string ANSWER_PROMPT = "answerPrompt";
        private QnAMaker qnAMaker;

        public SelectQuestionTopic() : base()
        {
            this.SubTopics.Add(ANSWER_PROMPT, (object[] args) =>
            {
                var answerPrompt = new Prompt<string>();

                answerPrompt.Set
                    .OnPrompt((context, lastTurnReason) =>
                    {
                        context.SendActivity(this.State.answer);

                    })
                    .OnSuccess((context, value) =>
                    {
                        this.ClearActiveTopic();
                        this.State.answer = value;
                        context.SendActivity("SuccessMessages");
                        //OnReceiveActivity(context);
                    })
                    .OnFailure((context, reason) =>
                    {
                        this.ClearActiveTopic();
                        this.OnFailure(context, reason);
                    });

                return answerPrompt;
            });
        }

        public async override Task OnReceiveActivity(IBotContext context)
        {
            if (HasActiveTopic)
            {
                await ActiveTopic.OnReceiveActivity(context);
            }

            var options = new QnAMakerOptions()
            {
                KnowledgeBaseId = "",
                SubscriptionKey = "",
                ScoreThreshold = 0.7f
            };

            qnAMaker = new QnAMaker(options);
            var results = await qnAMaker.GetAnswers(context.Request.Text);
            this.State.answer = results.First().Answer;

            await this.SetActiveTopic(ANSWER_PROMPT)
                .OnReceiveActivity(context);

            this.OnSuccess(context, this.State.answer);
        }
    }
}
