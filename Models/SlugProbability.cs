namespace SeaSlugAPI.Models
{
    public class SlugProbability
    {
        public int LabelNr { get; set; } = 0;
        public string Label { get; set; } = string.Empty;
        public int Probability { get; set; } = 0;
    }
}