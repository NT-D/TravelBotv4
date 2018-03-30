using Microsoft.Bot.Builder.Ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBotv4.Services
{
    public class QnaMaker
    {
        private QnAMaker qnAMaker;
        public async Task<QueryResult[]> SearchQnaMaker(string message)
        {
            var options = new QnAMakerOptions()
            {
                KnowledgeBaseId = "",
                SubscriptionKey = "",
                ScoreThreshold = 0.7f
            };

            qnAMaker = new QnAMaker(options);
            var results = await qnAMaker.GetAnswers(message);
            if (results.Count() > 0)
            {
                return results;
            }
            else
            {
                return null;
            }

        }
    }
}
