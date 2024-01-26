using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SeaSlugAPI.Entities.DTOs;
using SeaSlugAPI.Models;
using System.IO.Compression;

namespace SeaSlugAPI.Services
{
    public interface IBlobStorageService
    {
        Task<BlobStorageResponse> CreateContainer(string name);
        Task<BlobStorageResponse> UploadBlob(ValidatePredictionRequest model);
        Task<BlobStorageResponse<List<ValidatedDataCount>>> RetrieveTrainingDataCount();
        Task<BlobStorageResponse<List<string>>> RetrieveRetrainingData();
        Task<BlobStorageResponse<List<string>>> RetrieveTrainingData();
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ISeaSlugService _seaSlugService;

        public BlobStorageService(IConfiguration configuration, ISeaSlugService seaSlugService)
        {
            _configuration = configuration;
            _seaSlugService = seaSlugService;
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
                int nextLabel = await GetBlobCount(blobContainerClient, model.Id.ToString()) + 1;

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

        public async Task<BlobStorageResponse<List<ValidatedDataCount>>> RetrieveTrainingDataCount()
        {
            // Get the blob storage endpoint
            string blobStorageConnectionString = _configuration["BlobStorageConnectionString"] ?? string.Empty;

            if (blobStorageConnectionString == string.Empty)
            {
                return new BlobStorageResponse<List<ValidatedDataCount>>("Unable to retrieve training data count.");
            }

            try
            {
                // Get the blob service client
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient("validated-images");

                // Get all sea slug species
                SeaSlugServiceResults<List<SeaSlugDTO>> response = await _seaSlugService.GetAll();

                // Check response
                if (!response.Success || response.Data == null)
                {
                    return new BlobStorageResponse<List<ValidatedDataCount>>("Unable to retrieve training data count.");
                }

                List<ValidatedDataCount> validatedDataCount = new List<ValidatedDataCount>();

                foreach (SeaSlugDTO seaSlug in response.Data)
                {
                    // Query existing blobs to determine the total images count for the slug species
                    int imageCount = await GetBlobCount(blobContainerClient, seaSlug.Id.ToString());
                    if(imageCount > 0)
                    {
                        validatedDataCount.Add(new ValidatedDataCount() { ImageCount = imageCount, SeaSlug = seaSlug });
                    }
                }

                // Check if there were any images
                if (validatedDataCount.Count < 1)
                {
                    return new BlobStorageResponse<List<ValidatedDataCount>>(validatedDataCount, "No images found for any of the species.");
                }

                return new BlobStorageResponse<List<ValidatedDataCount>>(validatedDataCount, "Successfully retrieved data count.");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new BlobStorageResponse<List<ValidatedDataCount>>("Unable to retrieve training data count.");
            }
        }

        public async Task<BlobStorageResponse<List<string>>> RetrieveTrainingData()
        {
            // Get the blob storage endpoint
            string blobStorageConnectionString = _configuration["BlobStorageConnectionString"] ?? string.Empty;

            if (blobStorageConnectionString == string.Empty)
            {
                return new BlobStorageResponse<List<string>>("Unable to retrieve data.");
            }

            try
            {

                List<string> blobUrls = new List<string>();

                // Get the blob service client
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient("validated-images");

                var blobItems = blobContainerClient.GetBlobsByHierarchy(delimiter: string.Empty);

                foreach (BlobHierarchyItem blobHierarchyItem in blobItems)
                {
                    string blobPath = blobHierarchyItem.Blob.Name; // Full path of the blob with folders

                    // Adjust this according to your folder structure
                    // Extract the species folder (assuming 'species' is part of the folder structure)
                    string speciesFolder = GetSpeciesFolder(blobPath);

                    Uri blobUri = blobContainerClient.GetBlobClient(blobPath).Uri;
                    blobUrls.Add(blobUri.ToString());
                }

                return new BlobStorageResponse<List<string>>(blobUrls, "Retrieved images successfully."); ;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new BlobStorageResponse<List<string>>("Unable to retrieve data.");
            }
        }

        public async Task<BlobStorageResponse<List<string>>> RetrieveRetrainingData()
        {
            // Get the blob storage endpoint
            string blobStorageConnectionString = _configuration["BlobStorageConnectionString"] ?? string.Empty;

            if (blobStorageConnectionString == string.Empty)
            {
                return new BlobStorageResponse<List<string>>("Unable to retrieve data.");
            }

            try
            {

                List<string> blobUrls = new List<string>();

                // Get the blob service client
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient("slug-images");

                var blobItems = blobContainerClient.GetBlobsByHierarchy(delimiter: string.Empty);

                foreach (BlobHierarchyItem blobHierarchyItem in blobItems)
                {
                    string blobPath = blobHierarchyItem.Blob.Name; // Full path of the blob with folders

                    // Adjust this according to your folder structure
                    // Extract the species folder (assuming 'species' is part of the folder structure)
                    string speciesFolder = GetSpeciesFolder(blobPath);

                    Uri blobUri = blobContainerClient.GetBlobClient(blobPath).Uri;
                    blobUrls.Add(blobUri.ToString());
                }

                return new BlobStorageResponse<List<string>>(blobUrls, "Retrieved images successfully."); ;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new BlobStorageResponse<List<string>>("Unable to retrieve data.");
            }
        }

        private async Task<int> GetBlobCount(BlobContainerClient containerClient, string folderPath)
        {
            var blobItems = containerClient.GetBlobsByHierarchy(delimiter: string.Empty, prefix: $"{folderPath}/");

            // Directly check if there are any blob items
            if (!blobItems.Any())
            {
                return 0; // If no blobs exist
            }

            // Count the blobs without materializing the list
            var count = blobItems.Count();

            return count;
        }

        private string GetSpeciesFolder(string blobPath)
        {
            // Adjust this logic according to your folder structure
            string[] pathSegments = blobPath.Split('/'); // Assuming folders are separated by '/'

            // Find and return the species folder
            foreach (string segment in pathSegments)
            {
                if (segment.ToLower().Contains("species")) // Adjust this condition based on your folder structure
                {
                    return segment;
                }
            }

            return string.Empty; // Or handle if species folder is not found
        }
    }
}
