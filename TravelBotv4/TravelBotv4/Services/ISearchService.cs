using System.Threading.Tasks;
using TravelBotv4.Services.Model;

namespace TravelBotv4.Services
{
    public interface ISearchService
    {
        Task<BaseSearchResult> SearchAsync(BaseSearchRequest req);
    }
}
