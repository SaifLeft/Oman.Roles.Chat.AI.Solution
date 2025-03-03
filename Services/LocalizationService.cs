using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Services
{
    public interface ILocalizationService
    {
        string GetMessage(string key, string category = "Messages", string language = "en");
    }

    public class LocalizationService : ILocalizationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LocalizationService> _logger;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _resources;

        public LocalizationService(IConfiguration configuration, ILogger<LocalizationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _resources = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            LoadResources();
        }

        private void LoadResources()
        {
            try
            {
                var resourcesPath = _configuration["Localization:ResourcesPath"] ?? "Resources";
                var resourceFiles = Directory.GetFiles(resourcesPath, "*.json");

                foreach (var file in resourceFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var language = fileName.ToLower();

                    var json = File.ReadAllText(file);
                    var resourceData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);

                    if (resourceData != null)
                    {
                        _resources[language] = resourceData;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading localization resources");
                // Add default fallback resources
                _resources["en"] = new Dictionary<string, Dictionary<string, string>>
                {
                    ["Errors"] = new Dictionary<string, string>
                    {
                        ["GenericError"] = "An error occurred",
                        ["ServiceUnavailable"] = "Service unavailable"
                    },
                    ["Messages"] = new Dictionary<string, string>
                    {
                        ["Success"] = "Success"
                    }
                };
            }
        }

        public string GetMessage(string key, string category = "Messages", string language = "en")
        {
            if (_resources.TryGetValue(language.ToLower(), out var languageResources) &&
                languageResources.TryGetValue(category, out var categoryResources) &&
                categoryResources.TryGetValue(key, out var message))
            {
                return message;
            }

            return _resources["en"]["Messages"]["GenericError"];
        }
    }
}