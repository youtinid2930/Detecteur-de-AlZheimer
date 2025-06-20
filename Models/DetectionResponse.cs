namespace Alzheimer_Detection.Models
{
    public class DetectionResponse
    {
        public required string FileName { get; set; } // Ajout de 'required'

        public required PredictionResult Prediction { get; set; } // Ajout de 'required'
    }
}
