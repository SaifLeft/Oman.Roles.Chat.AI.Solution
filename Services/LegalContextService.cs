using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace Services
{
    /// <summary>
    /// واجهة خدمة التحليل القانوني وإدارة السياق
    /// </summary>
    public interface ILegalContextService
    {
        /// <summary>
        /// تحليل الاستعلام وتحديد نوع المسألة القانونية
        /// </summary>
        /// <param name="query">استعلام المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>نوع المسألة القانونية المحددة</returns>
        Task<string> DetectLegalTopicAsync(string query, string language = "ar");

        /// <summary>
        /// استخراج كلمات مفتاحية قانونية من نص
        /// </summary>
        /// <param name="text">النص المراد تحليله</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>قائمة بالكلمات المفتاحية القانونية</returns>
        Task<List<string>> ExtractLegalKeywordsAsync(string text, string language = "ar");

        /// <summary>
        /// البحث عن ملفات PDF ذات صلة بالاستعلام
        /// </summary>
        /// <param name="query">استعلام المستخدم</param>
        /// <param name="availablePdfFiles">قائمة ملفات PDF المتاحة</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>قائمة مرتبة بملفات PDF ذات الصلة</returns>
        Task<List<string>> FindRelevantPdfFilesAsync(string query, List<string> availablePdfFiles, string language = "ar");

        /// <summary>
        /// إثراء السياق القانوني
        /// </summary>
        /// <param name="query">استعلام المستخدم</param>
        /// <param name="pdfContents">محتويات ملفات PDF</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>سياق قانوني محسن</returns>
        Task<string> EnrichLegalContextAsync(string query, Dictionary<string, string> pdfContents, string language = "ar");

        /// <summary>
        /// التحقق من شرعية الاستعلام
        /// </summary>
        /// <param name="query">استعلام المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>نتيجة التحقق من شرعية الاستعلام</returns>
        Task<(bool IsValid, string Message)> ValidateQueryAsync(string query, string language = "ar");
    }

    /// <summary>
    /// تنفيذ خدمة التحليل القانوني وإدارة السياق
    /// </summary>
    public class LegalContextService : ILegalContextService
    {
        private readonly IPdfExtractionService _pdfExtractionService;
        private readonly IDeepSeekService _deepSeekService;
        private readonly ILogger<LegalContextService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;

        private readonly string _pdfBasePath;

        // تخزين مؤقت للكلمات المفتاحية لكل ملف
        private readonly ConcurrentDictionary<string, List<string>> _fileKeywordsCache = new();

        // قائمة بأنواع المواضيع القانونية المدعومة
        private readonly Dictionary<string, HashSet<string>> _legalTopics = new()
        {
            ["ar"] = new HashSet<string>
            {
                "أحوال شخصية", "قانون مدني", "قانون تجاري", "قانون العمل", "قانون جنائي",
                "قانون إداري", "قانون دستوري", "إجراءات حكومية", "عقود", "تأشيرات",
                "ضرائب", "أراضي وعقارات", "شركات", "تأمين", "بنوك"
            },
            ["en"] = new HashSet<string>
            {
                "Personal Status", "Civil Law", "Commercial Law", "Labor Law", "Criminal Law",
                "Administrative Law", "Constitutional Law", "Government Procedures", "Contracts", "Visas",
                "Taxes", "Real Estate", "Companies", "Insurance", "Banking"
            }
        };

        // قائمة بالكلمات المفتاحية القانونية الشائعة
        private readonly Dictionary<string, HashSet<string>> _commonLegalKeywords = new()
        {
            ["ar"] = new HashSet<string>
            {
                "قانون", "تشريع", "محكمة", "دعوى", "محامي", "مرسوم", "لائحة", "عقد", "اتفاقية",
                "قضاء", "حكم", "غرامة", "عقوبة", "تعويض", "ميراث", "طلاق", "أحوال شخصية", "مدني",
                "تجاري", "جنائي", "إداري", "عمل", "تأشيرة", "إقامة", "ضريبة", "ترخيص", "ملكية"
            },
            ["en"] = new HashSet<string>
            {
                "law", "legislation", "court", "lawsuit", "lawyer", "decree", "regulation", "contract", "agreement",
                "judiciary", "verdict", "fine", "penalty", "compensation", "inheritance", "divorce", "personal status", "civil",
                "commercial", "criminal", "administrative", "labor", "visa", "residency", "tax", "license", "property"
            }
        };

        /// <summary>
        /// إنشاء مثيل جديد من خدمة التحليل القانوني
        /// </summary>
        public LegalContextService(
            IPdfExtractionService pdfExtractionService,
            IDeepSeekService deepSeekService,
            ILogger<LegalContextService> logger,
            ILocalizationService localizationService,
            IConfiguration configuration)
        {
            _pdfExtractionService = pdfExtractionService;
            _deepSeekService = deepSeekService;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;

            // تحديد مسار ملفات PDF
            _pdfBasePath = _configuration["ChatSettings:PdfBasePath"] ??
                          Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfFiles");
        }

        /// <summary>
        /// تحليل الاستعلام وتحديد نوع المسألة القانونية
        /// </summary>
        public async Task<string> DetectLegalTopicAsync(string query, string language = "ar")
        {
            try
            {
                _logger.LogInformation("تحليل نوع المسألة القانونية للاستعلام: {Query}", query);

                // الحصول على الكلمات المفتاحية من الاستعلام
                var keywords = await ExtractLegalKeywordsAsync(query, language);

                // إعداد سياق للاستشارة مع DeepSeek
                var topicContext = language == "ar"
                    ? $"حدد نوع المسألة القانونية التي يتعلق بها الاستعلام التالي من ضمن القائمة: {string.Join(", ", _legalTopics["ar"])}. الاستعلام: {query}"
                    : $"Identify the type of legal matter related to the following query from this list: {string.Join(", ", _legalTopics["en"])}. Query: {query}";

                // استخدام DeepSeek للتحليل
                var detectedTopic = await _deepSeekService.ExecuteLegalQueryAsync(
                    query: topicContext,
                    context: string.Join(", ", keywords),
                    language: language
                );

                // التحقق من أن الموضوع المكتشف موجود في القائمة
                var topics = _legalTopics[language == "ar" ? "ar" : "en"];
                foreach (var topic in topics)
                {
                    if (detectedTopic.Contains(topic))
                    {
                        return topic;
                    }
                }

                // إذا لم يتم العثور على موضوع محدد، إرجاع القيمة الافتراضية
                return language == "ar" ? "إجراءات حكومية" : "Government Procedures";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحليل نوع المسألة القانونية للاستعلام: {Query}", query);
                return language == "ar" ? "غير محدد" : "Unspecified";
            }
        }

        /// <summary>
        /// استخراج كلمات مفتاحية قانونية من نص
        /// </summary>
        public async Task<List<string>> ExtractLegalKeywordsAsync(string text, string language = "ar")
        {
            try
            {
                _logger.LogInformation("استخراج الكلمات المفتاحية القانونية من النص");

                var legalKeywords = new HashSet<string>();
                var commonKeywords = _commonLegalKeywords[language == "ar" ? "ar" : "en"];

                // 1. البحث عن الكلمات المفتاحية الشائعة
                var words = Regex.Split(text.ToLower(), @"\W+").Where(w => w.Length > 2).ToList();
                foreach (var word in words)
                {
                    if (commonKeywords.Contains(word))
                    {
                        legalKeywords.Add(word);
                    }
                }

                // 2. البحث عن عبارات نمطية قانونية (مثل "وفقاً للمادة" أو "according to article")
                string pattern = language == "ar"
                    ? @"(وفقاً|وفقا|طبقاً|طبقا|حسب|بموجب|بناءً على|بناء على)\s+(المادة|البند|القانون|اللائحة|المرسوم|القرار)\s+(\d+)"
                    : @"(according to|as per|pursuant to|under|based on)\s+(article|section|law|regulation|decree|decision)\s+(\d+)";

                var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    legalKeywords.Add(match.Value);
                }

                // 3. استخدام DeepSeek لاستخراج كلمات مفتاحية متخصصة (للقضايا المعقدة)
                if (text.Length > 100) // فقط للنصوص الطويلة
                {
                    var prompt = language == "ar"
                        ? $"استخرج 5 كلمات أو عبارات مفتاحية قانونية من النص التالي: {text}"
                        : $"Extract 5 key legal terms or phrases from the following text: {text}";

                    var aiResponse = await _deepSeekService.ExecuteLegalQueryAsync(prompt, "", language);

                    // استخراج الكلمات المفتاحية من رد النموذج
                    var aiKeywords = Regex.Split(aiResponse, @"[\r\n,]+")
                        .Where(k => !string.IsNullOrWhiteSpace(k))
                        .Select(k => k.Trim(' ', '.', ':', '-', '*', '•'))
                        .Where(k => k.Length > 2);

                    foreach (var keyword in aiKeywords)
                    {
                        legalKeywords.Add(keyword);
                    }
                }

                return legalKeywords.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في استخراج الكلمات المفتاحية القانونية");
                return new List<string>();
            }
        }

        /// <summary>
        /// البحث عن ملفات PDF ذات صلة بالاستعلام
        /// </summary>
        public async Task<List<string>> FindRelevantPdfFilesAsync(string query, List<string> availablePdfFiles, string language = "ar")
        {
            try
            {
                _logger.LogInformation("البحث عن ملفات PDF ذات صلة بالاستعلام: {Query}", query);

                // إذا لم تكن هناك ملفات متاحة
                if (availablePdfFiles.Count == 0)
                {
                    return new List<string>();
                }

                // استخراج الكلمات المفتاحية من الاستعلام
                var queryKeywords = await ExtractLegalKeywordsAsync(query, language);
                if (queryKeywords.Count == 0)
                {
                    // إذا لم يتم العثور على كلمات مفتاحية، إرجاع جميع الملفات المتاحة
                    return availablePdfFiles;
                }

                // قاموس لتخزين درجة الصلة لكل ملف
                var relevanceScores = new Dictionary<string, int>();

                // تقييم كل ملف
                foreach (var pdfFile in availablePdfFiles)
                {
                    // الحصول على كلمات مفتاحية للملف (من التخزين المؤقت أو استخراجها)
                    if (!_fileKeywordsCache.TryGetValue(pdfFile, out var fileKeywords))
                    {
                        var filePath = Path.Combine(_pdfBasePath, pdfFile);
                        if (File.Exists(filePath))
                        {
                            var content = await _pdfExtractionService.ExtractTextFromPdfAsync(pdfFile);
                            fileKeywords = await ExtractLegalKeywordsAsync(content, language);
                            _fileKeywordsCache.TryAdd(pdfFile, fileKeywords);
                        }
                        else
                        {
                            // إذا لم يكن الملف موجوداً، تخطيه
                            _logger.LogWarning("ملف PDF غير موجود: {FilePath}", filePath);
                            continue;
                        }
                    }

                    // حساب درجة الصلة بناءً على عدد الكلمات المفتاحية المشتركة
                    int score = queryKeywords.Count(qk => fileKeywords.Any(fk =>
                        fk.Contains(qk, StringComparison.OrdinalIgnoreCase) ||
                        qk.Contains(fk, StringComparison.OrdinalIgnoreCase)));

                    relevanceScores[pdfFile] = score;
                }

                // ترتيب الملفات حسب درجة الصلة (تنازلياً)
                return relevanceScores
                    .Where(rs => rs.Value > 0) // فقط الملفات ذات صلة
                    .OrderByDescending(rs => rs.Value)
                    .Select(rs => rs.Key)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في البحث عن ملفات PDF ذات صلة");
                return availablePdfFiles; // إرجاع جميع الملفات المتاحة في حالة الخطأ
            }
        }

        /// <summary>
        /// إثراء السياق القانوني
        /// </summary>
        public async Task<string> EnrichLegalContextAsync(string query, Dictionary<string, string> pdfContents, string language = "ar")
        {
            try
            {
                _logger.LogInformation("إثراء السياق القانوني للاستعلام: {Query}", query);

                // تحديد نوع المسألة القانونية
                var legalTopic = await DetectLegalTopicAsync(query, language);

                // استخراج الكلمات المفتاحية من الاستعلام
                var queryKeywords = await ExtractLegalKeywordsAsync(query, language);

                var enrichedContext = new StringBuilder();

                // إضافة المعلومات حول نوع المسألة القانونية
                if (language == "ar")
                {
                    enrichedContext.AppendLine($"### نوع المسألة القانونية: {legalTopic}");
                    enrichedContext.AppendLine($"### الكلمات المفتاحية المكتشفة: {string.Join(", ", queryKeywords)}");
                }
                else
                {
                    enrichedContext.AppendLine($"### Legal Matter Type: {legalTopic}");
                    enrichedContext.AppendLine($"### Detected Keywords: {string.Join(", ", queryKeywords)}");
                }
                enrichedContext.AppendLine();

                // استخراج المقاطع ذات الصلة من محتويات الملفات
                foreach (var content in pdfContents)
                {
                    if (string.IsNullOrWhiteSpace(content.Value))
                    {
                        continue;
                    }

                    var relevantParagraphs = ExtractRelevantParagraphs(content.Value, queryKeywords);
                    if (relevantParagraphs.Count > 0)
                    {
                        if (language == "ar")
                        {
                            enrichedContext.AppendLine($"### مقتطفات ذات صلة من ملف: {content.Key}");
                        }
                        else
                        {
                            enrichedContext.AppendLine($"### Relevant excerpts from file: {content.Key}");
                        }

                        foreach (var paragraph in relevantParagraphs)
                        {
                            enrichedContext.AppendLine($"- {paragraph}");
                        }

                        enrichedContext.AppendLine();
                    }
                }

                // إضافة توجيهات للإجابة
                if (language == "ar")
                {
                    enrichedContext.AppendLine("### إرشادات للإجابة:");
                    enrichedContext.AppendLine("- اعتمد فقط على المعلومات الواردة في المقتطفات أعلاه.");
                    enrichedContext.AppendLine("- قدم إجابة دقيقة ومباشرة للاستعلام.");
                    enrichedContext.AppendLine("- استشهد بالمصادر (أسماء الملفات) عند الإجابة.");
                    enrichedContext.AppendLine("- إذا لم تكن المعلومات كافية، اذكر ذلك بوضوح.");
                }
                else
                {
                    enrichedContext.AppendLine("### Answer Guidelines:");
                    enrichedContext.AppendLine("- Rely only on information contained in the excerpts above.");
                    enrichedContext.AppendLine("- Provide an accurate and direct answer to the query.");
                    enrichedContext.AppendLine("- Cite sources (file names) when answering.");
                    enrichedContext.AppendLine("- If information is insufficient, clearly state this.");
                }

                enrichedContext.AppendLine();
                enrichedContext.AppendLine($"### {(language == "ar" ? "الاستعلام" : "Query")}: {query}");

                return enrichedContext.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إثراء السياق القانوني");
                return query; // إرجاع الاستعلام الأصلي في حالة الخطأ
            }
        }

        /// <summary>
        /// التحقق من شرعية الاستعلام
        /// </summary>
        public async Task<(bool IsValid, string Message)> ValidateQueryAsync(string query, string language = "ar")
        {
            try
            {
                _logger.LogInformation("التحقق من شرعية الاستعلام: {Query}", query);

                // 1. فحص الكلمات المحظورة
                var prohibitedWords = language == "ar"
                    ? new[] { "تهريب", "مخدرات", "تهرب ضريبي", "غسل أموال", "احتيال" }
                    : new[] { "smuggling", "drugs", "tax evasion", "money laundering", "fraud" };

                foreach (var word in prohibitedWords)
                {
                    if (query.Contains(word, StringComparison.OrdinalIgnoreCase))
                    {
                        var message = language == "ar"
                            ? $"عذراً، لا يمكننا تقديم المساعدة في أمور تتعلق بـ '{word}'."
                            : $"Sorry, we cannot provide assistance on matters related to '{word}'.";

                        return (false, message);
                    }
                }

                // 2. فحص طول الاستعلام
                if (query.Length < 10)
                {
                    var message = language == "ar"
                        ? "يرجى تقديم استعلام أكثر تفصيلاً للحصول على إجابة دقيقة."
                        : "Please provide a more detailed query to get an accurate answer.";

                    return (false, message);
                }

                // 3. استخدام DeepSeek للتحقق من المحتوى المناسب
                if (query.Length > 100) // فقط للاستعلامات الطويلة
                {
                    var prompt = language == "ar"
                        ? $"هل هذا الاستعلام القانوني مناسب ولا يطلب مشورة غير أخلاقية أو غير قانونية؟ أجب بنعم أو لا فقط: {query}"
                        : $"Is this legal query appropriate and not asking for unethical or illegal advice? Answer with yes or no only: {query}";

                    var aiResponse = await _deepSeekService.ExecuteLegalQueryAsync(prompt, "", language);

                    if (language == "ar" && aiResponse.Contains("لا") ||
                        language == "en" && aiResponse.ToLower().Contains("no"))
                    {
                        var message = language == "ar"
                            ? "عذراً، لا يمكننا تقديم المساعدة في هذا النوع من الاستعلامات."
                            : "Sorry, we cannot provide assistance with this type of query.";

                        return (false, message);
                    }
                }

                // الاستعلام صالح
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في التحقق من شرعية الاستعلام");
                return (true, string.Empty); // افتراض صحة الاستعلام في حالة الخطأ
            }
        }

        #region Helper Methods

        /// <summary>
        /// استخراج الفقرات ذات الصلة من نص
        /// </summary>
        private List<string> ExtractRelevantParagraphs(string text, List<string> keywords)
        {
            var relevantParagraphs = new List<string>();

            // تقسيم النص إلى فقرات
            var paragraphs = Regex.Split(text, @"(\r?\n){2,}");

            foreach (var paragraph in paragraphs)
            {
                var trimmedParagraph = paragraph.Trim();
                if (string.IsNullOrWhiteSpace(trimmedParagraph) || trimmedParagraph.Length < 10)
                {
                    continue;
                }

                // التحقق مما إذا كانت الفقرة ذات صلة (تحتوي على أي من الكلمات المفتاحية)
                if (keywords.Any(k => trimmedParagraph.Contains(k, StringComparison.OrdinalIgnoreCase)))
                {
                    // اختصار الفقرات الطويلة جداً
                    if (trimmedParagraph.Length > 300)
                    {
                        trimmedParagraph = trimmedParagraph.Substring(0, 297) + "...";
                    }

                    relevantParagraphs.Add(trimmedParagraph);

                    // تحديد عدد الفقرات المستخرجة (لتجنب الإفراط)
                    if (relevantParagraphs.Count >= 5)
                    {
                        break;
                    }
                }
            }

            return relevantParagraphs;
        }

        #endregion
    }
}