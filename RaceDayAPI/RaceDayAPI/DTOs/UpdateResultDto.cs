using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class UpdateResultDto
    {
        [Required]
        public TimeSpan FinishTime { get; set; }

        public int? OverallPosition { get; set; }

        public int? CategoryPosition { get; set; }

        public decimal? Pace { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";
    }
}