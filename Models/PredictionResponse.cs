namespace SeaSlugAPI.Models
{
    public class PredictionResponse
    {
        public List<SlugProbability> Probabilities { get; set; } = new List<SlugProbability>();
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}