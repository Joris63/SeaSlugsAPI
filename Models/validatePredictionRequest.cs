namespace SeaSlugAPI.Models
{
    public class ValidatePredictionRequest
    {
        public string Label { get; set; }
        public IFormFile Image { get; set; }
    }
}
