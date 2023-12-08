namespace SeaSlugAPI.Entities.DTOs
{
    public class TrainingLogDTO
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TrainingStatus Status { get; set; }
        public string Error { get; set; }
    }
}