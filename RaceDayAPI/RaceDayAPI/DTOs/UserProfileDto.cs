namespace RaceDayAPI.DTOs
{
    public class UserProfileDto
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Participant fields
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? ContactNumber { get; set; }
        public string? EmergencyContact { get; set; }

        // Organiser fields
        public string? OrganisationName { get; set; }
    }
}