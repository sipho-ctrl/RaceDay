using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public int OrganiserID { get; set; }

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

        [MaxLength(20)]
        public string EventStatus { get; set; } = "Open";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Part 2 requirements
        [MaxLength(50)]
        public string? Distance { get; set; }

        [MaxLength(20)]
        public string? EventType { get; set; } // run, walk, or cycle

        // Navigation properties
        [ForeignKey("OrganiserID")]
        public Organiser? Organiser { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}