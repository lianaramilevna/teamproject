using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using LearningPlatform.Common.DTOs.Auth;

namespace LearningPlatform.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;
    private readonly AuthStateService _authStateService;

    public AuthService(HttpClient httpClient, ILogger<AuthService> logger, AuthStateService authStateService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authStateService = authStateService;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                _logger.LogInformation("Registration successful for {Email}", request.Email);
                if (result != null)
                {
                    await SetAuthResponse(result);
                }
                return result;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Registration failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
            
            // Throw exception for connection errors to be handled by the UI
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || 
                response.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)
            {
                throw new System.Net.Http.HttpRequestException($"API server is not available: {response.StatusCode}");
            }
            
            return null;
        }
        catch (System.Net.Http.HttpRequestException)
        {
            // Re-throw connection errors
            throw;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Registration timeout - API server may be unavailable");
            throw new System.Net.Http.HttpRequestException("Connection timeout. Please make sure the API server is running.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            throw new System.Net.Http.HttpRequestException($"Failed to connect to API: {ex.Message}", ex);
        }
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                _logger.LogInformation("Login successful for {Email}, Role: {Role}", request.Email, result?.Role);
                if (result != null)
                {
                    await SetAuthResponse(result);
                }
                return result;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Login failed: {StatusCode}, {Error}", response.StatusCode, errorContent);
            
            // Throw exception for connection errors to be handled by the UI
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || 
                response.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)
            {
                throw new System.Net.Http.HttpRequestException($"API server is not available: {response.StatusCode}");
            }
            
            return null;
        }
        catch (System.Net.Http.HttpRequestException)
        {
            // Re-throw connection errors
            throw;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Login timeout - API server may be unavailable");
            throw new System.Net.Http.HttpRequestException("Connection timeout. Please make sure the API server is running.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            throw new System.Net.Http.HttpRequestException($"Failed to connect to API: {ex.Message}", ex);
        }
    }

    public void SetToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task SetAuthResponse(AuthResponse response)
    {
        SetToken(response.Token);
        await _authStateService.SetAuthResponse(response);
    }

    public async Task ClearToken()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        await _authStateService.ClearAuth();
    }
}

