using SeaSlugAPI.Entities.DTOs;

namespace SeaSlugAPI.Models
{
    public class SeaSlugValidatedDataCount
    {
        public int ImageCount { get; set; }
        public SeaSlugDTO SeaSlug { get; set; }
    }

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

    public class BlobStorageResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public BlobStorageResponse(string message)
        {
            Message = message;
        }

        public BlobStorageResponse(T data, string message)
        {
            Data = data;
            Success = true;
            Message = message;
        }
    }
}

