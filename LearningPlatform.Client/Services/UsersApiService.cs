using System.Net.Http.Json;
using System.Net.Http.Headers;
using LearningPlatform.Common.DTOs.Users;

namespace LearningPlatform.Client.Services;

public class UsersApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersApiService> _logger;
    private readonly AuthStateService _authStateService;

    public UsersApiService(HttpClient httpClient, ILogger<UsersApiService> logger, AuthStateService authStateService)
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

    public async Task<UserDto?> GetProfileAsync()
    {
        try
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<UserDto>("/api/users/profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching profile");
            return null;
        }
    }

    public async Task<UserDto?> UpdateProfileAsync(UpdateProfileRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync("/api/users/profile", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Update profile failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
                throw new Exception(errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            throw;
        }
    }
}

