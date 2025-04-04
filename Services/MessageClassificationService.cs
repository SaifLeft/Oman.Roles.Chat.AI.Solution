using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs;

namespace Services
{
    /// <summary>
    /// واجهة خدمة تصنيف الرسائل وتحليل المشاعر
    /// </summary>
    public interface IMessageClassificationService
    {
        /// <summary>
        /// تصنيف رسالة تلقائيًا
        /// </summary>
        Task<BaseResponse<MessageCategoryDTO>> ClassifyMessageAsync(int messageId, string language);

        /// <summary>
        /// تحليل مشاعر الرسالة
        /// </summary>
        Task<BaseResponse<MessageSentimentDTO>> AnalyzeSentimentAsync(int messageId, string language);

        /// <summary>
        /// الحصول على فئات الرسالة
        /// </summary>
        Task<BaseResponse<List<MessageCategoryDTO>>> GetMessageCategoriesAsync(int messageId, string language);

        /// <summary>
        /// تعيين فئة الرسالة يدويًا
        /// </summary>
        Task<BaseResponse<MessageCategoryDTO>> SetMessageCategoryAsync(int messageId, int categoryId, string language);

        /// <summary>
        /// الحصول على جميع الفئات القانونية
        /// </summary>
        Task<BaseResponse<List<LegalCategoryDTO>>> GetLegalCategoriesAsync(string language);

        /// <summary>
        /// الحصول على ملخص تصنيف الاستعلامات
        /// </summary>
        Task<BaseResponse<QueryCategorySummaryDTO>> GetCategorySummaryAsync(DateTime fromDate, DateTime toDate, string language);
    }

    /// <summary>
    /// تنفيذ خدمة تصنيف الرسائل وتحليل المشاعر
    /// </summary>
    public class MessageClassificationService : IMessageClassificationService
    {
        private readonly MuhamiContext _context;
        private readonly ILogger<MessageClassificationService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MessageClassificationService(
            MuhamiContext context,
            ILogger<MessageClassificationService> logger,
            ILocalizationService localizationService,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("AIServices");
        }

        /// <summary>
        /// تصنيف رسالة تلقائيًا
        /// </summary>
        public async Task<BaseResponse<MessageCategoryDTO>> ClassifyMessageAsync(int messageId, string language)
        {
            try
            {
                // التحقق من وجود الرسالة
                var message = await _context.ChatMessages
                    .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

                if (message == null)
                {
                    var errorMessage = _localizationService.GetMessage("MessageNotFound", "Errors", language);
                    return BaseResponse<MessageCategoryDTO>.FailureResponse(errorMessage, 404);
                }

                // التحقق مما إذا كانت الرسالة مصنفة بالفعل
                var existingCategory = await _context.MessageCategories
                    .FirstOrDefaultAsync(c => c.MessageId == messageId && !c.IsDeleted);

                if (existingCategory != null)
                {
                    var category = await _context.LegalCategories
                        .FirstOrDefaultAsync(c => c.Id == existingCategory.LegalCategoryId);

                    var existingDto = new MessageCategoryDTO
                    {
                        Id = existingCategory.Id,
                        MessageId = existingCategory.MessageId,
                        CategoryId = existingCategory.LegalCategoryId,
                        CategoryName = category?.Name ?? "Unknown",
                        Confidence = (float)existingCategory.Confidence,
                        IsAutoClassified = existingCategory.IsAutoClassified,
                        CreateDate = existingCategory.CreateDate
                    };

                    var successMessage = _localizationService.GetMessage("MessageAlreadyClassified", "Messages", language);
                    return BaseResponse<MessageCategoryDTO>.SuccessResponse(existingDto, successMessage);
                }

                // الحصول على جميع الفئات القانونية
                var categories = await _context.LegalCategories
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .ToListAsync();

                // استدعاء خدمة الذكاء الاصطناعي لتصنيف الرسالة
                // في هذا المثال، سنستخدم تصنيفًا مبسطًا يعتمد على الكلمات المفتاحية
                // في التطبيق الواقعي، يجب استخدام خدمة ذكاء اصطناعي متخصصة

                // تنفيذ تصنيف بسيط باستخدام الكلمات المفتاحية
                var categoryResults = await SimpleCategoryClassifierAsync(message.Content, categories);

                // اختيار الفئة ذات أعلى ثقة
                var topCategory = categoryResults.OrderByDescending(c => c.Confidence).First();

                // حفظ التصنيف في قاعدة البيانات
                var newCategory = new MessageCategory
                {
                    MessageId = messageId,
                    LegalCategoryId = topCategory.CategoryId,
                    Confidence = (decimal)topCategory.Confidence,
                    IsAutoClassified = true,
                    CreateDate = DateTime.Now,
                    CreatedByUserId = message.CreatedByUserId
                };

                _context.MessageCategories.Add(newCategory);
                await _context.SaveChangesAsync();

                // إنشاء كائن النتيجة
                var result = new MessageCategoryDTO
                {
                    Id = newCategory.Id,
                    MessageId = newCategory.MessageId,
                    CategoryId = newCategory.LegalCategoryId,
                    CategoryName = categories.First(c => c.Id == newCategory.LegalCategoryId).Name,
                    Confidence = (float)newCategory.Confidence,
                    IsAutoClassified = newCategory.IsAutoClassified,
                    CreateDate = newCategory.CreateDate
                };

                var message2 = _localizationService.GetMessage("MessageClassified", "Messages", language);
                return BaseResponse<MessageCategoryDTO>.SuccessResponse(result, message2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تصنيف الرسالة");
                var errorMessage = _localizationService.GetMessage("MessageClassificationError", "Errors", language);
                return BaseResponse<MessageCategoryDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحليل مشاعر الرسالة
        /// </summary>
        public async Task<BaseResponse<MessageSentimentDTO>> AnalyzeSentimentAsync(int messageId, string language)
        {
            try
            {
                // التحقق من وجود الرسالة
                var message = await _context.ChatMessages
                    .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

                if (message == null)
                {
                    var errorMessage = _localizationService.GetMessage("MessageNotFound", "Errors", language);
                    return BaseResponse<MessageSentimentDTO>.FailureResponse(errorMessage, 404);
                }

                // التحقق مما إذا كانت الرسالة محللة المشاعر بالفعل
                if (message.SentimentScore.HasValue)
                {
                    var existingDto = new MessageSentimentDTO
                    {
                        MessageId = message.Id,
                        SentimentScore = (float)message.SentimentScore.Value,
                        Urgency = message.Urgency ?? 1,
                        EmotionalTone = message.EmotionalTone
                    };

                    var successMessage = _localizationService.GetMessage("MessageSentimentAlreadyAnalyzed", "Messages", language);
                    return BaseResponse<MessageSentimentDTO>.SuccessResponse(existingDto, successMessage);
                }

                // استدعاء خدمة الذكاء الاصطناعي لتحليل المشاعر
                // في هذا المثال، سنستخدم تحليلًا مبسطًا يعتمد على الكلمات المفتاحية
                // في التطبيق الواقعي، يجب استخدام خدمة ذكاء اصطناعي متخصصة

                var sentimentResult = await SimpleSentimentAnalyzerAsync(message.Content);

                // حفظ تحليل المشاعر في قاعدة البيانات
                message.SentimentScore = sentimentResult.SentimentScore;
                message.Urgency = sentimentResult.Urgency;
                message.EmotionalTone = sentimentResult.EmotionalTone;

                await _context.SaveChangesAsync();

                // إنشاء كائن النتيجة
                var result = new MessageSentimentDTO
                {
                    MessageId = message.Id,
                    SentimentScore = (float)message.SentimentScore.Value,
                    Urgency = message.Urgency.Value,
                    EmotionalTone = message.EmotionalTone
                };

                var successMessage2 = _localizationService.GetMessage("MessageSentimentAnalyzed", "Messages", language);
                return BaseResponse<MessageSentimentDTO>.SuccessResponse(result, successMessage2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحليل مشاعر الرسالة");
                var errorMessage = _localizationService.GetMessage("MessageSentimentError", "Errors", language);
                return BaseResponse<MessageSentimentDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على فئات الرسالة
        /// </summary>
        public async Task<BaseResponse<List<MessageCategoryDTO>>> GetMessageCategoriesAsync(int messageId, string language)
        {
            try
            {
                var categories = await _context.MessageCategories
                    .Where(c => c.MessageId == messageId && !c.IsDeleted)
                    .Join(_context.LegalCategories,
                          mc => mc.LegalCategoryId,
                          lc => lc.Id,
                          (mc, lc) => new MessageCategoryDTO
                          {
                              Id = mc.Id,
                              MessageId = mc.MessageId,
                              CategoryId = mc.LegalCategoryId,
                              CategoryName = lc.Name,
                              Confidence = (float)mc.Confidence,
                              IsAutoClassified = mc.IsAutoClassified,
                              CreateDate = mc.CreateDate
                          })
                    .ToListAsync();

                var successMessage = _localizationService.GetMessage("MessageCategoriesRetrieved", "Messages", language);
                return BaseResponse<List<MessageCategoryDTO>>.SuccessResponse(categories, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على فئات الرسالة");
                var errorMessage = _localizationService.GetMessage("MessageCategoriesError", "Errors", language);
                return BaseResponse<List<MessageCategoryDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تعيين فئة الرسالة يدويًا
        /// </summary>
        public async Task<BaseResponse<MessageCategoryDTO>> SetMessageCategoryAsync(int messageId, int categoryId, string language)
        {
            try
            {
                // التحقق من وجود الرسالة
                var message = await _context.ChatMessages
                    .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);

                if (message == null)
                {
                    var errorMessage = _localizationService.GetMessage("MessageNotFound", "Errors", language);
                    return BaseResponse<MessageCategoryDTO>.FailureResponse(errorMessage, 404);
                }

                // التحقق من وجود الفئة
                var category = await _context.LegalCategories
                    .FirstOrDefaultAsync(c => c.Id == categoryId && c.IsActive && !c.IsDeleted);

                if (category == null)
                {
                    var errorMessage = _localizationService.GetMessage("CategoryNotFound", "Errors", language);
                    return BaseResponse<MessageCategoryDTO>.FailureResponse(errorMessage, 404);
                }

                // البحث عن تصنيف موجود
                var existingCategory = await _context.MessageCategories
                    .FirstOrDefaultAsync(c => c.MessageId == messageId && !c.IsDeleted);

                if (existingCategory != null)
                {
                    // تحديث التصنيف الموجود
                    existingCategory.LegalCategoryId = categoryId;
                    existingCategory.Confidence = (decimal)1.0f; // ثقة كاملة للتصنيف اليدوي
                    existingCategory.IsAutoClassified = false;
                    existingCategory.ModifiedDate = DateTime.Now;

                    await _context.SaveChangesAsync();

                    var result = new MessageCategoryDTO
                    {
                        Id = existingCategory.Id,
                        MessageId = existingCategory.MessageId,
                        CategoryId = existingCategory.LegalCategoryId,
                        CategoryName = category.Name,
                        Confidence = (float)existingCategory.Confidence,
                        IsAutoClassified = existingCategory.IsAutoClassified,
                        CreateDate = existingCategory.CreateDate
                    };

                    var successMessage = _localizationService.GetMessage("MessageCategoryUpdated", "Messages", language);
                    return BaseResponse<MessageCategoryDTO>.SuccessResponse(result, successMessage);
                }
                else
                {
                    // إنشاء تصنيف جديد
                    var newCategory = new MessageCategory
                    {
                        MessageId = messageId,
                        LegalCategoryId = categoryId,
                        Confidence = (decimal)1.0f, // ثقة كاملة للتصنيف اليدوي
                        IsAutoClassified = false,
                        CreateDate = DateTime.Now,
                        CreatedByUserId = message.CreatedByUserId
                    };

                    _context.MessageCategories.Add(newCategory);
                    await _context.SaveChangesAsync();

                    var result = new MessageCategoryDTO
                    {
                        Id = newCategory.Id,
                        MessageId = newCategory.MessageId,
                        CategoryId = newCategory.LegalCategoryId,
                        CategoryName = category.Name,
                        Confidence = (float)newCategory.Confidence,
                        IsAutoClassified = newCategory.IsAutoClassified,
                        CreateDate = newCategory.CreateDate
                    };

                    var successMessage = _localizationService.GetMessage("MessageCategoryCreated", "Messages", language);
                    return BaseResponse<MessageCategoryDTO>.SuccessResponse(result, successMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تعيين فئة الرسالة");
                var errorMessage = _localizationService.GetMessage("MessageCategoryError", "Errors", language);
                return BaseResponse<MessageCategoryDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على جميع الفئات القانونية
        /// </summary>
        public async Task<BaseResponse<List<LegalCategoryDTO>>> GetLegalCategoriesAsync(string language)
        {
            try
            {
                var categories = await _context.LegalCategories
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .Select(c => new LegalCategoryDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    })
                    .ToListAsync();

                var successMessage = _localizationService.GetMessage("LegalCategoriesRetrieved", "Messages", language);
                return BaseResponse<List<LegalCategoryDTO>>.SuccessResponse(categories, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على الفئات القانونية");
                var errorMessage = _localizationService.GetMessage("LegalCategoriesError", "Errors", language);
                return BaseResponse<List<LegalCategoryDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على ملخص تصنيف الاستعلامات
        /// </summary>
        public async Task<BaseResponse<QueryCategorySummaryDTO>> GetCategorySummaryAsync(DateTime fromDate, DateTime toDate, string language)
        {
            try
            {
                // الحصول على توزيع الاستعلامات حسب الفئة
                var distribution = await _context.MessageCategories
                    .Where(c => c.CreateDate >= fromDate && c.CreateDate <= toDate && !c.IsDeleted)
                    .Join(_context.LegalCategories,
                          mc => mc.LegalCategoryId,
                          lc => lc.Id,
                          (mc, lc) => new { mc, lc })
                    .GroupBy(x => x.lc.Name)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var distributionDict = distribution.ToDictionary(x => x.Category, x => x.Count);

                // حساب متوسط درجة الثقة
                var averageConfidence = await _context.MessageCategories
                    .Where(c => c.CreateDate >= fromDate && c.CreateDate <= toDate && !c.IsDeleted)
                    .AverageAsync(c => c.Confidence);

                // حساب عدد الرسائل المصنفة تلقائيًا ويدويًا
                var autoClassifiedCount = await _context.MessageCategories
                    .CountAsync(c => c.CreateDate >= fromDate && c.CreateDate <= toDate && !c.IsDeleted && c.IsAutoClassified);

                var manuallyClassifiedCount = await _context.MessageCategories
                    .CountAsync(c => c.CreateDate >= fromDate && c.CreateDate <= toDate && !c.IsDeleted && !c.IsAutoClassified);

                var summary = new QueryCategorySummaryDTO
                {
                    DistributionByCategory = distributionDict,
                    AverageConfidence = (float)averageConfidence,
                    AutoClassifiedCount = autoClassifiedCount,
                    ManuallyClassifiedCount = manuallyClassifiedCount
                };

                var successMessage = _localizationService.GetMessage("CategorySummaryRetrieved", "Messages", language);
                return BaseResponse<QueryCategorySummaryDTO>.SuccessResponse(summary, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على ملخص تصنيف الاستعلامات");
                var errorMessage = _localizationService.GetMessage("CategorySummaryError", "Errors", language);
                return BaseResponse<QueryCategorySummaryDTO>.FailureResponse(errorMessage, 500);
            }
        }

        #region Helper Methods

        /// <summary>
        /// مصنف فئات بسيط يعتمد على الكلمات المفتاحية
        /// في تطبيق واقعي، يجب استخدام خدمة ذكاء اصطناعي متخصصة
        /// </summary>
        private async Task<List<MessageCategoryDTO>> SimpleCategoryClassifierAsync(string text, List<LegalCategory> categories)
        {
            // قاموس للكلمات المفتاحية لكل فئة
            var keywordsByCategoryId = new Dictionary<long, List<string>>
            {
                // قانون تجاري
                { 1, new List<string> { "شركة", "تجارة", "استثمار", "عقد تجاري", "مساهمة", "أسهم", "مستثمر", "تاجر", "منافسة", "سجل تجاري" } },
                // قانون مدني
                { 2, new List<string> { "عقد", "التزام", "تعويض", "مسؤولية", "إيجار", "ملكية", "مدني", "مستهلك", "إخلال", "ضرر" } },
                // قانون أسرة
                { 3, new List<string> { "زواج", "طلاق", "حضانة", "نفقة", "أسرة", "ميراث", "وصاية", "أولاد", "نسب", "مهر" } },
                // قانون عقاري
                { 4, new List<string> { "عقار", "أرض", "بناء", "إيجار", "ملكية", "رهن", "تمليك", "عقد إيجار", "مستأجر", "مؤجر" } },
                // قانون جنائي
                { 5, new List<string> { "جريمة", "جناية", "جنحة", "عقوبة", "سجن", "غرامة", "متهم", "تحقيق", "محكمة جنائية", "حكم" } },
                // قانون العمل
                { 6, new List<string> { "موظف", "عامل", "مرتب", "أجور", "استقالة", "فصل", "إجازة", "عقد عمل", "تأمينات", "رب عمل" } },
                // قانون ضرائب
                { 7, new List<string> { "ضريبة", "ضرائب", "قيمة مضافة", "دخل", "إقرار ضريبي", "تهرب ضريبي", "إعفاء", "خصم", "استرداد", "مصلحة الضرائب" } },
                // قانون الملكية الفكرية
                { 8, new List<string> { "حقوق", "براءة", "تأليف", "نشر", "علامة تجارية", "ملكية فكرية", "اختراع", "نماذج", "رسوم", "قرصنة" } }
            };

            // النتائج
            var results = new List<MessageCategoryDTO>();

            // تحويل النص إلى أحرف صغيرة للمقارنة
            var lowercaseText = text.ToLower();

            // حساب نقاط لكل فئة بناءً على الكلمات المفتاحية الموجودة
            foreach (var category in categories)
            {
                if (keywordsByCategoryId.TryGetValue(category.Id, out var keywords))
                {
                    int matchCount = 0;
                    foreach (var keyword in keywords)
                    {
                        if (lowercaseText.Contains(keyword.ToLower()))
                        {
                            matchCount++;
                        }
                    }

                    float confidence = 0;
                    if (keywords.Count > 0)
                    {
                        confidence = (float)matchCount / keywords.Count;
                    }

                    // إضافة نتيجة لكل فئة
                    results.Add(new MessageCategoryDTO
                    {
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        Confidence = confidence
                    });
                }
                else
                {
                    // إضافة فئة بثقة منخفضة إذا لم تكن هناك كلمات مفتاحية
                    results.Add(new MessageCategoryDTO
                    {
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        Confidence = 0.1f
                    });
                }
            }

            // إذا لم يتم العثور على أي تطابق، استخدم الفئة "قوانين أخرى"
            if (results.All(r => r.Confidence < 0.3f))
            {
                var otherCategory = categories.FirstOrDefault(c => c.Name == "قوانين أخرى");
                if (otherCategory != null)
                {
                    results.First(r => r.CategoryId == otherCategory.Id).Confidence = 0.7f;
                }
            }

            // محاكاة تأخير الاستدعاء البعيد
            await Task.Delay(100);

            return results;
        }

        /// <summary>
        /// محلل مشاعر بسيط يعتمد على الكلمات المفتاحية
        /// في تطبيق واقعي، يجب استخدام خدمة ذكاء اصطناعي متخصصة
        /// </summary>
        private async Task<MessageSentimentDTO> SimpleSentimentAnalyzerAsync(string text)
        {
            // قوائم الكلمات المفتاحية للمشاعر
            var positiveWords = new List<string> { "شكرا", "ممتاز", "جيد", "رائع", "أتمنى", "أرجو", "أشكر", "ساعدني", "من فضلك" };
            var negativeWords = new List<string> { "مشكلة", "سيء", "غضب", "إحباط", "عصبي", "قلق", "خلاف", "صعب", "مزعج", "لا أستطيع" };
            var urgentWords = new List<string> { "عاجل", "ضروري", "فوري", "الآن", "سريع", "استعجال", "طارئ", "حرج", "خطير", "مستعجل" };

            // تحويل النص إلى أحرف صغيرة للمقارنة
            var lowercaseText = text.ToLower();

            // حساب عدد الكلمات الإيجابية والسلبية
            int positiveCount = 0;
            int negativeCount = 0;

            foreach (var word in positiveWords)
            {
                if (lowercaseText.Contains(word.ToLower()))
                {
                    positiveCount++;
                }
            }

            foreach (var word in negativeWords)
            {
                if (lowercaseText.Contains(word.ToLower()))
                {
                    negativeCount++;
                }
            }

            // حساب درجة المشاعر (-1 إلى 1)
            float sentimentScore = 0;
            int totalWords = positiveWords.Count + negativeWords.Count;
            if (totalWords > 0)
            {
                sentimentScore = ((float)positiveCount - negativeCount) / Math.Max(positiveCount + negativeCount, 1);
            }

            // حساب درجة الإلحاح (1-3)
            int urgencyScore = 1; // افتراضي: منخفض

            int urgentCount = 0;
            foreach (var word in urgentWords)
            {
                if (lowercaseText.Contains(word.ToLower()))
                {
                    urgentCount++;
                }
            }

            if (urgentCount >= 3)
            {
                urgencyScore = 3; // عالي
            }
            else if (urgentCount >= 1)
            {
                urgencyScore = 2; // متوسط
            }

            // تحديد النبرة العاطفية
            string emotionalTone = "محايد";
            if (sentimentScore <= -0.5)
            {
                emotionalTone = "غضب";
            }
            else if (sentimentScore < 0)
            {
                emotionalTone = "قلق";
            }
            else if (sentimentScore >= 0.5)
            {
                emotionalTone = "رضا";
            }
            else if (sentimentScore > 0)
            {
                emotionalTone = "أمل";
            }

            // محاكاة تأخير الاستدعاء البعيد
            await Task.Delay(100);

            return new MessageSentimentDTO
            {
                SentimentScore = sentimentScore,
                Urgency = urgencyScore,
                EmotionalTone = emotionalTone
            };
        }

        #endregion
    }
}