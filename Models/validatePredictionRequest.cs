namespace SeaSlugAPI.Models
{
    public class validatePredictionRequest
    {
        public string Label { get; set; }
        public IFormFile Image { get; set; }
    }
}
