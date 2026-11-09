using RaceDayMVC.DTOs;

namespace RaceDayMVC.ViewModels
{
    public class EventEnrolmentsViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string EventStatus { get; set; } = string.Empty;
        public List<EnrolmentResponseDto> Enrolments { get; set; } = new();

        public int TotalEnrolments => Enrolments.Count;
        public int PaidCount => Enrolments.Count(e => e.PaymentStatus == "Paid");
        public int PendingCount => Enrolments.Count(e => e.PaymentStatus == "Pending");
    }
}