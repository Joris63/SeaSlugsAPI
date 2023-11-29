using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface IPredictionService
    {
        Task<PredictionResults> Predict(PredictionRequest model);
        Task<List<PredictionResults>> PredictBatch(BatchPredictionRequest model);
    }

    public class PredictionService : IPredictionService
    {
        public async Task<PredictionResults> Predict(PredictionRequest model)
        {
            string message = "Could not identify sea slug species.";

            return new PredictionResults() { Message = message };
        }
        public async Task<List<PredictionResults>> PredictBatch(BatchPredictionRequest model)
        {
            return new List<PredictionResults>();
        }
    }
}
