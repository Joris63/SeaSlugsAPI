using Azure.Core;
using Azure.Storage.Blobs;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface IBlobStorageService
    {
        Task<BlobStorageResponse> CreateContainer(string name);
        Task<BlobStorageResponse> UploadBlob(ValidatePredictionRequest model);
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<BlobStorageResponse> CreateContainer(string name)
        {
            // Get the blob storage endpoint
            string blobStorageConnectionString = _configuration["BlobStorageConnectionString"] ?? string.Empty;

            if(blobStorageConnectionString == string.Empty)
            {
                return new BlobStorageResponse("Could not create container.");
            }

            try
            {
                // Get the blob service client
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(name);

                // Check if a container already exists with the same name
                if (!await blobContainerClient.ExistsAsync())
                {
                    // Add the new container
                    await blobContainerClient.CreateAsync();
                    return new BlobStorageResponse(true, "Successfully created container.");
                }

                return new BlobStorageResponse(false, "Container already exists.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new BlobStorageResponse("Could not create container.");
            }
        }

        public async Task<BlobStorageResponse> UploadBlob(ValidatePredictionRequest model)
        {
            // Get the blob storage endpoint
            string blobStorageConnectionString = _configuration["BlobStorageConnectionString"] ?? string.Empty;

            if (blobStorageConnectionString == string.Empty)
            {
                return new BlobStorageResponse("Could not create container.");
            }

            try
            {
                // Get the blob service client
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(model.Id.ToString());

                // Query existing blobs to determine the next available integer label
                int nextLabel = await GetNextLabel(blobContainerClient);

                // Use the next available integer as the label for the file name
                string fileName = $"{nextLabel}{Path.GetExtension(model.Image.FileName)}";

                var blobClient = blobContainerClient.GetBlobClient(fileName);

                using (var stream = model.Image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                return new BlobStorageResponse(true, "Image uploaded successfully!");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new BlobStorageResponse("Unable to upload image.");
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
