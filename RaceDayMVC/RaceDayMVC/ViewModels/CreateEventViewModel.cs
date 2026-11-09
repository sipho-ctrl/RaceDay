using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.ViewModels
{
    public class CreateEventViewModel
    {
        [Required]
        [Display(Name = "Event Name")]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Today.AddDays(30);

        [Required]
        [Display(Name = "Location")]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Max Participants")]
        [Range(1, 100000)]
        public int? MaxParticipants { get; set; }

        [Display(Name = "Registration Deadline")]
        [DataType(DataType.Date)]
        public DateTime? RegistrationDeadline { get; set; }

        [Required]
        [Display(Name = "Distance")]
        [MaxLength(50)]
        public string Distance { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Event Type")]
        public string EventType { get; set; } = "run";

        [Display(Name = "Banner Image")]
        public IFormFile? BannerImage { get; set; }
    }
}