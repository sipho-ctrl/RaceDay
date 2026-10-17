using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class CreateEnrolmentDto
    {
        [Required]
        public int CategoryID { get; set; }

        [MaxLength(20)]
        public string? PaymentStatus { get; set; } = "Pending";
    }
}