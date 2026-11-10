using RaceDayMVC.DTOs;

namespace RaceDayMVC.ViewModels
{
    public class EventResultsViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<EnrolmentResponseDto> Enrolments { get; set; } = new();
        public List<ResultResponseDto> ExistingResults { get; set; } = new();

        public bool HasResult(int enrolmentId)
            => ExistingResults.Any(r => r.EnrolmentID == enrolmentId);

        public ResultResponseDto? GetResult(int enrolmentId)
            => ExistingResults.FirstOrDefault(r => r.EnrolmentID == enrolmentId);
    }
}