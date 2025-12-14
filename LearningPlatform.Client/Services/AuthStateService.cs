using LearningPlatform.Common.DTOs.Auth;
using LearningPlatform.Common.DTOs.Users;
using LearningPlatform.Common.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop.Implementation;

namespace LearningPlatform.Client.Services;

public class AuthStateService
{
    private AuthResponse? _authResponse;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthStateService>? _logger;
    private bool _isInitialized = false;

    public event Action? OnAuthStateChanged;
    
    public AuthStateService(NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }
    
    // Constructor with logger for dependency injection
    public AuthStateService(NavigationManager navigationManager, IJSRuntime jsRuntime, ILogger<AuthStateService> logger)
    {
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
        _logger = logger;
    }


    public bool IsAuthenticated => _authResponse != null && !string.IsNullOrEmpty(_authResponse.Token);
    
    public UserDto? CurrentUser => _authResponse?.User;
    
    public UserRole? CurrentRole => _authResponse?.Role;
    
    public bool IsInstructor => CurrentRole == UserRole.Instructor;
    
    public bool IsStudent => CurrentRole == UserRole.Student;

    public async Task InitializeAsync()
    {
        _logger?.LogInformation("[AuthStateService.InitializeAsync] Starting. _isInitialized: {IsInitialized}, _authResponse is null: {IsNull}, Has token: {HasToken}, Service instance: {InstanceId}", 
            _isInitialized, _authResponse == null, _authResponse?.Token != null, GetHashCode());
        
        // Since AuthStateService is now Singleton, state persists across navigation
        // But we should still verify from localStorage to ensure consistency
        var hasStateInMemory = _authResponse != null && !string.IsNullOrEmpty(_authResponse.Token);
        
        if (hasStateInMemory && _isInitialized)
        {
            _logger?.LogInformation("[AuthStateService.InitializeAsync] Already initialized with valid state in memory, will still verify from localStorage");
        }
        
        try
        {
            _logger?.LogInformation("[AuthStateService.InitializeAsync] Reading from localStorage");
            
            // Check if JSRuntime is available (not during prerendering)
            // In Blazor Server, JSRuntime throws InvalidOperationException during prerendering
            string? authDataJson = null;
            try
            {
                authDataJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authData");
                _logger?.LogInformation("[AuthStateService.InitializeAsync] localStorage.getItem returned. Has data: {HasData}, Length: {Length}", 
                    !string.IsNullOrEmpty(authDataJson), authDataJson?.Length ?? 0);
            }
            catch (InvalidOperationException jsEx) when (jsEx.Message.Contains("statically rendered") || jsEx.Message.Contains("prerendering"))
            {
                _logger?.LogWarning("[AuthStateService.InitializeAsync] InvalidOperationException - JSRuntime not ready (prerendering), keeping existing state. Message: {Message}", jsEx.Message);
                // During prerendering, we can't read from localStorage
                // Keep existing state if available, otherwise leave as null
                _isInitialized = true;
                return; // Exit early, don't update state
            }
            
            AuthResponse? newAuthResponse = null;
            
            if (!string.IsNullOrEmpty(authDataJson))
            {
                try
                {
                    newAuthResponse = JsonSerializer.Deserialize<AuthResponse>(authDataJson);
                    _logger?.LogInformation("[AuthStateService.InitializeAsync] Deserialized. Has response: {HasResponse}, Token length: {TokenLength}, Role: {Role}, User Email: {Email}", 
                        newAuthResponse != null, newAuthResponse?.Token?.Length ?? 0, newAuthResponse != null ? newAuthResponse.Role.ToString() : "null", newAuthResponse?.User?.Email ?? "null");
                    
                    if (newAuthResponse != null && string.IsNullOrEmpty(newAuthResponse.Token))
                    {
                        _logger?.LogWarning("[AuthStateService.InitializeAsync] Token is empty, setting response to null");
                        newAuthResponse = null; // Invalid token
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "[AuthStateService.InitializeAsync] Error deserializing authData: {Message}, StackTrace: {StackTrace}", 
                        ex.Message, ex.StackTrace);
                    newAuthResponse = null;
                }
            }
            
            // Compare tokens to detect actual changes (not just boolean state)
            var oldToken = _authResponse?.Token;
            var newToken = newAuthResponse?.Token;
            var tokenChanged = oldToken != newToken;
            
            _logger?.LogInformation("[AuthStateService.InitializeAsync] Token comparison. Old: {OldToken}, New: {NewToken}, Changed: {Changed}", 
                oldToken != null ? $"{oldToken.Substring(0, Math.Min(10, oldToken.Length))}..." : "null",
                newToken != null ? $"{newToken.Substring(0, Math.Min(10, newToken.Length))}..." : "null",
                tokenChanged);
            
            // Track if this is the first successful initialization
            bool wasFirstInit = !_isInitialized;
            
            // Always update state from localStorage if we got valid data
            // This ensures state is always in sync with localStorage
            if (newAuthResponse != null)
            {
                _authResponse = newAuthResponse;
                _logger?.LogInformation("[AuthStateService.InitializeAsync] State updated from localStorage");
            }
            else if (newAuthResponse == null && _authResponse != null)
            {
                // localStorage is empty but we have state in memory - clear it
                _logger?.LogInformation("[AuthStateService.InitializeAsync] localStorage is empty, clearing state");
                _authResponse = null;
                tokenChanged = true; // Mark as changed to trigger event
            }
            
            var isNowAuthenticated = IsAuthenticated;
            var currentRole = CurrentRole;
            var currentUser = CurrentUser;
            _logger?.LogInformation("[AuthStateService.InitializeAsync] Final state. IsAuthenticated: {IsAuth}, Role: {Role}, IsInstructor: {IsInstructor}, CurrentUser: {Email}, Token changed: {TokenChanged}, Will trigger event: {WillTrigger}", 
                isNowAuthenticated, 
                currentRole?.ToString() ?? "null", 
                IsInstructor, 
                currentUser?.Email ?? "null",
                tokenChanged,
                tokenChanged && _isInitialized);
            
            // Trigger event if:
            // 1. Token changed AND already initialized (normal case)
            // 2. OR we just successfully initialized for the first time (to notify other components like MainLayout)
            // This allows MainLayout to sync state even if it couldn't initialize during prerendering
            bool shouldTriggerEvent = (tokenChanged && _isInitialized) || 
                                     (wasFirstInit && isNowAuthenticated && newAuthResponse != null);
            
            if (shouldTriggerEvent)
            {
                var subscribersCount = OnAuthStateChanged?.GetInvocationList()?.Length ?? 0;
                _logger?.LogInformation("[AuthStateService.InitializeAsync] Triggering OnAuthStateChanged event. Subscribers count: {Count}, Reason: tokenChanged={TokenChanged}, _isInitialized={IsInit}, wasFirstInit={FirstInit}", 
                    subscribersCount, tokenChanged, _isInitialized, wasFirstInit);
                if (subscribersCount > 0)
                {
                    OnAuthStateChanged?.Invoke();
                    _logger?.LogInformation("[AuthStateService.InitializeAsync] OnAuthStateChanged event invoked");
                }
                else
                {
                    _logger?.LogWarning("[AuthStateService.InitializeAsync] OnAuthStateChanged has no subscribers!");
                }
            }
            else
            {
                _logger?.LogInformation("[AuthStateService.InitializeAsync] Not triggering event - tokenChanged: {TokenChanged}, _isInitialized: {IsInitialized}, isNowAuthenticated: {IsAuth}, newAuthResponse: {HasResponse}", 
                    tokenChanged, _isInitialized, isNowAuthenticated, newAuthResponse != null);
            }
        }
        catch (JSDisconnectedException ex)
        {
            _logger?.LogWarning(ex, "[AuthStateService.InitializeAsync] JSDisconnectedException - JSRuntime not ready, keeping existing state");
            // Keep existing state if JSRuntime is not ready
        }
        catch (InvalidOperationException ex)
        {
            _logger?.LogWarning(ex, "[AuthStateService.InitializeAsync] InvalidOperationException - JSRuntime not ready, keeping existing state. Message: {Message}", ex.Message);
            // Keep existing state if JSRuntime is not ready
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[AuthStateService.InitializeAsync] Error during initialization: {Message}, Type: {Type}, StackTrace: {StackTrace}", 
                ex.Message, ex.GetType().Name, ex.StackTrace);
            // Keep existing state on error
        }
        finally
        {
            _isInitialized = true;
            _logger?.LogInformation("[AuthStateService.InitializeAsync] Finished. _isInitialized set to true, IsAuthenticated: {IsAuth}, CurrentUser: {Email}", 
                IsAuthenticated, CurrentUser?.Email ?? "null");
        }
    }

    public async Task SetAuthResponse(AuthResponse? response)
    {
        _logger?.LogInformation("[AuthStateService.SetAuthResponse] Starting. Response is null: {IsNull}, Token: {Token}, Service instance: {InstanceId}", 
            response == null, response?.Token != null ? $"{response.Token.Substring(0, Math.Min(10, response.Token.Length))}..." : "null", GetHashCode());
        
        var wasAuthenticated = IsAuthenticated;
        var oldUser = CurrentUser?.Email;
        _authResponse = response;
        var newUser = CurrentUser?.Email;
        _logger?.LogInformation("[AuthStateService.SetAuthResponse] State updated in memory. Was authenticated: {WasAuth}, Now authenticated: {NowAuth}, Old user: {OldUser}, New user: {NewUser}", 
            wasAuthenticated, IsAuthenticated, oldUser ?? "null", newUser ?? "null");
        
        try
        {
            if (response != null)
            {
                _logger?.LogInformation("[AuthStateService.SetAuthResponse] Serializing response to JSON");
                var authDataJson = JsonSerializer.Serialize(response);
                _logger?.LogInformation("[AuthStateService.SetAuthResponse] JSON length: {Length}, Writing to localStorage", authDataJson.Length);
                
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authData", authDataJson);
                _logger?.LogInformation("[AuthStateService.SetAuthResponse] Data written to localStorage");
                
                // Ensure data is written before continuing
                // This is important for immediate redirects after login
                await Task.Delay(50);
                _logger?.LogInformation("[AuthStateService.SetAuthResponse] Delay completed");
            }
            else
            {
                _logger?.LogInformation("[AuthStateService.SetAuthResponse] Removing authData from localStorage");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authData");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[AuthStateService.SetAuthResponse] Error writing to localStorage");
        }
        
        // Always trigger state change event when setting auth response
        // This ensures UI updates after login/register
        var subscribersCount = OnAuthStateChanged?.GetInvocationList()?.Length ?? 0;
        _logger?.LogInformation("[AuthStateService.SetAuthResponse] Triggering OnAuthStateChanged event. Subscribers count: {Count}", subscribersCount);
        if (subscribersCount > 0)
        {
            OnAuthStateChanged?.Invoke();
            _logger?.LogInformation("[AuthStateService.SetAuthResponse] OnAuthStateChanged event invoked");
        }
        else
        {
            _logger?.LogWarning("[AuthStateService.SetAuthResponse] OnAuthStateChanged has no subscribers!");
        }
        _logger?.LogInformation("[AuthStateService.SetAuthResponse] Finished");
    }

    public async Task ClearAuth()
    {
        _logger?.LogInformation("[AuthStateService.ClearAuth] Starting");
        _authResponse = null;
        _isInitialized = false; // Reset initialization flag
        
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authData");
            _logger?.LogInformation("[AuthStateService.ClearAuth] Removed authData from localStorage");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "[AuthStateService.ClearAuth] Error removing from localStorage");
            // Ignore errors
        }
        
        var subscribersCount = OnAuthStateChanged?.GetInvocationList()?.Length ?? 0;
        _logger?.LogInformation("[AuthStateService.ClearAuth] Triggering OnAuthStateChanged event. Subscribers count: {Count}", subscribersCount);
        if (subscribersCount > 0)
        {
            OnAuthStateChanged?.Invoke();
            _logger?.LogInformation("[AuthStateService.ClearAuth] OnAuthStateChanged event invoked");
        }
        else
        {
            _logger?.LogWarning("[AuthStateService.ClearAuth] OnAuthStateChanged has no subscribers!");
        }
    }
    
    /// <summary>
    /// Clears all localStorage data (useful for development/debugging)
    /// </summary>
    public async Task ClearAllStorage()
    {
        _logger?.LogInformation("[AuthStateService.ClearAllStorage] Starting");
        _authResponse = null;
        _isInitialized = false;
        
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.clear");
            _logger?.LogInformation("[AuthStateService.ClearAllStorage] Cleared all localStorage");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "[AuthStateService.ClearAllStorage] Error clearing localStorage");
            // Ignore errors
        }
        
        var subscribersCount = OnAuthStateChanged?.GetInvocationList()?.Length ?? 0;
        _logger?.LogInformation("[AuthStateService.ClearAllStorage] Triggering OnAuthStateChanged event. Subscribers count: {Count}", subscribersCount);
        if (subscribersCount > 0)
        {
            OnAuthStateChanged?.Invoke();
            _logger?.LogInformation("[AuthStateService.ClearAllStorage] OnAuthStateChanged event invoked");
        }
        else
        {
            _logger?.LogWarning("[AuthStateService.ClearAllStorage] OnAuthStateChanged has no subscribers!");
        }
    }

    public string? GetToken()
    {
        return _authResponse?.Token;
    }
}

