using RaceDayMVC.DTOs;

namespace RaceDayMVC.ViewModels
{
    public class OrganiserDashboardViewModel
    {
        public string OrganisationName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int TotalEvents { get; set; }
        public int TotalEnrolments { get; set; }
        public List<EventSummary> Events { get; set; } = new();

        public class EventSummary
        {
            public int EventID { get; set; }
            public string EventName { get; set; } = string.Empty;
            public DateTime EventDate { get; set; }
            public string Location { get; set; } = string.Empty;
            public string EventStatus { get; set; } = string.Empty;
            public int EnrolmentCount { get; set; }
        }
    }
}