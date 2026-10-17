using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.DTOs
{
    public class UpdateProfileDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        // Participant fields
        [MaxLength(100)]
        public string? FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(15)]
        public string? ContactNumber { get; set; }

        [MaxLength(100)]
        public string? EmergencyContact { get; set; }

        // Organiser fields
        [MaxLength(100)]
        public string? OrganisationName { get; set; }
    }
}