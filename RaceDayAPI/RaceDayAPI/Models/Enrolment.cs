using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        public int? StartNumber { get; set; }

        // Navigation properties
        [ForeignKey("ParticipantID")]
        public Participant? Participant { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }

        public Result? Result { get; set; }
    }
}