using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Organiser
    {
        [Key]
        public int OrganiserID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string OrganisationName { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string ContactNumber { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("UserID")]
        public User? User { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}