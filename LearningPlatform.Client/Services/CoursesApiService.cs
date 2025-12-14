using System.Net.Http.Json;
using System.Net.Http.Headers;
using LearningPlatform.Common.DTOs.Courses;
using LearningPlatform.Common.DTOs.Users;

namespace LearningPlatform.Client.Services;

public class CoursesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoursesApiService> _logger;
    private readonly AuthStateService _authStateService;

    public CoursesApiService(HttpClient httpClient, ILogger<CoursesApiService> logger, AuthStateService authStateService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authStateService = authStateService;
    }

    private void SetAuthorizationHeader()
    {
        var token = _authStateService.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<List<CourseDto>?> GetCoursesAsync()
    {
        try
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<CourseDto>>("/api/courses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching courses");
            return null;
        }
    }

    public async Task<CourseDto?> CreateCourseAsync(CreateCourseRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("/api/courses", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CourseDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Create course failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course");
            return null;
        }
    }

    public async Task<CourseDto?> GetCourseByIdAsync(Guid courseId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"/api/courses/{courseId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CourseDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching course by ID");
            return null;
        }
    }

    public async Task<CourseDto?> UpdateCourseAsync(Guid courseId, UpdateCourseRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"/api/courses/{courseId}", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CourseDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Update course failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new Exception(errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course");
            throw;
        }
    }

    public async Task<bool> DeleteCourseAsync(Guid courseId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"/api/courses/{courseId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course");
            return false;
        }
    }

    public async Task<List<UserDto>?> GetCourseStudentsAsync(Guid courseId)
    {
        try
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<UserDto>>($"/api/courses/{courseId}/students");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching course students");
            return null;
        }
    }

    public async Task<CourseDto?> JoinCourseAsync(JoinCourseRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("/api/courses/join", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CourseDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Join course failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new Exception(errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining course");
            throw;
        }
    }
}


