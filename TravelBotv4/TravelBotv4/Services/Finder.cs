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
    public class Finder 
    {
        private static string ModelId = "";
        private static string SubscriptionKey = "";
        private static string StreamUrl = "";

        private enum INTENT : UInt16 {
            PLACES__FIND_PLACE = 1,
            NONE
        };
 

        public async Task<BaseSearchResult> SearchAsync(string utterance)
        {
            return await Search(utterance);
        }

        public async Task<BaseSearchResult> Search(string utterance)
        {
            try
            {
                var luisResult = await find(utterance);
                var intent = primaryIntent(luisResult);
                var entity = primaryEntity(luisResult);

                SpotsResult result = null;
                switch (intent) {
                    case INTENT.PLACES__FIND_PLACE:
                        result = await searchSpotAsync(entity);
                        break;
                    default:
                        // nothing
                        break;
                }
         
                return result;
            }
            catch (Exception)
            {
                return null;
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
            if (luisResult.Intents.GetValue("Places.FindPlace") != null) {
                return INTENT.PLACES__FIND_PLACE;
            }

            return INTENT.NONE;
        }

        private string primaryEntity(RecognizerResult luisResult)
        {
            var entity = luisResult.Entities.GetValue("Places_AbsoluteLocation");
            if(entity != null && entity.First() != null) {
                return entity.First().ToString();
            }
            return null;
        }

        private async Task<SpotsResult> searchSpotAsync(string entity)
        {
            var service = new Services.SpotSearchService();
            var req = new SpotsRequest();
            req.keyword = entity;
            return await service.Search(req) as SpotsResult;
        }
    }
}
