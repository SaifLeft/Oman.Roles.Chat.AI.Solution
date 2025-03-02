using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.RegularExpressions;
using Path = System.IO.Path;
namespace Services
{
    /// <summary>
    /// واجهة خدمة استخراج محتوى PDF
    /// </summary>
    public interface IPdfExtractionService
    {
        /// <summary>
        /// استخراج نص من ملف PDF
        /// </summary>
        /// <param name="filePath">مسار الملف</param>
        /// <returns>النص المستخرج من الملف</returns>
        Task<string> ExtractTextFromPdfAsync(string filePath);

        /// <summary>
        /// استخراج نص من ملفات PDF متعددة
        /// </summary>
        /// <param name="filePaths">قائمة مسارات الملفات</param>
        /// <returns>النص المستخرج من جميع الملفات</returns>
        Task<Dictionary<string, string>> ExtractTextFromMultiplePdfsAsync(List<string> filePaths);

        /// <summary>
        /// استخراج بيانات منظمة من ملف PDF قانوني
        /// </summary>
        /// <param name="filePath">مسار الملف</param>
        /// <returns>بيانات منظمة مستخرجة من الملف</returns>
        Task<Dictionary<string, object>> ExtractStructuredLegalDataAsync(string filePath);

        /// <summary>
        /// تحليل ملف PDF وإنشاء فهرس للبحث
        /// </summary>
        /// <param name="filePath">مسار الملف</param>
        /// <returns>فهرس الملف للبحث</returns>
        Task<Dictionary<string, List<int>>> CreateSearchIndexAsync(string filePath);
    }

    /// <summary>
    /// تنفيذ خدمة استخراج محتوى PDF
    /// </summary>
    public class PdfExtractionService : IPdfExtractionService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PdfExtractionService> _logger;
        private readonly string _pdfBasePath;
        private readonly string _pdfCachePath;

        public PdfExtractionService(
            IConfiguration configuration,
            ILogger<PdfExtractionService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _pdfBasePath = _configuration["ChatSettings:PdfBasePath"] ??
                          Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfFiles");
            _pdfCachePath = _configuration["ChatSettings:PdfCachePath"] ??
                          Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfCache");

            Directory.CreateDirectory(_pdfBasePath);
            Directory.CreateDirectory(_pdfCachePath);
        }

        /// <summary>
        /// استخراج النص من ملف PDF
        /// </summary>
        public async Task<string> ExtractTextFromPdfAsync(string fileName)
        {
            try
            {
                _logger.LogInformation("Extracting text from PDF: {FileName}", fileName);

                string fullPath = Path.Combine(_pdfBasePath, fileName);
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("PDF file not found: {FilePath}", fullPath);
                    return string.Empty;
                }

                // Check cache first
                string cacheFilePath = Path.Combine(_pdfCachePath, $"{Path.GetFileNameWithoutExtension(fileName)}.txt");
                if (File.Exists(cacheFilePath))
                {
                    _logger.LogInformation("Using cached extracted text: {CacheFilePath}", cacheFilePath);
                    return await File.ReadAllTextAsync(cacheFilePath);
                }

                // Extract text using iTextSharp
                StringBuilder text = new StringBuilder();
                using (PdfReader reader = new PdfReader(fullPath))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        string pageText = PdfTextExtractor.GetTextFromPage(reader, i);
                        text.AppendLine(pageText);
                    }
                }

                // Cache the extracted text
                string extractedText = text.ToString();
                await File.WriteAllTextAsync(cacheFilePath, extractedText);

                return extractedText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from PDF: {FilePath}", fileName);
                return $"Error extracting text: {ex.Message}";
            }
        }

        /// <summary>
        /// استخراج النص من ملفات PDF متعددة
        /// </summary>
        public async Task<Dictionary<string, string>> ExtractTextFromMultiplePdfsAsync(List<string> filePaths)
        {
            _logger.LogInformation("استخراج النص من {Count} ملفات PDF", filePaths.Count);

            var results = new Dictionary<string, string>();

            foreach (var filePath in filePaths)
            {
                try
                {
                    string extractedText = await ExtractTextFromPdfAsync(filePath);
                    results[filePath] = extractedText;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "فشل استخراج النص من ملف: {FilePath}", filePath);
                    results[filePath] = $"فشل استخراج النص: {ex.Message}";
                }
            }

            return results;
        }

        /// <summary>
        /// استخراج بيانات منظمة من ملف PDF قانوني
        /// </summary>
        public async Task<Dictionary<string, object>> ExtractStructuredLegalDataAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("استخراج بيانات منظمة من ملف قانوني: {FilePath}", filePath);

                // استخراج النص الكامل أولاً
                string fullText = await ExtractTextFromPdfAsync(filePath);

                // تحليل النص وتنظيمه
                var structuredData = new Dictionary<string, object>();

                // استخراج العنوان (عادة في بداية المستند)
                var titleMatch = Regex.Match(fullText, @"^(.+?)(?:\r?\n|$)");
                if (titleMatch.Success)
                {
                    structuredData["Title"] = titleMatch.Groups[1].Value.Trim();
                }

                // استخراج التاريخ (بحث عن نمط تاريخ شائع)
                var dateMatches = Regex.Matches(fullText, @"\d{1,2}[/.-]\d{1,2}[/.-]\d{2,4}|\d{2,4}[/.-]\d{1,2}[/.-]\d{1,2}");
                if (dateMatches.Count > 0)
                {
                    structuredData["Dates"] = dateMatches.Select(m => m.Value).ToList();
                }

                // استخراج المواد القانونية (المادة X)
                var articlesMatches = Regex.Matches(fullText, @"(?:المادة|مادة|فصل)\s+(\d+)[:\s](.+?)(?=(?:المادة|مادة|فصل)\s+\d+[:\s]|$)", RegexOptions.Singleline);
                if (articlesMatches.Count > 0)
                {
                    var articles = new Dictionary<string, string>();
                    foreach (Match match in articlesMatches)
                    {
                        articles[$"المادة {match.Groups[1].Value}"] = match.Groups[2].Value.Trim();
                    }
                    structuredData["Articles"] = articles;
                }

                // استخراج المصطلحات القانونية المهمة
                var legalTerms = new List<string>();
                string[] commonLegalTerms = { "قانون", "تشريع", "لائحة", "مرسوم", "قرار", "حكم", "محكمة", "قضاء", "عقد", "اتفاقية", "دعوى", "خصوم", "المدعي", "المدعى عليه" };

                foreach (var term in commonLegalTerms)
                {
                    if (fullText.Contains(term))
                    {
                        legalTerms.Add(term);
                    }
                }
                structuredData["LegalTerms"] = legalTerms;

                // استخراج الأطراف المعنية (أشخاص أو كيانات)
                var partiesMatches = Regex.Matches(fullText, @"(?:الطرف الأول|الطرف الثاني|المالك|المستأجر|البائع|المشتري)[:\s]+(.+?)(?=\r?\n|$)");
                if (partiesMatches.Count > 0)
                {
                    var parties = new Dictionary<string, string>();
                    foreach (Match match in partiesMatches)
                    {
                        var partyType = match.Groups[0].Value.Split(':')[0].Trim();
                        parties[partyType] = match.Groups[1].Value.Trim();
                    }
                    structuredData["Parties"] = parties;
                }

                return structuredData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في استخراج البيانات المنظمة من ملف PDF: {FilePath}", filePath);
                return new Dictionary<string, object> { { "Error", ex.Message } };
            }
        }

        /// <summary>
        /// إنشاء فهرس بحث للملف
        /// </summary>
        public async Task<Dictionary<string, List<int>>> CreateSearchIndexAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("إنشاء فهرس بحث لملف: {FilePath}", filePath);

                // استخراج النص الكامل
                string fullText = await ExtractTextFromPdfAsync(filePath);

                // تقسيم النص إلى فقرات
                string[] paragraphs = fullText.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

                // إنشاء الفهرس
                var searchIndex = new Dictionary<string, List<int>>();

                // كلمات التوقف (الكلمات الشائعة التي لا تضيف قيمة للبحث)
                HashSet<string> stopWords = new HashSet<string> { "من", "إلى", "في", "على", "عن", "أو", "مع", "هذا", "هذه", "ذلك", "تلك", "الذي", "التي" };

                // معالجة كل فقرة
                for (int i = 0; i < paragraphs.Length; i++)
                {
                    string paragraph = paragraphs[i];

                    // تنظيف النص وتقسيمه إلى كلمات
                    string[] words = Regex.Split(paragraph.ToLower(), @"\W+")
                        .Where(word => word.Length > 2 && !stopWords.Contains(word))
                        .ToArray();

                    // إضافة كل كلمة فريدة إلى الفهرس مع رقم الفقرة
                    foreach (string word in words.Distinct())
                    {
                        if (!searchIndex.ContainsKey(word))
                        {
                            searchIndex[word] = new List<int>();
                        }

                        if (!searchIndex[word].Contains(i))
                        {
                            searchIndex[word].Add(i);
                        }
                    }
                }

                _logger.LogInformation("تم إنشاء فهرس بحث يحتوي على {Count} كلمة", searchIndex.Count);
                return searchIndex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء فهرس البحث لملف PDF: {FilePath}", filePath);
                return new Dictionary<string, List<int>>();
            }
        }
    }
}