using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface IAzureService
    {
        Task<PredictionResponse> GetPrediction(string id);
    }

    public class AzureService : IAzureService
    {
        private readonly HttpClient httpClient;

        public AzureService()
        {
            httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer a6SmZE0hC0d6RscBwYzR0qJDuNTFPUN3");
        }

        public async Task<PredictionResponse> GetPrediction(string image)
        {
            string apiEndpoint = "https://sea-slug-tpije.germanywestcentral.inference.ml.azure.com/score";

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(image), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, content);

            // Check the response status
            if (response.IsSuccessStatusCode)
            {
                // Read and deserialize the response content
                string responseBody = await response.Content.ReadAsStringAsync();
                AzurePredictionResponse responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AzurePredictionResponse>(responseBody);

                SeaSlugDictionary dictionary = new SeaSlugDictionary();

                List<Probabilities> probabilities = new List<Probabilities>();

                foreach (PredictionProbabilities probability in responseObject.ParsedData)
                {
                    if(dictionary.Data.TryGetValue(probability.Label, out var slugName))
                    {
                        probabilities.Add(new Probabilities() { Label = slugName, Probability = (int)Math.Round(probability.Probability * 100) });
                    }
                }

                if (probabilities.Count > 0)
                {
                    return new PredictionResponse(responseObject.Message, probabilities);
                }
                else
                {
                    return new PredictionResponse(responseObject.Message);
                }
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                return new PredictionResponse(errorResponse);
            }
        }
    }
}