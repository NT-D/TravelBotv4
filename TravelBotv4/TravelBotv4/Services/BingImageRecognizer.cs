using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TravelBotv4.Models;

namespace TravelBotv4.Services
{
    public class BingImageRecognizer : IImageRecognizer
    {
        public Task<IImageRecognizedResult> DetectImage(Stream imageStream, float threshold)
        {
            throw new NotImplementedException();
        }
    }
}
