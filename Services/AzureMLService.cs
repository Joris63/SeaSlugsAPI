using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface IAzureMLService
    {
        Task<RealtimeEndpointResponse> Predict(PredictionRequest model);
        Task<string> PredictBatch(BatchPredictionRequest model);
        Task<string> GetTrainingStatus();
    }

    public class AzureMLService : IAzureMLService
    {
        public async Task<RealtimeEndpointResponse> Predict(PredictionRequest model)
        {
            return new RealtimeEndpointResponse();
        }

        public async Task<string> PredictBatch(BatchPredictionRequest model)
        {
            return string.Empty;
        }

        public async Task<string> GetTrainingStatus()
        {
            return string.Empty;
        }
    }
}
