namespace RaceDayAPI.DTOs
{
    public class EnrolmentResponseDto
    {
        public int EnrolmentID { get; set; }
        public int ParticipantID { get; set; }
        public string? ParticipantName { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public decimal CategoryDistance { get; set; }
        public int EventID { get; set; }
        public string? EventName { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public int? StartNumber { get; set; }
    }
}