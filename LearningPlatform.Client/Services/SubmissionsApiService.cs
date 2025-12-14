using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using LearningPlatform.Common.DTOs.Submissions;

namespace LearningPlatform.Client.Services;

public class SubmissionsApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SubmissionsApiService> _logger;
    private readonly AuthStateService _authStateService;

    public SubmissionsApiService(HttpClient httpClient, ILogger<SubmissionsApiService> logger, AuthStateService authStateService)
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

    public async Task<SubmissionDto?> SubmitAssignmentAsync(SubmitAssignmentRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            _logger.LogInformation("Submitting assignment. AssignmentId: {AssignmentId}, Link: {Link}", request.AssignmentId, request.Link);
            var response = await _httpClient.PostAsJsonAsync("/api/submissions", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SubmissionDto>();
                _logger.LogInformation("Assignment submitted successfully. SubmissionId: {SubmissionId}", result?.Id);
                return result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to submit assignment. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, errorContent);
                
                // Try to extract error message from response
                string errorMessage = "Failed to submit assignment.";
                try
                {
                    var errorJson = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    if (errorJson.TryGetProperty("message", out var messageElement))
                    {
                        errorMessage = messageElement.GetString() ?? errorMessage;
                    }
                    else if (errorJson.TryGetProperty("title", out var titleElement))
                    {
                        errorMessage = titleElement.GetString() ?? errorMessage;
                    }
                }
                catch
                {
                    // If parsing fails, use default message
                }
                
                throw new HttpRequestException($"{(int)response.StatusCode} {response.StatusCode}: {errorMessage}");
            }
        }
        catch (HttpRequestException)
        {
            // Re-throw HTTP errors with message
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting assignment. AssignmentId: {AssignmentId}", request.AssignmentId);
            throw new HttpRequestException($"Failed to submit assignment: {ex.Message}", ex);
        }
    }

    public async Task<List<SubmissionDto>?> GetSubmissionsAsync(Guid assignmentId)
    {
        try
        {
            SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<SubmissionDto>>($"/api/assignments/{assignmentId}/submissions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching submissions");
            return null;
        }
    }

    public async Task<SubmissionDto?> GradeSubmissionAsync(Guid submissionId, GradeSubmissionRequest request)
    {
        try
        {
            SetAuthorizationHeader();
            _logger.LogInformation("Grading submission. SubmissionId: {SubmissionId}, Grade: {Grade}", submissionId, request.Grade);
            var response = await _httpClient.PostAsJsonAsync($"/api/submissions/{submissionId}/grade", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SubmissionDto>();
                _logger.LogInformation("Grade submitted successfully. SubmissionId: {SubmissionId}", submissionId);
                return result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to grade submission. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, errorContent);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error grading submission. SubmissionId: {SubmissionId}", submissionId);
            throw;
        }
    }
}


