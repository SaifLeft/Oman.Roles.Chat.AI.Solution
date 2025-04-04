using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class CollectLocalizationKeys
{
    private readonly string _basePath;
    private readonly string _resourcesPath;
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _resources;
    private readonly HashSet<(string key, string category)> _foundKeys;

    public CollectLocalizationKeys(string basePath, string resourcesPath)
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
                try
                {
                    var resourceData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);

                    if (resourceData != null)
                    {
                        _resources[language] = resourceData;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading resource file {file}: {ex.Message}");
                }
            }
        }
    }

    public async Task ScanCodebaseForKeysAsync()
    {
        var csFiles = Directory.GetFiles(_basePath, "*.cs", SearchOption.AllDirectories);
        var pattern = @"_localizationService\.GetMessage\(""([^""]+)"",\s*""([^""]+)""";
        var keysPerCategory = new Dictionary<string, HashSet<string>>();
        var keysPerFile = new Dictionary<string, HashSet<(string key, string category)>>();

        foreach (var file in csFiles)
        {
            string content = await File.ReadAllTextAsync(file);
            var matches = Regex.Matches(content, pattern);
            string relativePath = Path.GetRelativePath(_basePath, file);

            if (matches.Count > 0 && !keysPerFile.ContainsKey(relativePath))
            {
                keysPerFile[relativePath] = new HashSet<(string key, string category)>();
            }

            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 3)
                {
                    string key = match.Groups[1].Value;
                    string category = match.Groups[2].Value;
                    _foundKeys.Add((key, category));
                    
                    // Track by category
                    if (!keysPerCategory.ContainsKey(category))
                    {
                        keysPerCategory[category] = new HashSet<string>();
                    }
                    keysPerCategory[category].Add(key);
                    
                    // Track by file
                    keysPerFile[relativePath].Add((key, category));
                }
            }
        }

        // Print summary
        Console.WriteLine($"Found {_foundKeys.Count} unique keys across the codebase.");
        Console.WriteLine("\nKeys by Category:");
        
        foreach (var category in keysPerCategory.OrderBy(k => k.Key))
        {
            Console.WriteLine($"  {category.Key}: {category.Value.Count} keys");
        }
        
        Console.WriteLine("\nTop Files by Key Count:");
        foreach (var file in keysPerFile.OrderByDescending(f => f.Value.Count).Take(10))
        {
            Console.WriteLine($"  {file.Key}: {file.Value.Count} keys");
        }
    }

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

    public async Task ExportKeysToCSVAsync(string outputPath)
    {
        if (_foundKeys.Count == 0)
        {
            Console.WriteLine("No keys found. Run ScanCodebaseForKeysAsync first.");
            return;
        }

        using (var writer = new StreamWriter(outputPath))
        {
            // Write header
            await writer.WriteLineAsync("Key,Category,ArValue,EnValue");

            // Sort keys for easier reading
            var sortedKeys = _foundKeys.OrderBy(k => k.category).ThenBy(k => k.key).ToList();

            foreach (var (key, category) in sortedKeys)
            {
                string arValue = "";
                string enValue = "";

                // Get values from resource files if they exist
                if (_resources.TryGetValue("ar", out var arResources) &&
                    arResources.TryGetValue(category, out var arCategory) &&
                    arCategory.TryGetValue(key, out var arVal))
                {
                    arValue = arVal;
                }

                if (_resources.TryGetValue("en", out var enResources) &&
                    enResources.TryGetValue(category, out var enCategory) &&
                    enCategory.TryGetValue(key, out var enVal))
                {
                    enValue = enVal;
                }

                // Escape values for CSV
                string escapedKey = key.Replace("\"", "\"\"");
                string escapedCategory = category.Replace("\"", "\"\"");
                string escapedArValue = arValue.Replace("\"", "\"\"");
                string escapedEnValue = enValue.Replace("\"", "\"\"");

                await writer.WriteLineAsync($"\"{escapedKey}\",\"{escapedCategory}\",\"{escapedArValue}\",\"{escapedEnValue}\"");
            }
        }

        Console.WriteLine($"Exported {_foundKeys.Count} keys to {outputPath}");
    }

    public async Task TranslationCompareAsync()
    {
        if (!_resources.ContainsKey("ar") || !_resources.ContainsKey("en"))
        {
            Console.WriteLine("Both Arabic and English resource files are required for comparison.");
            return;
        }

        var arResources = _resources["ar"];
        var enResources = _resources["en"];

        int missingInAr = 0;
        int missingInEn = 0;
        int needsTranslation = 0;
        var categoriesWithIssues = new HashSet<string>();

        Console.WriteLine("\n=== Translation Status ===\n");

        // Check all categories in English file
        foreach (var category in enResources.Keys)
        {
            if (!arResources.ContainsKey(category))
            {
                Console.WriteLine($"Category '{category}' exists in English but not in Arabic");
                categoriesWithIssues.Add(category);
                missingInAr += enResources[category].Count;
                continue;
            }

            var enCategory = enResources[category];
            var arCategory = arResources[category];

            // Check for keys in English that are missing in Arabic
            foreach (var key in enCategory.Keys)
            {
                if (!arCategory.ContainsKey(key))
                {
                    Console.WriteLine($"Key '{key}' in category '{category}' exists in English but not in Arabic");
                    missingInAr++;
                    categoriesWithIssues.Add(category);
                    continue;
                }

                // Check if the Arabic translation is missing or may need review
                if (arCategory[key] == enCategory[key] || arCategory[key].StartsWith($"[ar] {key}"))
                {
                    Console.WriteLine($"Key '{key}' in category '{category}' may need translation in Arabic");
                    needsTranslation++;
                    categoriesWithIssues.Add(category);
                }
            }
        }

        // Check for keys in Arabic that are missing in English
        foreach (var category in arResources.Keys)
        {
            if (!enResources.ContainsKey(category))
            {
                Console.WriteLine($"Category '{category}' exists in Arabic but not in English");
                categoriesWithIssues.Add(category);
                missingInEn += arResources[category].Count;
                continue;
            }

            var arCategory = arResources[category];
            var enCategory = enResources[category];

            foreach (var key in arCategory.Keys)
            {
                if (!enCategory.ContainsKey(key))
                {
                    Console.WriteLine($"Key '{key}' in category '{category}' exists in Arabic but not in English");
                    missingInEn++;
                    categoriesWithIssues.Add(category);
                }
            }
        }

        Console.WriteLine("\n=== Translation Summary ===");
        Console.WriteLine($"Keys missing in Arabic: {missingInAr}");
        Console.WriteLine($"Keys missing in English: {missingInEn}");
        Console.WriteLine($"Keys that may need translation: {needsTranslation}");
        Console.WriteLine($"Categories with issues: {categoriesWithIssues.Count}");
    }

    public async Task TransferTranslationsAsync(string fromLanguage, string toLanguage)
    {
        if (!_resources.ContainsKey(fromLanguage) || !_resources.ContainsKey(toLanguage))
        {
            Console.WriteLine($"Both {fromLanguage} and {toLanguage} resource files are required for transfer.");
            return;
        }

        var fromResources = _resources[fromLanguage];
        var toResources = _resources[toLanguage];
        int transferred = 0;
        int skipped = 0;

        Console.WriteLine($"\n=== Transferring translations from {fromLanguage} to {toLanguage} ===\n");

        // Go through all categories in source language
        foreach (var category in fromResources.Keys)
        {
            if (!toResources.ContainsKey(category))
            {
                toResources[category] = new Dictionary<string, string>();
                Console.WriteLine($"Created category '{category}' in {toLanguage}");
            }

            var fromCategory = fromResources[category];
            var toCategory = toResources[category];

            // Transfer keys that are missing in the target language
            foreach (var key in fromCategory.Keys)
            {
                if (!toCategory.ContainsKey(key))
                {
                    // For Arabic target, add "[ar] " prefix if source is English
                    string value = fromCategory[key];
                    if (toLanguage == "ar" && fromLanguage == "en")
                    {
                        value = $"[ar] {value}";
                    }
                    
                    toCategory[key] = value;
                    transferred++;
                }
                else
                {
                    skipped++;
                }
            }
        }

        // Save the updated resources
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(toResources, options);
        await File.WriteAllTextAsync(Path.Combine(_resourcesPath, $"{toLanguage}.json"), json);

        Console.WriteLine($"\n=== Transfer Summary ===");
        Console.WriteLine($"Keys transferred: {transferred}");
        Console.WriteLine($"Keys skipped (already exist): {skipped}");
        Console.WriteLine($"{toLanguage}.json has been updated.");
    }

    public async Task RunAsync()
    {
        await ScanCodebaseForKeysAsync();
        await UpdateResourceFilesAsync();
    }

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Localization Keys Collection Tool");
        Console.WriteLine("=================================");

        // Determine paths
        string basePath = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
        basePath = Path.GetFullPath(basePath);
        
        string resourcesPath = args.Length > 1 ? args[1] : Path.Combine(basePath, "Resources");
        
        Console.WriteLine($"Base path: {basePath}");
        Console.WriteLine($"Resources path: {resourcesPath}");
        Console.WriteLine();

        try
        {
            // Initialize the tool
            var tool = new CollectLocalizationKeys(basePath, resourcesPath);
            
            // Check if we're just comparing translations
            if (args.Length > 2 && args[2] == "compare")
            {
                Console.WriteLine("Comparing translations between ar.json and en.json...");
                await tool.TranslationCompareAsync();
            }
            // Check if we're transferring translations
            else if (args.Length > 4 && args[2] == "transfer")
            {
                string fromLang = args[3];
                string toLang = args[4];
                Console.WriteLine($"Transferring translations from {fromLang}.json to {toLang}.json...");
                await tool.TransferTranslationsAsync(fromLang, toLang);
            }
            else
            {
                // Default scan and update
                Console.WriteLine("Scanning codebase for localization keys...");
                await tool.ScanCodebaseForKeysAsync();
                
                // Update resource files
                Console.WriteLine("\nUpdating resource files...");
                await tool.UpdateResourceFilesAsync();

                // Export to CSV
                string csvFilePath = Path.Combine(resourcesPath, "localization-keys.csv");
                Console.WriteLine($"\nExporting keys to CSV: {csvFilePath}");
                await tool.ExportKeysToCSVAsync(csvFilePath);
                
                // Compare translations
                Console.WriteLine("\nComparing translations...");
                await tool.TranslationCompareAsync();
            }
            
            Console.WriteLine("\nProcess completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
} 