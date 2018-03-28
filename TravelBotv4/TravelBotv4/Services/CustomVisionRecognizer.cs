using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TravelBotv4.Models;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.Cognitive.CustomVision.Prediction.Models;

namespace TravelBotv4.Services
{
    public class CustomVisionRecognizer : IImageRecognizer
    {
        const string projectId = "<Your project id>";
        //const string uriBase = "<Your uri base>";
        const string key = "<Your key>";
        public async Task<IImageRecognizedResult> DetectImage(Stream imageStream, float threshold)
        {
            using (var predictEndpoint = new PredictionEndpoint() { ApiKey = key })
            {
                try
                {
                    var result = await predictEndpoint.PredictImageAsync(new Guid(projectId), imageStream) as CustomVisionResult;
                    var predictedObject = result.Predictions.FirstOrDefault(obj => obj.Probability > threshold);
                    if (predictedObject != null) result.IsSure = true;
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw new Exception();
                }
            }

            //using (var httpClient = new HttpClient())
            //{
            //    httpClient.DefaultRequestHeaders.Add("Prediction-Key", key);
            //    var content = new StreamContent(imageStream);
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            //    var response = await httpClient.PostAsync(uriBase, content);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        string jsonResult = await response.Content.ReadAsStringAsync();
            //        var result = JsonConvert.DeserializeObject<CustomVisionResult>(jsonResult);
            //        if (result.Predictions.Any() && result.Predictions[0].Probability > threshold) result.IsSure = true;
            //        return result;
            //    }
            //    else
            //    {
            //        throw new Exception();
            //    }
            //}
        }
    }
}
