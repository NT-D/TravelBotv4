using System;
using System.IO;
using System.Threading.Tasks;
using TravelBotv4.Models;

namespace TravelBotv4.Services
{
    public class CustomVisionRecognizer : IImageRecognizer
    {
        public Task<IImageRecognizedResult> DetectImage(Stream imageStream, float threshold)
        {
            throw new NotImplementedException();
        }
    }
}
