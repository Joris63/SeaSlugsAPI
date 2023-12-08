using SeaSlugAPI.Entities;
using SeaSlugAPI.Entities.DTOs;

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

        public static TrainingLogDTO TrainingLogToDTO(TrainingLog log)
        {
            return new TrainingLogDTO()
            {
                Id = log.Id,
                StartDate = log.StartDate,
                EndDate = log.EndDate,
                Status = log.Status,
                Error = log.Error
            };
        }
    }
}
