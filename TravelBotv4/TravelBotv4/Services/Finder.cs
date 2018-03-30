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
            NONE = 0,
            PLACES__FIND_PLACE,
            UTILITIES__SHOW_PREVIOUS,
            UTILITIES__SHOW_NEXT,
            COUNT
        };

        private static SpotsRequest previous_request = null;

        public async Task<BaseSearchResult> SearchWithKeywordAsync(string keyword)
        {
            return await SearchWithKeyword(keyword);
        }
        public async Task<BaseSearchResult> SearchAsync(string utterance)
        {
            return await Search(utterance);
        }
        
        public async Task<BaseSearchResult> SearchWithKeyword(string keyword)
        {
            try
            {
                var request = createSpotNameFieldRequest(keyword);
                previous_request = request;               
                return await searchSpotAsync(request); ;
            }
            catch (Exception)
            {
                return null;
            }
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
                        var request = createSpotRequest(entity);
                        previous_request = request;
                        result = await searchSpotAsync(request);
                        break;

                    case INTENT.UTILITIES__SHOW_PREVIOUS:
                        if (previous_request == null)
                        {
                            var new_request = createSpotRequest(entity);
                            previous_request = new_request;
                            result = await searchSpotAsync(new_request);
                        }
                        else
                        {
                            previous_request.ModifyForPrev();
                            result = await searchSpotAsync(previous_request);
                        }
                        break;

                    case INTENT.UTILITIES__SHOW_NEXT:
                        if (previous_request == null)
                        {
                            var new_request = createSpotRequest(entity);
                            previous_request = new_request;
                            result = await searchSpotAsync(new_request);
                        }
                        else
                        {
                            previous_request.ModifyForNext();
                            result = await searchSpotAsync(previous_request);
                        }
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
            if (luisResult.Intents.GetValue("Utilities.ShowPrevious") != null)
            {
                return INTENT.UTILITIES__SHOW_PREVIOUS;
            }
            if (luisResult.Intents.GetValue("Utilities.ShowNext") != null)
            {
                return INTENT.UTILITIES__SHOW_NEXT;
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
        private SpotsRequest createSpotRequest(string entity)
        {
            var request = new SpotsRequest();
            request.keyword = entity;
            return request;
        }
        private SpotsRequest createSpotNameFieldRequest(string keyword)
        {
            var request = new SpotsRequest();
            request.keyword = keyword;
            request.search_fields = "name";
            return request;
        }
        private async Task<SpotsResult> searchSpotAsync(SpotsRequest request)
        {
            var service = new Services.SpotSearchService();
            return await service.Search(request) as SpotsResult;
        }
    }
}
