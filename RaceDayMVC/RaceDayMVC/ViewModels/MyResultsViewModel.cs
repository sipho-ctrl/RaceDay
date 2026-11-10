using RaceDayMVC.DTOs;

namespace RaceDayMVC.ViewModels
{
    public class MyResultsViewModel
    {
        public List<ResultResponseDto> Results { get; set; } = new();
        public int TotalRaces => Results.Count;
        public int ConfirmedResults => Results.Count(r => r.Status == "Confirmed");
    }
}