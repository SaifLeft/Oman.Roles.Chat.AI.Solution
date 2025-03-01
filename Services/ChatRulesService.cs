using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Services
{
    /// <summary>
    /// خدمة إدارة قواعد الدردشة مع Deep Seek
    /// </summary>
    public interface IChatRulesService
    {
        /// <summary>
        /// الحصول على القواعد الافتراضية
        /// </summary>
        string GetDefaultRules(string language = "en");

        /// <summary>
        /// تحديث القواعد الافتراضية
        /// </summary>
        bool UpdateDefaultRules(string rules, string language = "en");

        /// <summary>
        /// الحصول على قائمة القواعد المتاحة
        /// </summary>
        Dictionary<string, string> GetAvailableRulesets();

        /// <summary>
        /// إضافة مجموعة قواعد جديدة
        /// </summary>
        bool AddRuleset(string name, string rules);

        /// <summary>
        /// حذف مجموعة قواعد
        /// </summary>
        bool DeleteRuleset(string name);
    }

    public class ChatRulesService : IChatRulesService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatRulesService> _logger;
        private readonly string _rulesPath;
        private readonly Dictionary<string, string> _rulesets = new();
        private readonly object _fileLock = new object();

        private readonly Dictionary<string, string> _defaultRules = new()
        {
            ["en"] = @"
1. Only answer questions related to the provided PDF files.
2. Do not provide information outside the scope of the specified files.
3. Refuse to answer any question not related to the content of the files.
4. Maintain respect and politeness in all responses.
5. Clearly indicate the source of information from the file when answering.",
            ["ar"] = @"
1. الالتزام بالإجابة على الأسئلة المتعلقة بالملفات PDF المحددة فقط.
2. عدم إعطاء معلومات خارج نطاق الملفات المحددة.
3. رفض الإجابة على أي سؤال غير متعلق بمحتوى الملفات.
4. الالتزام بقواعد الاحترام والأدب في الردود.
5. توضيح مصدر المعلومة من الملف عند الإجابة."
        };

        public ChatRulesService(IConfiguration configuration, ILogger<ChatRulesService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // تحديد مسار حفظ القواعد
            _rulesPath = _configuration["ChatSettings:RulesPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChatRules");

            // التأكد من وجود المسار
            Directory.CreateDirectory(_rulesPath);

            // تحميل القواعد المخزنة
            LoadRulesets();
        }

        /// <summary>
        /// الحصول على القواعد الافتراضية
        /// </summary>
        public string GetDefaultRules(string language = "en")
        {
            if (!_defaultRules.ContainsKey(language))
            {
                language = "en";
            }

            return _defaultRules[language];
        }

        /// <summary>
        /// تحديث القواعد الافتراضية
        /// </summary>
        public bool UpdateDefaultRules(string rules, string language = "en")
        {
            if (string.IsNullOrEmpty(rules))
            {
                return false;
            }

            try
            {
                _defaultRules[language] = rules;
                SaveDefaultRules();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث القواعد الافتراضية");
                return false;
            }
        }

        /// <summary>
        /// الحصول على قائمة القواعد المتاحة
        /// </summary>
        public Dictionary<string, string> GetAvailableRulesets()
        {
            return _rulesets;
        }

        /// <summary>
        /// إضافة مجموعة قواعد جديدة
        /// </summary>
        public bool AddRuleset(string name, string rules)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(rules))
            {
                return false;
            }

            try
            {
                _rulesets[name] = rules;
                SaveRuleset(name, rules);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إضافة مجموعة قواعد جديدة");
                return false;
            }
        }

        /// <summary>
        /// حذف مجموعة قواعد
        /// </summary>
        public bool DeleteRuleset(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            try
            {
                if (_rulesets.Remove(name))
                {
                    var rulesetPath = Path.Combine(_rulesPath, $"{name}.json");
                    if (File.Exists(rulesetPath))
                    {
                        File.Delete(rulesetPath);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف مجموعة القواعد");
                return false;
            }
        }

        /// <summary>
        /// تحميل مجموعات القواعد
        /// </summary>
        private void LoadRulesets()
        {
            try
            {
                // تحميل القواعد الافتراضية المخزنة
                var defaultRulesPath = Path.Combine(_rulesPath, "default_rules.json");
                if (File.Exists(defaultRulesPath))
                {
                    var json = File.ReadAllText(defaultRulesPath);
                    var savedDefaultRules = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (savedDefaultRules != null)
                    {
                        foreach (var kvp in savedDefaultRules)
                        {
                            _defaultRules[kvp.Key] = kvp.Value;
                        }
                    }
                }

                // تحميل مجموعات القواعد المخصصة
                var files = Directory.GetFiles(_rulesPath, "*.json");
                foreach (var file in files)
                {
                    if (Path.GetFileName(file) != "default_rules.json")
                    {
                        var json = File.ReadAllText(file);
                        var ruleset = JsonSerializer.Deserialize<KeyValuePair<string, string>>(json);
                        var name = Path.GetFileNameWithoutExtension(file);
                        _rulesets[name] = json;
                    }
                }

                _logger.LogInformation("تم تحميل {count} مجموعة قواعد", _rulesets.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل مجموعات القواعد");
            }
        }

        /// <summary>
        /// حفظ القواعد الافتراضية
        /// </summary>
        private void SaveDefaultRules()
        {
            try
            {
                lock (_fileLock)
                {
                    var defaultRulesPath = Path.Combine(_rulesPath, "default_rules.json");
                    var json = JsonSerializer.Serialize(_defaultRules, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(defaultRulesPath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حفظ القواعد الافتراضية");
            }
        }

        /// <summary>
        /// حفظ مجموعة قواعد
        /// </summary>
        private void SaveRuleset(string name, string rules)
        {
            try
            {
                lock (_fileLock)
                {
                    var rulesetPath = Path.Combine(_rulesPath, $"{name}.json");
                    var ruleData = new KeyValuePair<string, string>(name, rules);
                    var json = JsonSerializer.Serialize(ruleData, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(rulesetPath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حفظ مجموعة القواعد {name}", name);
            }
        }
    }
}