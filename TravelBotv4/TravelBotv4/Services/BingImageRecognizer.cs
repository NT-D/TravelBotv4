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
            /*
             *You can use following API
             *https://docs.microsoft.com/en-us/rest/api/cognitiveservices/bing-images-api-v7-reference#modulesrequested
             * 
             * HTTP POST https://api.cognitive.microsoft.com/bing/v7.0/images/details?modules=ALL
             * 
             * Headers
             * Ocp-Apim-Subscription-Key : <Your key>
             * Content-Type : application/x-www-form-urlencoded
             * 
             * Body
             * Binary files
             * 
             */
            throw new NotImplementedException();
        }
    }
}
