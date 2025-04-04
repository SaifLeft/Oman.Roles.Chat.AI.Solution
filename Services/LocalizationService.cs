using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Services
{
    /// <summary>
    /// واجهة خدمات الترجمة والتعريب
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// الحصول على رسالة مترجمة
        /// </summary>
        /// <param name="key">مفتاح الرسالة</param>
        /// <param name="category">فئة الرسالة</param>
        /// <param name="language">اللغة المستهدفة</param>
        /// <returns>الرسالة المترجمة</returns>
        string GetMessage(string key, string category, string language);

        /// <summary>
        /// إضافة أو تحديث رسالة مترجمة
        /// </summary>
        /// <param name="key">مفتاح الرسالة</param>
        /// <param name="category">فئة الرسالة</param>
        /// <param name="language">اللغة المستهدفة</param>
        /// <param name="message">نص الرسالة</param>
        /// <returns>نجاح العملية</returns>
        bool AddOrUpdateMessage(string key, string category, string language, string message);

        /// <summary>
        /// الحصول على جميع الرسائل لفئة ولغة محددة
        /// </summary>
        /// <param name="category">فئة الرسالة</param>
        /// <param name="language">اللغة المستهدفة</param>
        /// <returns>قاموس الرسائل</returns>
        Dictionary<string, string> GetAllMessages(string category, string language);
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
            try
            {
                if (_resources.TryGetValue(language.ToLower(), out var languageResources) &&
                languageResources.TryGetValue(category, out var categoryResources) &&
                categoryResources.TryGetValue(key, out var message))
                {
                    return message;
                }

                return _resources["en"]["Messages"]["GenericError"];
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

                return _resources["en"]["Messages"]["GenericError"];
            }
        }

        /// <summary>
        /// إضافة أو تحديث رسالة في ملف اللغة
        /// </summary>
        public bool AddOrUpdateMessage(string key, string category, string language, string message)
        {
            try
            {
                _logger.LogInformation("إضافة/تحديث رسالة. المفتاح: {Key}, الفئة: {Category}, اللغة: {Language}", key, category, language);

                // تحديد المسار الكامل للملف
                string filePath = GetResourceFilePath(category, language);
                var directory = Path.GetDirectoryName(filePath);

                // التأكد من وجود المجلد
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // قراءة محتوى الملف الحالي أو إنشاء ملف جديد
                Dictionary<string, string> messages;
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    messages = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent) ?? new Dictionary<string, string>();
                }
                else
                {
                    messages = new Dictionary<string, string>();
                }

                // إضافة أو تحديث المفتاح
                messages[key] = message;

                // حفظ التغييرات
                var options = new JsonSerializerOptions { WriteIndented = true };
                string updatedJson = JsonSerializer.Serialize(messages, options);
                File.WriteAllText(filePath, updatedJson);

                // تحديث الذاكرة المؤقتة
                if (_resources.ContainsKey(language))
                {
                    if (_resources[language].ContainsKey(category))
                    {
                        _resources[language][category][key] = message;
                    }
                    else
                    {
                        _resources[language][category] = messages;
                    }
                }
                else
                {
                    _resources[language] = new Dictionary<string, Dictionary<string, string>> { { category, messages } };
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إضافة/تحديث رسالة");
                return false;
            }
        }

        /// <summary>
        /// الحصول على مسار ملف الموارد
        /// </summary>
        private string GetResourceFilePath(string category, string language)
        {
            var resourcesPath = _configuration["Localization:ResourcesPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            return Path.Combine(resourcesPath, language, $"{category}.json");
        }

        /// <summary>
        /// الحصول على جميع الرسائل في فئة معينة
        /// </summary>
        public Dictionary<string, string> GetAllMessages(string category, string language)
        {
            try
            {
                _logger.LogInformation("الحصول على جميع الرسائل. الفئة: {Category}, اللغة: {Language}", category, language);

                // محاولة الحصول على الرسائل من الذاكرة
                if (_resources.ContainsKey(language) && _resources[language].ContainsKey(category))
                {
                    return _resources[language][category];
                }

                // تحديد مسار الملف
                string filePath = GetResourceFilePath(category, language);
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("ملف اللغة غير موجود: {FilePath}", filePath);
                    return new Dictionary<string, string>();
                }

                // قراءة محتوى الملف
                string jsonContent = File.ReadAllText(filePath);
                var messages = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent) ?? new Dictionary<string, string>();

                // تخزين في الذاكرة
                if (_resources.ContainsKey(language))
                {
                    _resources[language][category] = messages;
                }
                else
                {
                    _resources[language] = new Dictionary<string, Dictionary<string, string>>
                    {
                        { category, messages }
                    };
                }

                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على جميع الرسائل");
                return new Dictionary<string, string>();
            }
        }
    }
}