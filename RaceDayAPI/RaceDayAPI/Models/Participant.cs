using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(15)]
        public string? ContactNumber { get; set; }

        [MaxLength(100)]
        public string? EmergencyContact { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public User? User { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}