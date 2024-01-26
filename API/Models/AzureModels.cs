using Newtonsoft.Json;

namespace SeaSlugAPI.Models
{
    public class ModelPredictionProbability
    {
        public int Label { get; set; }
        public float Probability { get; set; }
    }

    public class RealtimeEndpointResponse
    {
        public string Data { get; set; } = string.Empty;
        public List<ModelPredictionProbability> ParsedData => JsonConvert.DeserializeObject<List<ModelPredictionProbability>>(Data) ?? new List<ModelPredictionProbability>();
        public string Message { get; set; } = string.Empty;
    }

    public class TrainingResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public TrainingResponse(string message)
        {
            Message = message;
        }

        public TrainingResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
