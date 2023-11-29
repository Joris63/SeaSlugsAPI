namespace SeaSlugAPI.Models
{
    public class PredictionRequest
    {
        public IFormFile Image { get; set; }
    }
    public class BatchPredictionRequest
    {
        public List<IFormFile> Images { get; set; }
    }

    public class ValidatePredictionRequest
    {
        public Guid Id { get; set; }
        public IFormFile Image { get; set; }
    }

    public class SlugProbability
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Probability { get; set; } = 0;
    }

    public class PredictionResults
    {
        public List<SlugProbability> Probabilities { get; set; } = new List<SlugProbability>();
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
