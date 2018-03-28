using System.IO;
using System.Threading.Tasks;
using TravelBotv4.Models;

namespace TravelBotv4.Services
{
    public interface IImageRecognizer
    {
        Task<IImageRecognizedResult> DetectImage(Stream imageStream, float recognizeThreshold);
    }
}
