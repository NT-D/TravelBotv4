using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TravelBotv4.Models;
using Newtonsoft.Json;

namespace TravelBotv4.Services
{
    public class ComputerVisionRecognizer : IImageRecognizer
    {
        const string uriBase = "https://eastasia.api.cognitive.microsoft.com/vision/v1.0/models/landmarks/analyze";
        const string subscriptionKey = "<Your subscription key>";
        public async Task<IImageRecognizedResult> DetectImage(Stream imageStream, float threshold)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                var content = new StreamContent(imageStream);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var response = await httpClient.PostAsync(uriBase, content);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ComputerVisionResult>(jsonResult);
                    if (result.Result.Landmarks.Any() && result.Result.Landmarks[0].Confidence > threshold) result.IsSure = true;
                    return result;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

    }
}
