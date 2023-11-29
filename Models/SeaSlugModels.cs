namespace SeaSlugAPI.Models
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

    public class ReorderedSeaSlug
    {
        public Guid Id { get; set; }
        public int Label { get; set; }
    }

    public class ReorderSeaSlugsRequest
    {
        public List<ReorderedSeaSlug> SeaSlugs = new List<ReorderedSeaSlug>();
    }
}
