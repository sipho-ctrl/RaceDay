using RaceDayMVC.DTOs;

namespace RaceDayMVC.Services
{
    public interface IApiService
    {
        // Auth
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);

        // Users
        Task<UserProfileDto?> GetProfileAsync(string token);
        Task<bool> UpdateProfileAsync(string token, object dto);
        Task<string?> UploadProfilePictureAsync(string token, IFormFile file);

        // Events
        Task<List<EventResponseDto>> GetAllEventsAsync(string token);
        Task<EventResponseDto?> GetEventByIdAsync(string token, int id);
        Task<List<EventResponseDto>> GetMyEventsAsync(string token);
        Task<EventResponseDto?> CreateEventAsync(string token, object dto);
        Task<bool> UpdateEventAsync(string token, int id, object dto);
        Task<bool> DeleteEventAsync(string token, int id);

        // Categories
        Task<List<CategoryResponseDto>> GetCategoriesByEventAsync(string token, int eventId);
        Task<CategoryResponseDto?> CreateCategoryAsync(string token, object dto);
        Task<bool> DeleteCategoryAsync(string token, int id);

        // Enrolments
        Task<List<EnrolmentResponseDto>> GetMyEnrolmentsAsync(string token);
        Task<List<EnrolmentResponseDto>> GetEnrolmentsByEventAsync(string token, int eventId);
        Task<EnrolmentResponseDto?> CreateEnrolmentAsync(string token, object dto);
        Task<bool> DeleteEnrolmentAsync(string token, int id);

        // Results
        Task<List<ResultResponseDto>> GetMyResultsAsync(string token);
        Task<List<ResultResponseDto>> GetResultsByEventAsync(string token, int eventId);
        Task<ResultResponseDto?> CreateResultAsync(string token, object dto);
        Task<bool> UpdateResultAsync(string token, int id, object dto);

        // File upload
        Task<string?> UploadEventBannerAsync(string token, IFormFile file);
    }
}