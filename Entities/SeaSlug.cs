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


        public SeaSlug(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            Label = -1;
        }
    }
}
