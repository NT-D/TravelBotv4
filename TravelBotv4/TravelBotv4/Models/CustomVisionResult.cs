using Microsoft.Cognitive.CustomVision.Prediction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBotv4.Models
{
    public class CustomVisionResult : ImagePredictionResultModel, IImageRecognizedResult
    {
        public bool IsSure { get; set; }
    }
}
