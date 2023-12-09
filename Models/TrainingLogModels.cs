using SeaSlugAPI.Entities;
using SeaSlugAPI.Entities.DTOs;

namespace SeaSlugAPI.Models
{
    public class EditTrainingLogRequest
    {
        public Guid Id { get; set; }
        public TrainingStatus Status { get; set; }
        public string Error {  get; set; } = string.Empty;
    }

    public class TrainingLogServiceResults
    {
        public TrainingLogDTO? Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public TrainingLogServiceResults(string message)
        {
            Message = message;
        }

        public TrainingLogServiceResults(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public TrainingLogServiceResults(TrainingLogDTO data, bool success, string message)
        {
            Data = data;
            Success = success;
            Message = message;
        }
    }
}
