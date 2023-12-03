using Newtonsoft.Json;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;
using System.Net;
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
            // Get the realtime endpoint and key of the deployed model from secrets
            string endpoint = _configuration["ModelRTEndpoint"] ?? string.Empty;
            string endpointKey = _configuration["ModelRTEndpointKey"] ?? string.Empty;

            if (endpoint == string.Empty || endpointKey == string.Empty)
            {
                return new PredictionResults("Could not get a prediction.");
            }

            try
            {
                // Serialize base64String and request prediction from the realtime endpoint
                StringContent content = new StringContent(JsonConvert.SerializeObject(base64String), Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(endpoint),
                    Headers =
                    {
                        {HttpRequestHeader.Authorization.ToString(), $"Bearer {endpointKey}" }
                    },
                    Content = content
                };

                HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequestMessage);

                // Check the response status
                if (httpResponse.IsSuccessStatusCode)
                {
                    // Read and deserialize response content
                    string responseBody = await httpResponse.Content.ReadAsStringAsync();
                    RealtimeEndpointResponse? response = JsonConvert.DeserializeObject<RealtimeEndpointResponse>(responseBody) ?? null;

                    // Check the response content
                    if (response == null)
                    {
                        return new PredictionResults("Could not get a prediction.");
                    }

                    // Create an empty list of probabilities which we will then fill with the results gotten from the AI
                    List<SlugProbability> probabilities = new List<SlugProbability>();
                    foreach (ModelPredictionProbability probability in response.ParsedData)
                    {
                        // Retrieve the label with the label received from the prediction results
                        SeaSlugServiceResults<SeaSlugDTO> slugResponse = await _seaSlugService.GetByLabel(probability.Label);

                        if(!slugResponse.Success)
                        {
                            return new PredictionResults("Could not get a prediction.");
                        }

                        // Check value and add the slug probability
                        if (slugResponse.Data != null)
                        {
                            decimal percentage = (decimal)Math.Round(probability.Probability * 100, 2);
                            probabilities.Add(new SlugProbability() { Id = slugResponse.Data.Id, Name = slugResponse.Data.Name, Probability = percentage });
                        }
                    }

                    if(probabilities.Count > 0)
                    {
                        return new PredictionResults("Identified sea slug successfully.", probabilities);
                    }

                    return new PredictionResults("Could not get a prediction.", probabilities);
                }
                else
                {
                    return new PredictionResults("Could not get a prediction.");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new PredictionResults("Could not get a prediction.");
            }
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
