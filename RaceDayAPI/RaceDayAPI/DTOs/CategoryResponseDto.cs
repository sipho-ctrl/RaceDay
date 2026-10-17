namespace RaceDayAPI.DTOs
{
    public class CategoryResponseDto
    {
        public int CategoryID { get; set; }
        public int EventID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string? AgeGroup { get; set; }
        public string? GenderGroup { get; set; }
        public decimal? EntryFee { get; set; }
        public int? MaxParticipants { get; set; }
        public string? EventName { get; set; }
        public int CurrentEnrolments { get; set; }
    }
}