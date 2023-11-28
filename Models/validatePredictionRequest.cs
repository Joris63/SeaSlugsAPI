namespace SeaSlugAPI.Models
{
    public class ValidatePredictionRequest
    {
        public Guid Id { get; set; }
        public IFormFile Image { get; set; }
    }
}
