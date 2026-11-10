using RaceDayMVC.DTOs;

namespace RaceDayMVC.ViewModels
{
    public class EventDetailViewModel
    {
        public EventResponseDto Event { get; set; } = new();
        public List<CategoryResponseDto> Categories { get; set; } = new();
        public List<int> AlreadyEnrolledCategoryIds { get; set; } = new();

        public bool IsEnrolledInCategory(int categoryId)
            => AlreadyEnrolledCategoryIds.Contains(categoryId);
    }
}