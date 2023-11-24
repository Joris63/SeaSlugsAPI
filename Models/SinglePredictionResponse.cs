namespace SeaSlugAPI.Models
{
    public class SinglePredictionResponse : BaseResponse 
    {
        public List<SlugProbability> Probabilities { get; set; } = new List<SlugProbability>();
    }
}