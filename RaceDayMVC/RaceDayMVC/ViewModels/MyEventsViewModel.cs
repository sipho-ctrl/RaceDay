using RaceDayMVC.DTOs;

namespace RaceDayMVC.ViewModels
{
    public class MyEventsViewModel
    {
        public List<EnrolmentResponseDto> Enrolments { get; set; } = new();
        public int ConfirmedCount => Enrolments.Count(e => e.PaymentStatus == "Paid");
        public int PendingCount => Enrolments.Count(e => e.PaymentStatus == "Pending");
    }
}