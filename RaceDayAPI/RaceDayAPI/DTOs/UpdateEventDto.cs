using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class UpdateEventDto
    {
        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? MaxParticipants { get; set; }

        public DateTime? RegistrationDeadline { get; set; }

        [MaxLength(50)]
        public string? Distance { get; set; }

        [MaxLength(20)]
        public string? EventType { get; set; }

        [MaxLength(20)]
        public string EventStatus { get; set; } = "Open";
    }
}