using Azure.Core;
using Azure.Storage.Blobs;
using SeaSlugAPI.Models;
using System.IO.Compression;

namespace SeaSlugAPI.Services
{
    public interface IBlobStorageService
    {
        Task<BlobStorageResponse> CreateContainer(string name);
        Task<BlobStorageResponse> UploadBlob(ValidatePredictionRequest model);
        Task<BlobStorageResponse<Stream>> RetrieveTrainingData();
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

            if (blobStorageConnectionString == string.Empty)
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
                return new BlobStorageResponse("Unable to upload image.");
            }

            try
            {
                // Get the blob service client
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient("validated-images");

                // Check if the container exists
                if (!await blobContainerClient.ExistsAsync())
                {
                    Console.Write("Container does not exist.");
                    return new BlobStorageResponse("Unable to upload image.");
                }

                // Query existing blobs to determine the next available integer label
                int nextLabel = await GetNextLabel(blobContainerClient, model.Id.ToString());

                // Use the next available integer as the label for the file name with its slug id as folder path
                string fileName = $"{model.Id}/{nextLabel}{Path.GetExtension(model.Image.FileName)}";

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

        public async Task<BlobStorageResponse<Stream>> RetrieveTrainingData()
        {
            // Get the blob storage endpoint
            string blobStorageConnectionString = _configuration["BlobStorageConnectionString"] ?? string.Empty;

            if (blobStorageConnectionString == string.Empty)
            {
                return new BlobStorageResponse<Stream>("Unable to upload image.");
            }

            try
            {
                // Get the blob service client
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient("validated-images");

                // Get all the blob items
                var blobItems = blobContainerClient.GetBlobsByHierarchy(delimiter: string.Empty);

                // Create a memory stream to store the compressed data
                using (var compressedStream = new MemoryStream())
                {
                    // Create a GZipStream for compressing data asynchronously
                    using (var gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal, true))
                    {
                        // Iterate over each blob and append its content to the compressed stream
                        foreach (var blobItem in blobItems)
                        {
                            var blobClient = blobContainerClient.GetBlobClient(blobItem.Blob.Name);
                            var blobDownloadInfo = await blobClient.DownloadAsync();

                            using (var blobStream = blobDownloadInfo.Value.Content)
                            {
                                await blobStream.CopyToAsync(gzipStream);
                                await gzipStream.FlushAsync();
                            }
                        }
                    }

                    // Rewind the compressed stream to the beginning
                    compressedStream.Position = 0;

                    return new BlobStorageResponse<Stream>(compressedStream, "Downloaded images successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new BlobStorageResponse<Stream>("Unable to upload image.");
            }
        }

        private async Task<int> GetNextLabel(BlobContainerClient containerClient, string folderPath)
        {
            var blobItems = containerClient.GetBlobsByHierarchy(delimiter: "/", prefix: folderPath);

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
