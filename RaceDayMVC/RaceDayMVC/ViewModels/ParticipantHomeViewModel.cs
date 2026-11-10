using RaceDayMVC.DTOs;

namespace RaceDayMVC.ViewModels
{
    public class ParticipantHomeViewModel
    {
        public List<EventResponseDto> Events { get; set; } = new();
        public string? EventTypeFilter { get; set; }
        public string? LocationFilter { get; set; }
        public string Username { get; set; } = string.Empty;
        public int MyEnrolmentCount { get; set; }
    }
}