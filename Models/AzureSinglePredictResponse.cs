using Newtonsoft.Json;

namespace SeaSlugAPI.Models
{
    public class AzureSinglePredictResponse
    {
        public string Data { get; set; } = string.Empty;
        public List<PredictionProbability> ParsedData => JsonConvert.DeserializeObject<List<PredictionProbability>>(Data) ?? new List<PredictionProbability>();
        public string Message { get; set; } = string.Empty;
    }

    public class PredictionProbability
    {
        public int Label { get; set; }
        public float Probability { get; set; }
    }
}