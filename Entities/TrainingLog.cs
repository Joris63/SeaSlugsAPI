using System.ComponentModel.DataAnnotations;

namespace SeaSlugAPI.Entities
{
    public class TrainingLog
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public TrainingStatus Status { get; set; }

        public string Error { get; set; }
    }

    public enum TrainingStatus
    {
        Starting,
        Training,
        Registering,
        Deploying,
        Failed, 
        Done
    }
}
