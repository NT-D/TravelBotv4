namespace TravelBotv4.Models
{
    public class ImageRecognizeResult
    {
        public ImageServiceType RecognizedServiceType { get; set; }
        public ComputerVisionResult ComputerVisionResult { get; set; }
        public CustomVisionResult CustomVisionResult { get; set; }
        public BingImageResult BingImageResult { get; set; }

        public string PrimaryKeyword()
        {
            string keyword = null;
            if (this.RecognizedServiceType == ImageServiceType.ComputerVisionService)
            {
                keyword = this.ComputerVisionResult.Result.Landmarks?[0].Name;
            }
            else if (this.RecognizedServiceType == ImageServiceType.ComputerVisionService)
            {
                keyword = this.ComputerVisionResult.Result.Landmarks?[0].Name;
            }
            return keyword;
        }

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
