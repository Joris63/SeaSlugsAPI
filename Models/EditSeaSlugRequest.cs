namespace SeaSlugAPI.Models
{
    public class EditSeaSlugRequest
    {
        public Guid Id { get; set; }
        public int Label { get; set; } = -1;
        public string Name { get; set;} = string.Empty;
    }
}
