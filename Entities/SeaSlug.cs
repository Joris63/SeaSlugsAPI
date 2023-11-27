using System.ComponentModel.DataAnnotations;

namespace SeaSlugAPI.Entities
{
    public class SeaSlug
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public int Label { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
