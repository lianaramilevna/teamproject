using System.Net.Http.Json;
using System.Net.Http.Headers;
using LearningPlatform.Common.DTOs.Teams;

namespace LearningPlatform.Client.Services;

public class TeamsApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TeamsApiService> _logger;
    private readonly AuthStateService _authStateService;

    public TeamsApiService(HttpClient httpClient, ILogger<TeamsApiService> logger, AuthStateService authStateService)
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

    public async Task<List<TeamDto>?> GetTeamsAsync(Guid courseId)
    {
        try
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<TeamDto>>($"/api/courses/{courseId}/teams");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching teams");
            return null;
        }
    }

    public async Task<TeamDto?> CreateTeamAsync(Guid courseId, CreateTeamRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync($"/api/courses/{courseId}/teams", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TeamDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Create team failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new Exception(errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team");
            throw;
        }
    }

    public async Task<TeamDto?> UpdateTeamAsync(Guid courseId, Guid teamId, UpdateTeamRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"/api/courses/{courseId}/teams/{teamId}", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TeamDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Update team failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new Exception(errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team");
            throw;
        }
    }

    public async Task<bool> DeleteTeamAsync(Guid courseId, Guid teamId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"/api/courses/{courseId}/teams/{teamId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting team");
            return false;
        }
    }

    public async Task<TeamDto?> AddTeamMemberAsync(Guid courseId, Guid teamId, AddTeamMemberRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync($"/api/courses/{courseId}/teams/{teamId}/members", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TeamDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Add team member failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new Exception(errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding team member");
            throw;
        }
    }

    public async Task<TeamDto?> RemoveTeamMemberAsync(Guid courseId, Guid teamId, Guid studentId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"/api/courses/{courseId}/teams/{teamId}/members/{studentId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TeamDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Remove team member failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new Exception(errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing team member");
            throw;
        }
    }
}

