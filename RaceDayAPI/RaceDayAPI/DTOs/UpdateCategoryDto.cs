using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class UpdateCategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public decimal Distance { get; set; }

        [MaxLength(20)]
        public string? AgeGroup { get; set; }

        [MaxLength(10)]
        public string? GenderGroup { get; set; }

        public decimal? EntryFee { get; set; }

        public int? MaxParticipants { get; set; }
    }
}