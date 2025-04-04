using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Tools
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
                        Console.WriteLine($"Found key: {key} in category: {category}");
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
                        Console.WriteLine($"Added new key: {key} in category: {category} for language: {language}");
                    }
                }

                // Write updated resources back to file
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_resources[language], options);
                string outputFile = Path.Combine(_resourcesPath, $"{language}.json");
                await File.WriteAllTextAsync(outputFile, json);
                Console.WriteLine($"Updated resource file: {outputFile}");
            }

            Console.WriteLine("Resource files have been updated.");
        }

        /// <summary>
        /// Run the complete update process
        /// </summary>
        public async Task RunAsync()
        {
            Console.WriteLine($"Scanning codebase in: {_basePath}");
            Console.WriteLine($"Resources directory: {_resourcesPath}");
            
            await ScanCodebaseForKeysAsync();
            await UpdateResourceFilesAsync();
        }
    }

    /// <summary>
    /// Command line tool for updating localization resources
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point for the resource update tool
        /// </summary>
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting resource update tool...");
                
                // Get solution directory (3 levels up from the tool directory)
                string currentDir = Directory.GetCurrentDirectory();
                Console.WriteLine($"Current directory: {currentDir}");
                
                string solutionDir = Path.GetFullPath(Path.Combine(currentDir));
                Console.WriteLine($"Solution directory: {solutionDir}");
                
                // Resources path is in the API project
                string resourcesPath = Path.Combine(solutionDir, "API", "Resources");
                Console.WriteLine($"Resources directory: {resourcesPath}");
                
                if (!Directory.Exists(resourcesPath))
                {
                    Console.WriteLine($"Creating resources directory: {resourcesPath}");
                    Directory.CreateDirectory(resourcesPath);
                }
                
                var tool = new UpdateResourcesTools(solutionDir, resourcesPath);
                Console.WriteLine("Tool initialized. Starting scan...");
                await tool.RunAsync();
                
                Console.WriteLine("Resource update completed successfully.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
} 