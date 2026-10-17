namespace RaceDayAPI.DTOs
{
    public class EventResponseDto
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? MaxParticipants { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public string EventStatus { get; set; } = string.Empty;
        public string? Distance { get; set; }
        public string? EventType { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OrganiserID { get; set; }
        public string? OrganiserName { get; set; }
    }
}