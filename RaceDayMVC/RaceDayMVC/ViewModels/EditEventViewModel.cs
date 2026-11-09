using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.ViewModels
{
    public class EditEventViewModel
    {
        public int EventID { get; set; }

        [Required]
        [Display(Name = "Event Name")]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

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

        [Required]
        [Display(Name = "Event Status")]
        public string EventStatus { get; set; } = "Open";

        [Display(Name = "Banner Image")]
        public IFormFile? BannerImage { get; set; }
    }
}