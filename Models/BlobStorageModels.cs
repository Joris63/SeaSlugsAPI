namespace SeaSlugAPI.Models
{
    public class BlobStorageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public BlobStorageResponse(string error)
        {
            Error = error;
        }

        public BlobStorageResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}

