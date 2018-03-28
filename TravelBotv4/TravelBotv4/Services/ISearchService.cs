using System.Threading.Tasks;
using TravelBotv4.Services.Models;

namespace TravelBotv4.Services
{
    public interface ISearchService
    {
        Task<BaseSearchResult> SearchAsync(BaseSearchRequest req);
    }
}
