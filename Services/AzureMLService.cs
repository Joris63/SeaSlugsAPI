﻿using Newtonsoft.Json;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using System.Text;

namespace SeaSlugAPI.Services
{
    public interface IAzureMLService
    {
        Task<PredictionResults> Predict(PredictionRequest model);
        Task<List<PredictionResults>> PredictBatch(BatchPredictionRequest model);
        Task<string> GetTrainingStatus();
    }

    public class AzureMLService : IAzureMLService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private readonly ISeaSlugService _seaSlugService;

        public AzureMLService(IConfiguration configuration, ISeaSlugService seaSlugService)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _seaSlugService = seaSlugService;
        }

        public async Task<PredictionResults> Predict(PredictionRequest model)
        {
            // Convert the image provided to a base64 string
            string base64String = ImageHelper.ConvertImageToBase64(model.Image);
            // Get the realtime endpoint of the deployed model from secrets
            string endpoint = _configuration["ModelRTEndpoint"] ?? string.Empty;

            if(endpoint == string.Empty)
            {
                return new PredictionResults() { Message = "Could not get a prediction." };
            }

            try
            {
                // Serialize base64String and request prediction from the realtime endpoint
                var content = new StringContent(JsonConvert.SerializeObject(base64String), Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(endpoint, content);

                // Check the response status
                if (httpResponse.IsSuccessStatusCode)
                {
                    // Read and deserialize response content
                    string responseBody = await httpResponse.Content.ReadAsStringAsync();
                    RealtimeEndpointResponse? response = JsonConvert.DeserializeObject<RealtimeEndpointResponse>(responseBody) ?? null;

                    // Check the response content
                    if(response == null)
                    {
                        return new PredictionResults() { Message = "Could not get a prediction" };
                    }

                    List<SlugProbability> probabilities = new List<SlugProbability>();

                    foreach (ModelPredictionProbability probability in response.ParsedData)
                    {
                        SeaSlugDTO slug = _seaSlugService.Get(probability.Label);


                    }
                }
            }
            catch (Exception ex)
            {

            }

            return new PredictionResults();
        }

        public async Task<List<PredictionResults>> PredictBatch(BatchPredictionRequest model)
        {
            return new List<PredictionResults>();
        }

        public async Task<string> GetTrainingStatus()
        {
            return string.Empty;
        }
    }
}
