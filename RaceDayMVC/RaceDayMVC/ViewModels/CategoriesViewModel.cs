using RaceDayMVC.DTOs;

namespace RaceDayMVC.ViewModels
{
    public class CategoriesViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<CategoryResponseDto> Categories { get; set; } = new();
        public CreateCategoryViewModel NewCategory { get; set; } = new();
    }

    public class CreateCategoryViewModel
    {
        public int EventID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string? AgeGroup { get; set; }
        public string? GenderGroup { get; set; }
        public decimal? EntryFee { get; set; }
        public int? MaxParticipants { get; set; }
    }
}