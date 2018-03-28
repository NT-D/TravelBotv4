namespace TravelBotv4.Models
{
    public class ComputerVisionResult : IImageRecognizedResult
    {
        public bool IsSure { get; set; }
        public Result Result { get; set; }
        public string RequestId { get; set; }
    }

    public class Result
    {
        public Landmark[] Landmarks { get; set; }
    }

    public class Landmark
    {
        public string Name { get; set; }
        public float Confidence { get; set; }
    }
}