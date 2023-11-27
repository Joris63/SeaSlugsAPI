using Azure.Storage.Blobs;
using SeaSlugAPI.Helpers;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface IAzureService
    {
        Task<BaseResponse> GetPrediction(IFormFile image);
        Task<BaseResponse> UploadBlob(ValidatePredictionRequest request);
    }

    public class AzureService : IAzureService
    {
        private readonly HttpClient httpClient;

        public AzureService()
        {
            httpClient = new HttpClient();

            // Get the authorization key from appsettings.json and add it to the http request header
            var authorizationKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Secrets")["ModelEndpointKey"];
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authorizationKey}");
        }

        public async Task<BaseResponse> GetPrediction(IFormFile image)
        {
            // Convert image uploaded to a base64 string
            string base64String = ImageHelper.ConvertImageToBase64(image);

            // Get the api endpoint from appsettings.json
            string apiEndpoint = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Secrets")["ModelEndpoint"];

            // Send API Post request containing the image base64 string 
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(base64String), System.Text.Encoding.UTF8, "application/json");
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
                        probabilities.Add(new SlugProbability() { LabelNr = probability.Label, Label = slugName, Probability = (int)Math.Round(probability.Probability * 100) });
                    }
                }

                if (probabilities.Count > 0)
                {
                    return new SinglePredictionResponse() { Success = true, Message = responseObject.Message, Probabilities = probabilities };
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

        public async Task<BaseResponse> CreateContainer(string containerName)
        {
            // Get the api endpoint from appsettings.json
            string blobStorageConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Secrets")["BlobStorage"];

            try
            {
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                if (!await blobContainerClient.ExistsAsync())
                {
                    await blobContainerClient.CreateAsync();
                    return new BaseResponse() { Success = true, Message = "New container created." };
                }

                return new BaseResponse() { Message = "Container already exists." };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { Message = ex.Message };
            }
        }

        public async Task<BaseResponse> UploadBlob(ValidatePredictionRequest request)
        {
            // Get the api endpoint from appsettings.json
            string blobStorageConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Secrets")["BlobStorage"];

            try
            {
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(request.Label);

                // Query existing blobs to determine the next available integer label
                int nextLabel = await GetNextLabel(blobContainerClient);

                // Use the next available integer as the label for the file name
                string fileName = $"{nextLabel}{Path.GetExtension(request.Image.FileName)}";

                var blobClient = blobContainerClient.GetBlobClient(fileName);

                using (var stream = request.Image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                return new BaseResponse() { Success = true, Message = $"Image uploaded successfully!" };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { Message = ex.Message };
            }
        }

        private async Task<int> GetNextLabel(BlobContainerClient containerClient)
        {
            var blobItems = containerClient.GetBlobsByHierarchy(delimiter: string.Empty);

            if (blobItems == null || !blobItems.Any())
            {
                return 1; // If no blobs exist, start from 1
            }

            var labels = blobItems.Select(blobItem =>
            {
                var fileName = Path.GetFileNameWithoutExtension(blobItem.Blob.Name);
                return int.TryParse(fileName, out var label) ? label : 0;
            });

            return labels.Max() + 1; // Find the maximum label and increment
        }
    }
}