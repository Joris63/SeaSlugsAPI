namespace SeaSlugAPI.Models
{
    public class PredictionResponse
    {
        public string Message { get; private set; }
        public List<Probabilities> Probabilities { get; private set; }

        public PredictionResponse(string message)
        {
            Message = message;
            Probabilities = new List<Probabilities>();
        }

        public PredictionResponse(string message, List<Probabilities> probabilities)
        {
            Message = message;
            Probabilities = probabilities;
        }
    }

    public class Probabilities
    {
        public string Label { get; set; }
        public int Probability { get; set; }
    }
}