using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface IAzureService
    {
        Task<BaseResponse> GetPrediction(IFormFile image);
    }

    public class AzureService : IAzureService
    {
        private readonly HttpClient httpClient;

        public AzureService()
        {
            httpClient = new HttpClient();

            // Get the authorization key from appsettings.json and add it to the http request header
            var authorizationKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Secrets")["AzureEndpointKey"];
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authorizationKey}");
        }

        public async Task<BaseResponse> GetPrediction(IFormFile image)
        {
            // Convert image uploaded to a base64 string
            string base64String = ImageHelper.ConvertImageToBase64(image);

            // Get the api endpoint from appsettings.json
            string apiEndpoint = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Secrets")["AzureEndpoint"];

            // Send API Post request containing the image base64 string 
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(image), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(apiEndpoint, content);

            // Check the response status
            if (response.IsSuccessStatusCode)
            {
                // Read and deserialize the response content
                string responseBody = await response.Content.ReadAsStringAsync();
                AzureSinglePredictResponse? responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureSinglePredictResponse>(responseBody);

                if (responseObject == null)
                {
                    return new BaseResponse() { Message = "Could not determine sea slug species" };
                }

                SeaSlugDictionary dictionary = new SeaSlugDictionary();
                List<SlugProbability> probabilities = new List<SlugProbability>();

                foreach (PredictionProbability probability in responseObject.ParsedData)
                {
                    if (dictionary.Data.TryGetValue(probability.Label, out var slugName))
                    {
                        probabilities.Add(new SlugProbability() { LabelNr= probability.Label, Label = slugName, Probability = (int)Math.Round(probability.Probability * 100) });
                    }
                }

                if (probabilities.Count > 0)
                {
                    return new SinglePredictionResponse() { Message = responseObject.Message, Probabilities = probabilities };
                }
                else
                {
                    return new BaseResponse() { Message = "Could not determine sea slug species" };
                }
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                return new BaseResponse() { Message = errorResponse };
            }
        }
    }
}