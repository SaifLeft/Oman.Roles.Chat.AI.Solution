using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.AIChat;
using Models.DTOs.Files;
using System.Text;
using System.Text.RegularExpressions;

namespace Services
{
    /// <summary>
    /// واجهة خدمة قاعدة المعرفة للاستشارات القانونية
    /// </summary>
    public interface IKnowledgeBaseService
    {
        /// <summary>
        /// الحصول على الوثائق المرتبطة بسؤال المستخدم
        /// </summary>
        /// <param name="query">سؤال المستخدم</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="roomId">معرف غرفة الدردشة</param>
        /// <param name="maxResults">الحد الأقصى للنتائج</param>
        /// <param name="language">اللغة</param>
        /// <returns>قائمة الوثائق ذات الصلة</returns>
        Task<List<DocumentReferenceDTO>> GetRelevantDocumentsAsync(string query, long userId, long roomId, int maxResults = 5, string language = "ar");

        /// <summary>
        /// الحصول على سياق مثرى من الوثائق المرتبطة
        /// </summary>
        /// <param name="query">سؤال المستخدم</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="roomId">معرف غرفة الدردشة</param>
        /// <param name="language">اللغة</param>
        /// <returns>سياق مثرى مع معلومات الوثائق</returns>
        Task<string> GetEnrichedContextAsync(string query, long userId, long roomId, string language = "ar");

        /// <summary>
        /// إنشاء رد من الذكاء الاصطناعي باستخدام الوثائق ذات الصلة
        /// </summary>
        /// <param name="query">سؤال المستخدم</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="roomId">معرف غرفة الدردشة</param>
        /// <param name="language">اللغة</param>
        /// <returns>رد الذكاء الاصطناعي مع المراجع</returns>
        Task<AIQueryResponseDTO> GetAIResponseWithReferencesAsync(string query, long userId, long roomId, string language = "ar");
    }

    /// <summary>
    /// تنفيذ خدمة قاعدة المعرفة للاستشارات القانونية
    /// </summary>
    public class KnowledgeBaseService : IKnowledgeBaseService
    {
        private readonly MuhamiContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KnowledgeBaseService> _logger;
        private readonly IPdfExtractionService _pdfExtractionService;
        private readonly IDeepSeekService _deepSeekService;
        private readonly IFileManagementService _fileManagementService;
        private readonly IConversationTrackingService _conversationTrackingService;

        /// <summary>
        /// إنشاء مثيل جديد من خدمة قاعدة المعرفة
        /// </summary>
        public KnowledgeBaseService(
            MuhamiContext context,
            IConfiguration configuration,
            ILogger<KnowledgeBaseService> logger,
            IPdfExtractionService pdfExtractionService,
            IDeepSeekService deepSeekService,
            IFileManagementService fileManagementService,
            IConversationTrackingService conversationTrackingService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _pdfExtractionService = pdfExtractionService;
            _deepSeekService = deepSeekService;
            _fileManagementService = fileManagementService;
            _conversationTrackingService = conversationTrackingService;
        }

        /// <summary>
        /// الحصول على الوثائق المرتبطة بسؤال المستخدم
        /// </summary>
        public async Task<List<DocumentReferenceDTO>> GetRelevantDocumentsAsync(string query, long userId, long roomId, int maxResults = 5, string language = "ar")
        {
            try
            {
                _logger.LogInformation("البحث عن الوثائق ذات الصلة للمستخدم {UserId} في الغرفة {RoomId}", userId, roomId);

                // الحصول على قائمة وثائق المستخدم النشطة
                var userDocuments = await _context.DataSourceFiles
                    .Where(f => (f.UploadedBy == userId || f.IsPublic) && f.IsActive && !f.IsDeleted && f.FileType == "pdf")
                    .OrderByDescending(f => f.CreateDate)
                    .Take(100) // الحد للتحقق
                    .ToListAsync();

                if (userDocuments.Count == 0)
                {
                    _logger.LogWarning("لم يتم العثور على وثائق للمستخدم {UserId}", userId);
                    return new List<DocumentReferenceDTO>();
                }

                var relevantDocuments = new List<DocumentReferenceDTO>();
                
                // استخراج الكلمات المفتاحية والعبارات من الاستعلام
                var keywords = ExtractKeywords(query);
                var phrases = ExtractPhrases(query);
                var legalTerms = ExtractLegalTerms(query, language);

                // استخراج الموضوع العام للاستعلام
                var queryTopic = ExtractQueryTopic(query, language);

                _logger.LogInformation("تم استخراج {KeywordCount} كلمة مفتاحية و {PhraseCount} عبارة من الاستعلام", 
                    keywords.Count, phrases.Count);

                // تقييم أهمية كل وثيقة
                foreach (var document in userDocuments)
                {
                    string extractedText = "";
                    
                    // استخراج النص من الوثيقة
                    extractedText = await _pdfExtractionService.ExtractTextFromPdfAsync(document.FileName);

                    if (string.IsNullOrEmpty(extractedText))
                    {
                        continue;
                    }

                    // حساب درجات الصلة المختلفة
                    double keywordScore = CalculateKeywordRelevanceScore(extractedText, keywords);
                    double phraseScore = CalculatePhraseRelevanceScore(extractedText, phrases);
                    double legalTermScore = CalculateLegalTermRelevanceScore(extractedText, legalTerms);
                    double topicScore = CalculateTopicRelevanceScore(extractedText, queryTopic);
                    
                    // الدرجة النهائية المرجحة
                    double relevanceScore = (keywordScore * 0.3) + (phraseScore * 0.4) + 
                                          (legalTermScore * 0.2) + (topicScore * 0.1);

                    _logger.LogDebug("الوثيقة {DocumentName} - درجة الصلة: {Score}", 
                        document.FileName, relevanceScore);

                    // إضافة المستند إذا كانت درجة الصلة أعلى من 0.15
                    if (relevanceScore > 0.15)
                    {
                        // استخراج مقاطع النص الأكثر صلة
                        string snippet = GetRelevantSnippet(extractedText, keywords, phrases, query, 300);
                        // محاولة تحديد رقم الصفحة المرجعية إذا أمكن
                        int? pageNumber = TryDetectPageNumber(extractedText, snippet);

                        var documentRef = new DocumentReferenceDTO
                        {
                            DocumentId = document.Id.ToString(),
                            DocumentName = document.FileName,
                            RelevanceScore = relevanceScore,
                            TextSnippet = snippet,
                            PageNumber = pageNumber
                        };

                        relevantDocuments.Add(documentRef);
                    }
                }

                // ترتيب الوثائق حسب درجة الصلة واختيار الأفضل
                return relevantDocuments
                    .OrderByDescending(d => d.RelevanceScore)
                    .Take(maxResults)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على الوثائق ذات الصلة للمستخدم {UserId}", userId);
                return new List<DocumentReferenceDTO>();
            }
        }

        /// <summary>
        /// الحصول على سياق مثرى من الوثائق المرتبطة
        /// </summary>
        public async Task<string> GetEnrichedContextAsync(string query, long userId, long roomId, string language = "ar")
        {
            try
            {
                // الحصول على الوثائق ذات الصلة
                var relevantDocuments = await GetRelevantDocumentsAsync(query, userId, roomId, 3, language);

                if (relevantDocuments.Count == 0)
                {
                    return string.Empty;
                }

                var sb = new StringBuilder();
                sb.AppendLine($"-- السؤال: {query} --");
                sb.AppendLine();
                sb.AppendLine("-- معلومات من المصادر ذات الصلة: --");
                sb.AppendLine();

                // إضافة المعلومات من كل وثيقة
                foreach (var doc in relevantDocuments)
                {
                    sb.AppendLine($"[المصدر: {doc.DocumentName}]");
                    
                    // الحصول على النص الكامل للوثيقة
                    var file = await _context.DataSourceFiles.FirstOrDefaultAsync(f => f.Id.ToString() == doc.DocumentId);
                    if (file != null)
                    {
                        // الحصول على النص المستخرج
                        var extractedText = await _pdfExtractionService.ExtractTextFromPdfAsync(file.FileName);
                        
                        // تحديد مقطع أكبر للسياق (حتى 2000 حرف)
                        var largerSnippet = GetRelevantSnippet(extractedText, ExtractKeywords(query), ExtractPhrases(query), query, 2000);
                        sb.AppendLine(largerSnippet);
                    }
                    else
                    {
                        sb.AppendLine(doc.TextSnippet);
                    }
                    
                    sb.AppendLine();
                }

                // تتبع الوثائق المستخدمة في المحادثة
                await TrackDocumentReferencesAsync(roomId.ToString(), userId, relevantDocuments);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إنشاء سياق مثرى للمستخدم {UserId}", userId);
                return string.Empty;
            }
        }

        /// <summary>
        /// إنشاء رد من الذكاء الاصطناعي باستخدام الوثائق ذات الصلة
        /// </summary>
        public async Task<AIQueryResponseDTO> GetAIResponseWithReferencesAsync(string query, long userId, long roomId, string language = "ar")
        {
            try
            {
                // الحصول على السياق المثرى
                string enrichedContext = await GetEnrichedContextAsync(query, userId, roomId, language);

                var response = new AIQueryResponseDTO
                {
                    Response = string.Empty,
                    DocumentReferences = new List<DocumentReferenceDTO>(),
                    TokenUsage = new TokenUsageDTO()
                };

                if (string.IsNullOrEmpty(enrichedContext))
                {
                    // إذا لم تكن هناك وثائق، أخبر المستخدم بذلك
                    var noDocsMessage = language == "ar"
                        ? "عذراً، لا توجد وثائق مرتبطة بسؤالك في قاعدة البيانات. يرجى تحميل الوثائق المتعلقة أو طرح سؤال آخر."
                        : "Sorry, there are no documents related to your question in the database. Please upload relevant documents or ask another question.";
                    
                    response.Response = noDocsMessage;
                    response.ConversationId = roomId.ToString();
                    return response;
                }

                // تحضير السياق المثرى للاستعلام
                string preparedContext = $"السؤال: {query}\n\nالمعلومات المتاحة من المستندات:\n{enrichedContext}";

                // الحصول على الرد من نموذج الذكاء الاصطناعي
                var aiResponse = await _deepSeekService.ProcessPdfDataAsync(preparedContext, language);

                // الحصول على الوثائق ذات الصلة لإرفاقها بالرد
                var relevantDocuments = await GetRelevantDocumentsAsync(query, userId, roomId, 5, language);

                // إنشاء كائن الاستجابة
                response.Response = aiResponse;
                response.ConversationId = roomId.ToString();
                response.DocumentReferences = relevantDocuments;
                response.TokenUsage = new TokenUsageDTO
                {
                    PromptTokens = EstimateTokens(preparedContext),
                    CompletionTokens = EstimateTokens(aiResponse),
                    TotalTokens = EstimateTokens(preparedContext) + EstimateTokens(aiResponse)
                };

                // تتبع المحادثة
                var conversationId = Guid.NewGuid().ToString();
                await _conversationTrackingService.LogConversationAsync(
                    userId.ToString(),
                    roomId.ToString(),
                    query,
                    aiResponse,
                    new Dictionary<string, object>
                    {
                        { "documentCount", relevantDocuments.Count },
                        { "language", language }
                    });

                // تتبع الكلمات المفتاحية
                await _conversationTrackingService.TrackKeywordsAsync(conversationId, ExtractKeywords(query));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على رد الذكاء الاصطناعي للمستخدم {UserId}", userId);
                
                var errorMessage = language == "ar"
                    ? "حدث خطأ أثناء معالجة طلبك. يرجى المحاولة مرة أخرى."
                    : "An error occurred while processing your request. Please try again.";
                
                return new AIQueryResponseDTO
                {
                    Response = errorMessage,
                    ConversationId = roomId.ToString(),
                    DocumentReferences = new List<DocumentReferenceDTO>()
                };
            }
        }

        #region Helper Methods

        /// <summary>
        /// استخراج الكلمات المفتاحية من نص
        /// </summary>
        private List<string> ExtractKeywords(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string>();

            // كلمات التوقف العربية المحسنة
            HashSet<string> arabicStopWords = new HashSet<string> {
                "من", "إلى", "في", "على", "عن", "أو", "مع", "هذا", "هذه", "ذلك", "تلك",
                "الذي", "التي", "هل", "كيف", "لماذا", "متى", "أين", "ما", "هو", "هي", "نحن", "هم",
                "كان", "كانت", "يكون", "تكون", "سوف", "سوى", "ليس", "ثم", "لكن", "و", "ا", "أن", "إن",
                "إذا", "حتى", "قد", "بل", "لقد", "فقط", "بين", "حول", "ضد", "بعد", "قبل", "فوق", "تحت"
            };

            // كلمات التوقف الإنجليزية
            HashSet<string> englishStopWords = new HashSet<string> {
                "the", "and", "a", "an", "in", "on", "at", "by", "to", "for", "with", "about",
                "from", "as", "into", "like", "through", "after", "over", "between", "out", "of",
                "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "do",
                "does", "did", "but", "if", "or", "because", "as", "until", "while", "that", "which"
            };

            // تقسيم النص إلى كلمات
            var words = text.Split(new[] { ' ', '.', ',', ';', ':', '!', '؟', '?', '\n', '\r', '(', ')', '[', ']', '{', '}' }, 
                                  StringSplitOptions.RemoveEmptyEntries);

            return words
                .Where(word => word.Length > 2) // تجاهل الكلمات القصيرة جدًا
                .Where(word => !arabicStopWords.Contains(word.ToLower()) && !englishStopWords.Contains(word.ToLower()))
                .Select(word => word.ToLower().Trim())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// استخراج العبارات من النص
        /// </summary>
        private List<string> ExtractPhrases(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string>();

            var phrases = new List<string>();
            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // استخراج العبارات المكونة من كلمتين متتاليتين
            for (int i = 0; i < words.Length - 1; i++)
            {
                if (words[i].Length > 2 && words[i + 1].Length > 2)
                {
                    phrases.Add($"{words[i]} {words[i + 1]}".ToLower());
                }
            }

            // استخراج العبارات المكونة من ثلاث كلمات متتالية
            for (int i = 0; i < words.Length - 2; i++)
            {
                if (words[i].Length > 2 && words[i + 1].Length > 2 && words[i + 2].Length > 2)
                {
                    phrases.Add($"{words[i]} {words[i + 1]} {words[i + 2]}".ToLower());
                }
            }

            return phrases.Distinct().ToList();
        }

        /// <summary>
        /// استخراج المصطلحات القانونية من النص
        /// </summary>
        private List<string> ExtractLegalTerms(string text, string language)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string>();

            // قائمة المصطلحات القانونية العربية
            HashSet<string> arabicLegalTerms = new HashSet<string> {
                "قانون", "تشريع", "لائحة", "مرسوم", "قرار", "حكم", "محكمة", "قضاء", "عقد", "اتفاقية",
                "دعوى", "خصوم", "المدعي", "المدعى عليه", "وكيل", "موكل", "شهادة", "شاهد", "حقوق", "التزامات",
                "أحكام", "ملكية", "عقار", "التعويض", "الضرر", "التأمين", "الإيجار", "البيع", "الشراء",
                "الميراث", "الوصية", "الهبة", "الرهن", "الشفعة", "الوكالة", "المحاماة", "النقض",
                "الاستئناف", "التمييز", "الدستور", "الملكية", "الحيازة"
            };

            // قائمة المصطلحات القانونية الإنجليزية
            HashSet<string> englishLegalTerms = new HashSet<string> {
                "law", "statute", "regulation", "decree", "judgment", "court", "judiciary", "contract",
                "agreement", "plaintiff", "defendant", "attorney", "client", "testimony", "witness",
                "rights", "obligations", "property", "compensation", "damage", "insurance", "lease",
                "sale", "purchase", "inheritance", "will", "donation", "mortgage", "preemption",
                "agency", "advocacy", "cassation", "appeal", "discrimination", "constitution", "ownership"
            };

            var legalTerms = new List<string>();
            var words = text.Split(new[] { ' ', '.', ',', ';', ':', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // اختيار قائمة المصطلحات المناسبة حسب اللغة
            var termsToCheck = language.ToLower() == "ar" ? arabicLegalTerms : englishLegalTerms;

            // البحث عن المصطلحات القانونية
            foreach (var word in words)
            {
                string normalizedWord = word.ToLower().Trim();
                if (termsToCheck.Contains(normalizedWord) && !legalTerms.Contains(normalizedWord))
                {
                    legalTerms.Add(normalizedWord);
                }
            }

            return legalTerms;
        }

        /// <summary>
        /// استخراج موضوع الاستعلام العام
        /// </summary>
        private string ExtractQueryTopic(string query, string language)
        {
            if (string.IsNullOrEmpty(query))
                return string.Empty;

            // قائمة المواضيع القانونية باللغة العربية
            Dictionary<string, List<string>> arabicTopics = new Dictionary<string, List<string>>
            {
                { "العقارات", new List<string> { "عقار", "أرض", "مبنى", "ملكية", "إيجار", "سكن", "مسكن" } },
                { "العقود", new List<string> { "عقد", "اتفاق", "اتفاقية", "شروط", "بنود", "توقيع", "إلزام" } },
                { "التقاضي", new List<string> { "دعوى", "محكمة", "قضية", "خصم", "حكم", "استئناف", "تمييز" } },
                { "الأحوال الشخصية", new List<string> { "زواج", "طلاق", "نفقة", "حضانة", "ميراث", "وصية", "نسب" } },
                { "القانون التجاري", new List<string> { "شركة", "تجارة", "أسهم", "مساهم", "إفلاس", "استثمار", "ترخيص" } },
                { "القانون الجنائي", new List<string> { "جريمة", "جناية", "جنحة", "عقوبة", "سجن", "غرامة", "متهم" } },
                { "قانون العمل", new List<string> { "عمل", "موظف", "أجر", "راتب", "فصل", "استقالة", "إجازة" } }
            };

            // قائمة المواضيع القانونية باللغة الإنجليزية
            Dictionary<string, List<string>> englishTopics = new Dictionary<string, List<string>>
            {
                { "Real Estate", new List<string> { "property", "land", "building", "ownership", "lease", "housing", "residence" } },
                { "Contracts", new List<string> { "contract", "agreement", "terms", "conditions", "signing", "binding", "obligation" } },
                { "Litigation", new List<string> { "lawsuit", "court", "case", "opponent", "judgment", "appeal", "cassation" } },
                { "Personal Status", new List<string> { "marriage", "divorce", "alimony", "custody", "inheritance", "will", "lineage" } },
                { "Commercial Law", new List<string> { "company", "trade", "shares", "shareholder", "bankruptcy", "investment", "license" } },
                { "Criminal Law", new List<string> { "crime", "felony", "misdemeanor", "punishment", "prison", "fine", "accused" } },
                { "Labor Law", new List<string> { "work", "employee", "wage", "salary", "termination", "resignation", "leave" } }
            };

            // تحديد قائمة المواضيع حسب اللغة
            var topics = language.ToLower() == "ar" ? arabicTopics : englishTopics;
            string bestTopic = "";
            int maxMatches = 0;

            foreach (var topic in topics)
            {
                int matches = 0;
                foreach (var keyword in topic.Value)
                {
                    if (query.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        matches++;
                    }
                }

                if (matches > maxMatches)
                {
                    maxMatches = matches;
                    bestTopic = topic.Key;
                }
            }

            return bestTopic;
        }

        /// <summary>
        /// حساب درجة الصلة بالكلمات المفتاحية
        /// </summary>
        private double CalculateKeywordRelevanceScore(string text, List<string> keywords)
        {
            if (string.IsNullOrEmpty(text) || keywords.Count == 0)
                return 0;

            int keywordsFound = 0;
            int keywordOccurrences = 0;

            foreach (var keyword in keywords)
            {
                // عدد مرات ظهور الكلمة المفتاحية في النص
                int occurrences = CountOccurrences(text, keyword);
                if (occurrences > 0)
                {
                    keywordsFound++;
                    keywordOccurrences += occurrences;
                }
            }

            // نسبة الكلمات المفتاحية الموجودة
            double keywordCoverage = (double)keywordsFound / keywords.Count;
            
            // كثافة الكلمات المفتاحية في النص
            int totalWords = text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            double keywordDensity = totalWords > 0 ? (double)keywordOccurrences / totalWords : 0;

            // الدرجة النهائية: مزيج من التغطية والكثافة
            return (keywordCoverage * 0.7) + (keywordDensity * 20.0 * 0.3);
        }

        /// <summary>
        /// حساب درجة الصلة بالعبارات
        /// </summary>
        private double CalculatePhraseRelevanceScore(string text, List<string> phrases)
        {
            if (string.IsNullOrEmpty(text) || phrases.Count == 0)
                return 0;

            int phrasesFound = 0;
            int exactPhraseOccurrences = 0;

            foreach (var phrase in phrases)
            {
                // عدد مرات ظهور العبارة بالضبط في النص
                int occurrences = CountOccurrences(text, phrase);
                if (occurrences > 0)
                {
                    phrasesFound++;
                    exactPhraseOccurrences += occurrences;
                }
            }

            // عبارات دقيقة تعطي درجة أعلى (وزن مضاعف)
            double exactPhraseWeight = phrases.Count > 0 ? (double)phrasesFound / phrases.Count : 0;
            
            // أهمية العثور على عبارات كاملة أكبر من أهمية الكلمات الفردية
            return exactPhraseWeight * 2.0;
        }

        /// <summary>
        /// حساب درجة الصلة بالمصطلحات القانونية
        /// </summary>
        private double CalculateLegalTermRelevanceScore(string text, List<string> legalTerms)
        {
            if (string.IsNullOrEmpty(text) || legalTerms.Count == 0)
                return 0;

            int termsFound = 0;
            int termOccurrences = 0;

            foreach (var term in legalTerms)
            {
                int occurrences = CountOccurrences(text, term);
                if (occurrences > 0)
                {
                    termsFound++;
                    termOccurrences += occurrences;
                }
            }

            // المصطلحات القانونية لها أهمية خاصة
            double termCoverage = legalTerms.Count > 0 ? (double)termsFound / legalTerms.Count : 0;
            return termCoverage * 1.5; // وزن أعلى للمصطلحات القانونية
        }

        /// <summary>
        /// حساب درجة الصلة بالموضوع
        /// </summary>
        private double CalculateTopicRelevanceScore(string text, string topic)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(topic))
                return 0;

            // تحقق ما إذا كان النص يحتوي على الموضوع
            bool containsTopic = text.IndexOf(topic, StringComparison.OrdinalIgnoreCase) >= 0;
            
            // درجة أساسية إذا كان الموضوع موجودًا
            double baseScore = containsTopic ? 0.5 : 0;
            
            // تعزيز بناءً على كثافة المصطلحات المتعلقة بالموضوع
            double density = CountOccurrences(text, topic) / (double)text.Length;
            
            return baseScore + (density * 10);
        }

        /// <summary>
        /// عد مرات ظهور نمط في نص
        /// </summary>
        private int CountOccurrences(string text, string pattern)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern))
                return 0;

            int count = 0;
            int index = 0;

            while ((index = text.IndexOf(pattern, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                count++;
                index += pattern.Length;
            }

            return count;
        }

        /// <summary>
        /// الحصول على مقطع ذي صلة من نص
        /// </summary>
        private string GetRelevantSnippet(string text, List<string> keywords, List<string> phrases, string query, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // البحث عن أفضل مطابقة في النص
            int bestMatchPos = -1;
            double bestMatchScore = -1;

            // تقسيم النص إلى فقرات
            string[] paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            // قائمة بدرجات كل فقرة
            var paragraphScores = new Dictionary<int, double>();

            // تقييم كل فقرة
            for (int i = 0; i < paragraphs.Length; i++)
            {
                string paragraph = paragraphs[i];
                
                // حساب درجة الفقرة بناء على تطابق الكلمات المفتاحية
                double keywordScore = 0;
                foreach (var keyword in keywords)
                {
                    if (paragraph.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        keywordScore += 1.0;
                    }
                }
                
                // حساب درجة الفقرة بناء على تطابق العبارات (وزن مضاعف)
                double phraseScore = 0;
                foreach (var phrase in phrases)
                {
                    if (paragraph.IndexOf(phrase, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        phraseScore += 2.0;
                    }
                }
                
                // تحقق ما إذا كانت الفقرة تحتوي على الاستعلام كاملاً (أهمية قصوى)
                double queryScore = paragraph.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ? 5.0 : 0;
                
                // الدرجة النهائية للفقرة
                double totalScore = keywordScore + phraseScore + queryScore;
                paragraphScores[i] = totalScore;
                
                // تحديث أفضل مطابقة
                if (totalScore > bestMatchScore)
                {
                    bestMatchScore = totalScore;
                    bestMatchPos = i;
                }
            }

            // إذا لم يتم العثور على مطابقة جيدة، استخدم الفقرة الأولى
            if (bestMatchPos == -1)
            {
                bestMatchPos = 0;
            }

            // بناء المقطع من الفقرات ذات الصلة
            StringBuilder snippet = new StringBuilder();
            
            // إضافة الفقرة الأعلى تقييمًا
            snippet.AppendLine(paragraphs[bestMatchPos]);
            
            // إضافة فقرة إضافية ذات الصلة الأعلى (إذا وجدت)
            if (paragraphs.Length > 1)
            {
                // التحقق من الفقرات السابقة واللاحقة
                var candidateParagraphs = new Dictionary<int, double>();
                
                if (bestMatchPos > 0)
                {
                    candidateParagraphs[bestMatchPos - 1] = paragraphScores[bestMatchPos - 1];
                }
                
                if (bestMatchPos < paragraphs.Length - 1)
                {
                    candidateParagraphs[bestMatchPos + 1] = paragraphScores[bestMatchPos + 1];
                }
                
                // إضافة الفقرة ذات الصلة الأعلى
                if (candidateParagraphs.Count > 0)
                {
                    int nextBestPos = candidateParagraphs.OrderByDescending(p => p.Value).First().Key;
                    if (nextBestPos < bestMatchPos)
                    {
                        snippet.Insert(0, paragraphs[nextBestPos] + "\n\n");
                    }
                    else
                    {
                        snippet.AppendLine("\n" + paragraphs[nextBestPos]);
                    }
                }
            }

            // اقتصاص النص إلى الطول المطلوب مع التأكد من عدم قطع الجمل
            string result = snippet.ToString();
            if (result.Length > maxLength)
            {
                // قطع النص مع مراعاة حدود الجمل
                string truncated = result.Substring(0, maxLength);
                int lastPeriod = Math.Max(
                    truncated.LastIndexOf('.'),
                    Math.Max(truncated.LastIndexOf('؟'), truncated.LastIndexOf('!'))
                );
                
                if (lastPeriod > maxLength / 2)
                {
                    // إذا كانت هناك نقطة في النصف الثاني من النص، قم بالقطع عندها
                    truncated = truncated.Substring(0, lastPeriod + 1);
                }
                
                result = truncated + "...";
            }

            return result;
        }

        /// <summary>
        /// محاولة تحديد رقم الصفحة من مقطع نصي
        /// </summary>
        private int? TryDetectPageNumber(string fullText, string snippet)
        {
            if (string.IsNullOrEmpty(fullText) || string.IsNullOrEmpty(snippet))
                return null;

            try
            {
                // البحث عن رقم صفحة في المقطع المحدد
                var pageNumberMatches = Regex.Matches(snippet, @"(?:صفحة|page)\s*(\d+)", RegexOptions.IgnoreCase);
                if (pageNumberMatches.Count > 0 && int.TryParse(pageNumberMatches[0].Groups[1].Value, out int pageNumber))
                {
                    return pageNumber;
                }

                // البحث عن رقم صفحة قريب من المقطع في النص الكامل
                int snippetIndex = fullText.IndexOf(snippet, StringComparison.OrdinalIgnoreCase);
                if (snippetIndex >= 0)
                {
                    // البحث في نطاق 500 حرف قبل المقطع
                    int searchStartIndex = Math.Max(0, snippetIndex - 500);
                    string textBeforeSnippet = fullText.Substring(searchStartIndex, snippetIndex - searchStartIndex);
                    
                    pageNumberMatches = Regex.Matches(textBeforeSnippet, @"(?:صفحة|page)\s*(\d+)", RegexOptions.IgnoreCase);
                    if (pageNumberMatches.Count > 0 && int.TryParse(pageNumberMatches[pageNumberMatches.Count - 1].Groups[1].Value, out pageNumber))
                    {
                        return pageNumber;
                    }
                }

                return null;
            }
            catch
            {
                return null; // في حالة حدوث أي خطأ، عدم إرجاع رقم صفحة
            }
        }

        /// <summary>
        /// تقدير عدد الرموز في نص
        /// </summary>
        private int EstimateTokens(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // تقدير محسن: يعتمد على متوسط طول الكلمة وعدد الكلمات
            var words = text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int wordCount = words.Length;
            
            // العربية قد تكون أكثر كثافة من حيث الرموز مقارنة بالإنجليزية
            // ترميز GPT يستخدم تقريبًا رمزًا واحدًا لكل 3-4 أحرف في المتوسط للغات اللاتينية
            // ولكل 1-2 حرف للغات غير اللاتينية مثل العربية
            double averageTokensPerWord = 1.5; 
            
            return (int)(wordCount * averageTokensPerWord);
        }

        /// <summary>
        /// تتبع المراجع المستخدمة في المحادثة
        /// </summary>
        private async Task TrackDocumentReferencesAsync(string conversationId, long userId, List<DocumentReferenceDTO> documents)
        {
            try
            {
                var pdfReferences = new Dictionary<string, int>();
                
                foreach (var doc in documents)
                {
                    pdfReferences[doc.DocumentName] = (int)(doc.RelevanceScore * 100);
                }
                
                await _conversationTrackingService.TrackPdfReferencesAsync(conversationId, pdfReferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تتبع مراجع المستندات للمحادثة {ConversationId}", conversationId);
            }
        }

        #endregion
    }
} 