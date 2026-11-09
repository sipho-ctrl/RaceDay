using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using RaceDayMVC.DTOs;

namespace RaceDayMVC.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContext;

        public ApiService(HttpClient http, IConfiguration config, IHttpContextAccessor httpContext)
        {
            _http = http;
            _config = config;
            _httpContext = httpContext;

            var baseUrl = _config["ApiSettings:BaseUrl"] ?? "https://localhost:7xxx";
            _http.BaseAddress = new Uri(baseUrl);
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string route, string? token = null, object? body = null)
        {
            var request = new HttpRequestMessage(method, route);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (body != null)
                request.Content = JsonContent.Create(body);
            return request;
        }

        private async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) return default;
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json)) return default;
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // AUTH
        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var request = CreateRequest(HttpMethod.Post, "/api/Auth/register", body: dto);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<AuthResponseDto>(response);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var request = CreateRequest(HttpMethod.Post, "/api/Auth/login", body: dto);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<AuthResponseDto>(response);
        }

        // USERS
        public async Task<UserProfileDto?> GetProfileAsync(string token)
        {
            var request = CreateRequest(HttpMethod.Get, "/api/Users/profile", token);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<UserProfileDto>(response);
        }

        public async Task<bool> UpdateProfileAsync(string token, object dto)
        {
            var request = CreateRequest(HttpMethod.Put, "/api/Users/profile", token, dto);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> UploadProfilePictureAsync(string token, IFormFile file)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Users/profile/upload-picture");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();
            var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);
            request.Content = content;

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("url", out var url) ? url.GetString() : null;
        }

        // EVENTS
        public async Task<List<EventResponseDto>> GetAllEventsAsync(string token)
        {
            var request = CreateRequest(HttpMethod.Get, "/api/Events", token);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<List<EventResponseDto>>(response) ?? new();
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(string token, int id)
        {
            var request = CreateRequest(HttpMethod.Get, $"/api/Events/{id}", token);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<EventResponseDto>(response);
        }

        public async Task<List<EventResponseDto>> GetMyEventsAsync(string token)
        {
            var request = CreateRequest(HttpMethod.Get, "/api/Events/organiser", token);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<List<EventResponseDto>>(response) ?? new();
        }

        public async Task<EventResponseDto?> CreateEventAsync(string token, object dto)
        {
            var request = CreateRequest(HttpMethod.Post, "/api/Events", token, dto);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<EventResponseDto>(response);
        }

        public async Task<bool> UpdateEventAsync(string token, int id, object dto)
        {
            var request = CreateRequest(HttpMethod.Put, $"/api/Events/{id}", token, dto);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEventAsync(string token, int id)
        {
            var request = CreateRequest(HttpMethod.Delete, $"/api/Events/{id}", token);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // CATEGORIES
        public async Task<List<CategoryResponseDto>> GetCategoriesByEventAsync(string token, int eventId)
        {
            var request = CreateRequest(HttpMethod.Get, $"/api/Categories/event/{eventId}", token);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<List<CategoryResponseDto>>(response) ?? new();
        }

        public async Task<CategoryResponseDto?> CreateCategoryAsync(string token, object dto)
        {
            var request = CreateRequest(HttpMethod.Post, "/api/Categories", token, dto);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<CategoryResponseDto>(response);
        }

        public async Task<bool> DeleteCategoryAsync(string token, int id)
        {
            var request = CreateRequest(HttpMethod.Delete, $"/api/Categories/{id}", token);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // ENROLMENTS
        public async Task<List<EnrolmentResponseDto>> GetMyEnrolmentsAsync(string token)
        {
            var request = CreateRequest(HttpMethod.Get, "/api/Enrolments", token);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<List<EnrolmentResponseDto>>(response) ?? new();
        }

        public async Task<List<EnrolmentResponseDto>> GetEnrolmentsByEventAsync(string token, int eventId)
        {
            var request = CreateRequest(HttpMethod.Get, $"/api/Enrolments/event/{eventId}", token);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<List<EnrolmentResponseDto>>(response) ?? new();
        }

        public async Task<EnrolmentResponseDto?> CreateEnrolmentAsync(string token, object dto)
        {
            var request = CreateRequest(HttpMethod.Post, "/api/Enrolments", token, dto);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<EnrolmentResponseDto>(response);
        }

        public async Task<bool> DeleteEnrolmentAsync(string token, int id)
        {
            var request = CreateRequest(HttpMethod.Delete, $"/api/Enrolments/{id}", token);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // RESULTS
        public async Task<List<ResultResponseDto>> GetMyResultsAsync(string token)
        {
            var request = CreateRequest(HttpMethod.Get, "/api/Results", token);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<List<ResultResponseDto>>(response) ?? new();
        }

        public async Task<List<ResultResponseDto>> GetResultsByEventAsync(string token, int eventId)
        {
            var request = CreateRequest(HttpMethod.Get, $"/api/Results/event/{eventId}", token);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<List<ResultResponseDto>>(response) ?? new();
        }

        public async Task<ResultResponseDto?> CreateResultAsync(string token, object dto)
        {
            var request = CreateRequest(HttpMethod.Post, "/api/Results", token, dto);
            var response = await _http.SendAsync(request);
            return await DeserializeAsync<ResultResponseDto>(response);
        }

        // FILE UPLOAD
        public async Task<string?> UploadEventBannerAsync(string token, IFormFile file)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Events/upload-banner");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new MultipartFormDataContent();
            var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);
            request.Content = content;

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("url", out var url) ? url.GetString() : null;
        }
    }
}