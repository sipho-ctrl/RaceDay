namespace RaceDayAPI.DTOs
{
    public class ResultResponseDto
    {
        public int ResultID { get; set; }
        public int EnrolmentID { get; set; }
        public int ParticipantID { get; set; }
        public string? ParticipantName { get; set; }
        public int EventID { get; set; }
        public string? EventName { get; set; }
        public DateTime EventDate { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public decimal CategoryDistance { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int? OverallPosition { get; set; }
        public int? CategoryPosition { get; set; }
        public decimal? Pace { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RecordedDate { get; set; }
    }
}