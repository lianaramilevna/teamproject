using System.Net.Http.Json;
using System.Net.Http.Headers;
using LearningPlatform.Common.DTOs.Assignments;

namespace LearningPlatform.Client.Services;

public class AssignmentsApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AssignmentsApiService> _logger;
    private readonly AuthStateService _authStateService;

    public AssignmentsApiService(HttpClient httpClient, ILogger<AssignmentsApiService> logger, AuthStateService authStateService)
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

    public async Task<List<AssignmentDto>?> GetAssignmentsAsync(Guid courseId)
    {
        try
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<AssignmentDto>>($"/api/courses/{courseId}/assignments");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assignments");
            return null;
        }
    }

    public async Task<AssignmentDto?> GetAssignmentByIdAsync(Guid courseId, Guid assignmentId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"/api/courses/{courseId}/assignments/{assignmentId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AssignmentDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assignment by ID");
            return null;
        }
    }

    public async Task<AssignmentDto?> CreateAssignmentAsync(Guid courseId, CreateAssignmentRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync($"/api/courses/{courseId}/assignments", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AssignmentDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Create assignment failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new Exception(errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assignment");
            throw;
        }
    }

    public async Task<AssignmentDto?> UpdateAssignmentAsync(Guid courseId, Guid assignmentId, UpdateAssignmentRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"/api/courses/{courseId}/assignments/{assignmentId}", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AssignmentDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Update assignment failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new Exception(errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assignment");
            throw;
        }
    }

    public async Task<bool> DeleteAssignmentAsync(Guid courseId, Guid assignmentId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"/api/courses/{courseId}/assignments/{assignmentId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assignment");
            return false;
        }
    }
}


