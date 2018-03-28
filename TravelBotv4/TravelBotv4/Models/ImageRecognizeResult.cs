namespace TravelBotv4.Models
{
    public class ImageRecognizeResult
    {
        public ImageServiceType RecognizedServiceType { get; set; }
        public ComputerVisionResult ComputerVisionResult { get; set; }
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
    public class BingImageResult
    {
        public bool IsSure { get; set; }
        //reulst
    }
}
