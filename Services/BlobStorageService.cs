using SeaSlugAPI.Models;

namespace SeaSlugAPI.Services
{
    public interface IBlobStorageService
    {
        Task<string> CreateContainer(string name);
        Task<string> UploadBlob(ValidatePredictionRequest model);
    }

    public class BlobStorageService : IBlobStorageService
    {
        public async Task<string> CreateContainer(string name)
        {
            return "Container created.";
        }

        public async Task<string> UploadBlob(ValidatePredictionRequest model)
        {
            return "Blob uploaded successfully.";
        }
    }
}
