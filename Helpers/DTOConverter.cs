using SeaSlugAPI.Entities;
using SeaSlugAPI.Models;

namespace SeaSlugAPI.Helpers
{
    public class DTOConverter
    {
        public static SeaSlugDTO SeaSlugToDTO(SeaSlug slug)
        {
            return new SeaSlugDTO()
            {
                Id = slug.Id,
                Label = slug.Label,
                Name = slug.Name,
            };
        }
    }
}
