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

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer UEIJaOvA8DDFTYE5YrQ6aw5LsrOxuAKr");
        }

        public async Task<PredictionResponse> GetPrediction(string image)
        {
            string apiEndpoint = "https://sea-slug-prediction.germanywestcentral.inference.ml.azure.com/score";

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(image), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, content);

            // Check the response status
            if (response.IsSuccessStatusCode)
            {
                // Read and deserialize the response content
                string responseBody = await response.Content.ReadAsStringAsync();
                AzurePredictionResponse responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AzurePredictionResponse>(responseBody);

                SeaSlugDictionary dictionary = new SeaSlugDictionary();

                if (dictionary.Data.TryGetValue(responseObject.Data, out var slugName))
                {
                    return new PredictionResponse(true, slugName);
                }
                else
                {
                    return new PredictionResponse();
                }
            }
            else
            {
                return new PredictionResponse();
            }
        }
    }
}
