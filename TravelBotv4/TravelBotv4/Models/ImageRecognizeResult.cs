using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBotv4.Models
{
    public class ImageRecognizeResult
    {
        public ImageServiceType RecognizedServiceType { get; set; }
        public ComputerVisionResult GetComputerVisionResult { get; set; }
        public CustomVisionResult CustomVisionResult { get; set; }
        public BingImageResult BingImageResult { get; set; }
    }

    public enum ImageServiceType
    {
        ComputerVisionService,
        CustomVisionService,
        BingImageSearch
    }

    //TODO: Separate file per class after research and define
    public class ComputerVisionResult
    {
        public bool isSure { get; set; }
        //reulst
    }

    public class CustomVisionResult
    {
        public bool isSure { get; set; }
        //reulst
    }

    public class BingImageResult
    {
        public bool isSure { get; set; }
        //reulst
    }
}
