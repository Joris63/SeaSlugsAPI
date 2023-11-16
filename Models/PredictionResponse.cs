namespace SeaSlugAPI.Models
{
    public class PredictionResponse
    {
        public bool Success { get; private set; }
        public string SlugName { get; private set; }

        public PredictionResponse()
        {
            Success = false;
            SlugName = "Not Found!";
        }

        public PredictionResponse(bool success, string name)
        {
            Success = success;
            SlugName = name;
        }
    }
}
