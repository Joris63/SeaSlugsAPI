﻿namespace SeaSlugAPI.Models
{
    public class AddSeaSlugRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class RenameSeaSlugRequest
    {
        public Guid Id { get; set; }
        public string NewName { get; set; } = string.Empty;
    }

    public class ReorderSeaSlugsRequest
    {
        public List<Guid> OrderedSeaSlugIds { get; set; } = new List<Guid>();
    }

    public class SeaSlugServiceResults<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public SeaSlugServiceResults(string message)
        {   
            Message = message;
        }
        public SeaSlugServiceResults(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public SeaSlugServiceResults(T data, bool success, string message)
        {
            Data = data;
            Success = success;
            Message = message;
        }
    }
}
