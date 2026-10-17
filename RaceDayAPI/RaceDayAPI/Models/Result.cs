using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        public int EnrolmentID { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; }

        public int? OverallPosition { get; set; }

        public int? CategoryPosition { get; set; }

        public decimal? Pace { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime RecordedDate { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("EnrolmentID")]
        public Enrolment? Enrolment { get; set; }
    }
}