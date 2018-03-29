using System;
using System.Threading.Tasks;
using System.Net.Http;
using TravelBotv4.Services.Models;
using Newtonsoft.Json;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.LUIS;
using System.Threading;
using System.Linq;

namespace TravelBotv4.Services
{
    public class Feedbacker
    {
        
        private static string ModelId = "";
        private static string SubscriptionKey = "";
        private static string StreamUrl = "";
        

        public enum INTENT : UInt16
        {
            NONE = 0,
            FEEDBACK_NEGATIVE,
            FEEDBACK_POSITIVE
        };


        public async Task<Feedbacker.INTENT> SearchAsync(string utterance)
        {
            return await Search(utterance);
        }

        public async Task<Feedbacker.INTENT> Search(string utterance)
        {
            try
            {
                var luisResult = await find(utterance);
                var intent = primaryIntent(luisResult);
                return intent;
            }
            catch (Exception)
            {
                return Feedbacker.INTENT.NONE;
            }
        }

        private async Task<RecognizerResult> find(string utterance)
        {
            var luisModel = new LuisModel(ModelId, SubscriptionKey, new System.Uri(StreamUrl));
            var luisRecognizer = new LuisRecognizer(luisModel);
            return await luisRecognizer.Recognize(utterance, CancellationToken.None);
        }

        private INTENT primaryIntent(RecognizerResult luisResult)
        {
            if (luisResult.Intents.GetValue("Feedback.Negative") != null)
            {
                return INTENT.FEEDBACK_NEGATIVE;
            }
            else if (luisResult.Intents.GetValue("Feedback.Positive") != null)
            {
                return INTENT.FEEDBACK_POSITIVE;
            }

            return INTENT.NONE;
        }
        
    }
}
