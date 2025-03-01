using API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Services
{
    public interface IDeepSeekService
    {
        Task<string> ProcessPdfDataAsync(string prompt, string language = "en");
    }

    public class DeepSeekService : IDeepSeekService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeepSeekService> _logger;
        private readonly ILocalizationService _localizationService;

        public DeepSeekService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<DeepSeekService> logger,
            ILocalizationService localizationService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;
        }

        public async Task<string> ProcessPdfDataAsync(string prompt, string language = "en")
        {
            try
            {
                // Get the API key and endpoint from configuration
                var apiKey = _configuration["DeepSeekAPI:ApiKey"];
                var endpoint = _configuration["DeepSeekAPI:Endpoint"];
                var model = _configuration["DeepSeekAPI:Model"] ?? "deepseek-chat";

                // Check if the API key is configured
                if (string.IsNullOrEmpty(apiKey))
                {
                    var errorMessage = _localizationService.GetMessage("ApiKeyMissing", "Errors", language);
                    throw new InvalidOperationException(errorMessage);
                }

                // Check if the endpoint is configured
                if (string.IsNullOrEmpty(endpoint))
                {
                    var errorMessage = _localizationService.GetMessage("EndpointMissing", "Errors", language);
                    throw new InvalidOperationException(errorMessage);
                }

                // Set the API key in the request headers
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Prepare the request body
                var requestBody = new
                {
                    model,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.7,
                    max_tokens = 2000
                };

                var requestJson = JsonSerializer.Serialize(requestBody);
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // Send the request to DeepSeek
                var response = await _httpClient.PostAsync(endpoint, requestContent);

                // Check if the request was successful
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = _localizationService.GetMessage("ApiError", "Errors", language);
                    _logger.LogError("DeepSeek API returned status code: {statusCode}", response.StatusCode);
                    return $"{errorMessage} (Status Code: {response.StatusCode})";
                }

                // Read the response
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);

                // Extract the response text from the API response
                if (responseObject.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content))
                {
                    return content.GetString() ?? string.Empty;
                }

                _logger.LogWarning("Unexpected response format from DeepSeek API");
                return prompt; // Return the original prompt as fallback
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PDF data with DeepSeek");
                var errorMessage = _localizationService.GetMessage("ApiError", "Errors", language);
                return $"{errorMessage}: {ex.Message}";
            }
        }
    }
}