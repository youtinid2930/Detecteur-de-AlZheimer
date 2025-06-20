using System.ComponentModel.DataAnnotations;

namespace Alzheimer_Detection.Models
{
    public class PredictionResult
    {
        [Required]
        public string Class { get; set; } = "Unknown";

        public float Confidence { get; set; }

        [Required]
        public Dictionary<string, float> Probabilities { get; set; } = new Dictionary<string, float>();

        public int ClassIndex { get; set; }
    }
}
