using Newtonsoft.Json;

namespace SeaSlugAPI.Models
{
    public class AzurePredictionResponse
    {
        public string Data { get; set; }
        public List<PredictionProbabilities> ParsedData => JsonConvert.DeserializeObject<List<PredictionProbabilities>>(Data);
        public string Message { get; set; }
    }

    public class PredictionProbabilities
    {
        public int Label { get; set; }
        public float Probability { get; set; }
    }
}