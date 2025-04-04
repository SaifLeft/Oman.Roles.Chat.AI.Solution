using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API
{
    /// <summary>
    /// Tool for updating localization resources from code scanning
    /// </summary>
    public class UpdateResourcesTools
    {
        private readonly string _basePath;
        private readonly string _resourcesPath;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _resources;
        private readonly HashSet<(string key, string category)> _foundKeys;

        /// <summary>
        /// Create a new resource update tool
        /// </summary>
        /// <param name="basePath">Solution base path</param>
        /// <param name="resourcesPath">Resources directory path</param>
        public UpdateResourcesTools(string basePath, string resourcesPath)
        {
            _basePath = basePath;
            _resourcesPath = resourcesPath;
            _resources = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            _foundKeys = new HashSet<(string key, string category)>();

            // Create resources directory if it doesn't exist
            if (!Directory.Exists(_resourcesPath))
            {
                Directory.CreateDirectory(_resourcesPath);
            }

            // Load existing resources
            LoadExistingResources();
        }

        /// <summary>
        /// Load existing resource files
        /// </summary>
        private void LoadExistingResources()
        {
            if (Directory.Exists(_resourcesPath))
            {
                var resourceFiles = Directory.GetFiles(_resourcesPath, "*.json");

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
        }

        /// <summary>
        /// Scan the codebase for all localization keys
        /// </summary>
        public async Task ScanCodebaseForKeysAsync()
        {
            var csFiles = Directory.GetFiles(_basePath, "*.cs", SearchOption.AllDirectories);
            var pattern = @"_localizationService\.GetMessage\(""([^""]+)"",\s*""([^""]+)""";

            foreach (var file in csFiles)
            {
                string content = await File.ReadAllTextAsync(file);
                var matches = Regex.Matches(content, pattern);

                foreach (Match match in matches)
                {
                    if (match.Groups.Count >= 3)
                    {
                        string key = match.Groups[1].Value;
                        string category = match.Groups[2].Value;
                        _foundKeys.Add((key, category));
                    }
                }
            }

            Console.WriteLine($"Found {_foundKeys.Count} unique keys across the codebase.");
        }

        /// <summary>
        /// Update resource files with found keys
        /// </summary>
        public async Task UpdateResourceFilesAsync()
        {
            var languages = new[] { "ar", "en" };

            foreach (var language in languages)
            {
                bool languageExists = _resources.ContainsKey(language);
                if (!languageExists)
                {
                    _resources[language] = new Dictionary<string, Dictionary<string, string>>();
                }

                foreach (var (key, category) in _foundKeys)
                {
                    if (!_resources[language].ContainsKey(category))
                    {
                        _resources[language][category] = new Dictionary<string, string>();
                    }

                    if (!_resources[language][category].ContainsKey(key))
                    {
                        // Add a placeholder value if the key doesn't exist
                        string defaultValue = language == "en" ? key : $"[{language}] {key}";
                        _resources[language][category][key] = defaultValue;
                    }
                }

                // Write updated resources back to file
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_resources[language], options);
                await File.WriteAllTextAsync(Path.Combine(_resourcesPath, $"{language}.json"), json);
            }

            Console.WriteLine("Resource files have been updated.");
        }

        /// <summary>
        /// Run the complete update process
        /// </summary>
        public async Task RunAsync()
        {
            await ScanCodebaseForKeysAsync();
            await UpdateResourceFilesAsync();
        }
    }

    /// <summary>
    /// Command line tool for updating localization resources
    /// </summary>
    public class UpdateResourcesProgram
    {
        /// <summary>
        /// Entry point for the resource update tool
        /// </summary>
        public static async Task Main(string[] args)
        {
            string basePath = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
            string resourcesPath = args.Length > 1 ? args[1] : Path.Combine(basePath, "Resources");

            var tool = new UpdateResourcesTools(basePath, resourcesPath);
            await tool.RunAsync();
        }
    }
} 