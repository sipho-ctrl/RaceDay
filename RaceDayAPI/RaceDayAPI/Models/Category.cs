using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        public int EventID { get; set; }

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

        // Navigation properties
        [ForeignKey("EventID")]
        public Event? Event { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}