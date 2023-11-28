namespace SeaSlugAPI.Models
{
    public class SeaSlugResponse
    {
        public SeaSlugDTO? SeaSlug { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}