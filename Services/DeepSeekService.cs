using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Services
{
    /// <summary>
    /// واجهة خدمة الاتصال بـ DeepSeek للحصول على إجابات ذكية حول الاستشارات القانونية
    /// </summary>
    public interface IDeepSeekService
    {
        /// <summary>
        /// معالجة بيانات PDF واستخراج الإجابات المناسبة
        /// </summary>
        /// <param name="prompt">الاستعلام المطلوب معالجته</param>
        /// <param name="language">لغة الاستعلام والاستجابة</param>
        /// <returns>استجابة النموذج</returns>
        Task<string> ProcessPdfDataAsync(string prompt, string language = "ar");

        /// <summary>
        /// تنفيذ استعلام قانوني مع سياق محدد
        /// </summary>
        /// <param name="query">استعلام المستخدم</param>
        /// <param name="context">السياق القانوني المعطى</param>
        /// <param name="language">اللغة المطلوبة</param>
        /// <returns>استجابة قانونية</returns>
        Task<string> ExecuteLegalQueryAsync(string query, string context, string language = "ar");

        /// <summary>
        /// تحليل مستند PDF وتلخيصه
        /// </summary>
        /// <param name="pdfContent">محتوى ملف PDF</param>
        /// <param name="language">اللغة المطلوبة</param>
        /// <returns>ملخص للمستند</returns>
        Task<string> AnalyzePdfContentAsync(string pdfContent, string language = "ar");

        /// <summary>
        /// الاستعلام عن إجراءات حكومية محددة
        /// </summary>
        /// <param name="service">اسم الخدمة الحكومية</param>
        /// <param name="language">اللغة المطلوبة</param>
        /// <returns>إرشادات حول الإجراء</returns>
        Task<string> GetGovernmentProcedureAsync(string service, string language = "ar");
    }



    /// <summary>
    /// تنفيذ خدمة DeepSeek المتخصصة في الاستشارات القانونية والحكومية في سلطنة عمان
    /// </summary>
    public class DeepSeekService : IDeepSeekService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeepSeekService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly string _model;
        private readonly int _maxRetries = 3;

        /// <summary>
        /// إنشاء مثيل جديد من خدمة DeepSeek
        /// </summary>
        public DeepSeekService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<DeepSeekService> logger,
            ILocalizationService localizationService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;

            // استخراج بيانات التكوين
            _apiKey = _configuration["DeepSeekAPI:ApiKey"] ?? throw new InvalidOperationException("DeepSeek API Key is missing in configuration");
            _endpoint = _configuration["DeepSeekAPI:Endpoint"] ?? "https://api.deepseek.com/v1/chat/completions";
            _model = _configuration["DeepSeekAPI:Model"] ?? "deepseek-chat";

            // تهيئة عميل HTTP
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromMinutes(2); // زيادة مهلة الانتظار للردود الطويلة
        }

        /// <summary>
        /// معالجة محتوى الـ PDF والاستعلام وإرجاع إجابة مناسبة
        /// </summary>
        public async Task<string> ProcessPdfDataAsync(string prompt, string language = "ar")
        {
            try
            {
                _logger.LogInformation("جاري معالجة استعلام PDF: {PromptStart}...", prompt.Substring(0, Math.Min(100, prompt.Length)));

                // إضافة سياق خاص بالاستشارات القانونية العمانية
                var enhancedPrompt = EnhanceLegalPrompt(prompt, language);

                // تحضير الطلب
                var messages = new List<ChatMessageDTO>
                {
                    new ChatMessageDTO { Role = "system", Content = GetLegalSystemPrompt(language) },
                    new ChatMessageDTO { Role = "user", Content = enhancedPrompt }
                };

                var response = await SendRequestToDeepSeekAsync(messages);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء معالجة استعلام PDF");
                var errorMessage = _localizationService.GetMessage("ApiError", "Errors", language);
                return $"{errorMessage}: {ex.Message}";
            }
        }

        /// <summary>
        /// تنفيذ استعلام قانوني مع سياق محدد
        /// </summary>
        public async Task<string> ExecuteLegalQueryAsync(string query, string context, string language = "ar")
        {
            try
            {
                _logger.LogInformation("تنفيذ استعلام قانوني: {QueryStart}...", query.Substring(0, Math.Min(50, query.Length)));

                // تحضير سياق السؤال القانوني
                var legalContext = $"### السياق القانوني:\n{context}\n\n### السؤال:\n{query}";

                // تحضير الطلب
                var messages = new List<ChatMessageDTO>
                {
                    new ChatMessageDTO { Role = "system", Content = GetLegalSystemPrompt(language) },
                    new ChatMessageDTO { Role = "user", Content = legalContext }
                };

                var response = await SendRequestToDeepSeekAsync(messages, 0.5); // استخدام حرارة أقل للإجابات القانونية الأكثر دقة
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تنفيذ استعلام قانوني");
                var errorMessage = _localizationService.GetMessage("LegalQueryError", "Errors", language);
                return $"{errorMessage}: {ex.Message}";
            }
        }

        /// <summary>
        /// تحليل محتوى PDF وتلخيصه
        /// </summary>
        public async Task<string> AnalyzePdfContentAsync(string pdfContent, string language = "ar")
        {
            try
            {
                _logger.LogInformation("تحليل محتوى PDF بحجم {ContentLength} حرف", pdfContent.Length);

                // إعداد طلب تحليل المستند
                var analyzePrompt = $"قم بتحليل وتلخيص المستند التالي، مع استخراج النقاط القانونية الرئيسية والمصطلحات المهمة:\n\n{pdfContent}";

                // تحضير الطلب
                var messages = new List<ChatMessageDTO>
                {
                    new ChatMessageDTO { Role = "system", Content = "أنت محلل قانوني متخصص في تلخيص وتحليل المستندات القانونية. قم بتقديم تحليل منظم وشامل." },
                    new ChatMessageDTO { Role = "user", Content = analyzePrompt }
                };

                // استخدام عدد توكنز أعلى للتحليلات الطويلة
                var response = await SendRequestToDeepSeekAsync(messages, 0.3, 4000);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحليل محتوى PDF");
                var errorMessage = _localizationService.GetMessage("PdfAnalysisError", "Errors", language);
                return $"{errorMessage}: {ex.Message}";
            }
        }

        /// <summary>
        /// الحصول على معلومات حول إجراء حكومي محدد
        /// </summary>
        public async Task<string> GetGovernmentProcedureAsync(string service, string language = "ar")
        {
            try
            {
                _logger.LogInformation("طلب معلومات حول إجراء حكومي: {Service}", service);

                // إعداد طلب الإجراء الحكومي
                var procedurePrompt = $"اشرح بالتفصيل الإجراءات المطلوبة للحصول على خدمة \"{service}\" من الجهات الحكومية في سلطنة عُمان. يرجى توضيح المستندات المطلوبة، والرسوم، والوقت المتوقع، وأي نصائح مهمة.";

                // تحضير الطلب
                var messages = new List<ChatMessageDTO>
                {
                    new ChatMessageDTO { Role = "system", Content = "أنت خبير في الإجراءات الحكومية والخدمات في سلطنة عُمان، مهمتك تقديم معلومات دقيقة وشاملة عن كيفية الحصول على الخدمات الحكومية." },
                    new ChatMessageDTO { Role = "user", Content = procedurePrompt }
                };

                var response = await SendRequestToDeepSeekAsync(messages);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على معلومات الإجراءات الحكومية");
                var errorMessage = _localizationService.GetMessage("GovernmentProcedureError", "Errors", language);
                return $"{errorMessage}: {ex.Message}";
            }
        }

        /// <summary>
        /// إرسال طلب إلى DeepSeek والحصول على الرد
        /// </summary>
        private async Task<string> SendRequestToDeepSeekAsync(List<ChatMessageDTO> messages, double temperature = 0.7, int maxTokens = 2000)
        {
            // إعداد الطلب
            var requestBody = new DeepSeekRequestDTO
            {
                Model = _model,
                Messages = messages,
                Temperature = temperature,
                MaxTokens = maxTokens
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // إضافة رأس المصادقة لكل طلب
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // محاولات إعادة المحاولة مع تأخير تصاعدي
            int retryCount = 0;
            while (true)
            {
                try
                {
                    var response = await _httpClient.PostAsync(_endpoint, requestContent);

                    // التحقق من نجاح الاستجابة
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var deepSeekResponse = JsonSerializer.Deserialize<DeepSeekResponseDTO>(responseContent);

                        if (deepSeekResponse?.Choices?.Count > 0)
                        {
                            _logger.LogInformation("تم استلام رد ناجح من DeepSeek: {TokensUsed} توكنز", deepSeekResponse.Usage.TotalTokens);
                            return deepSeekResponse.Choices[0].Message.Content;
                        }
                        else
                        {
                            _logger.LogWarning("استجابة DeepSeek ناقصة: {ResponseContent}", responseContent);
                            return "عذراً، لم أتمكن من معالجة طلبك في الوقت الحالي. يرجى المحاولة مرة أخرى.";
                        }
                    }
                    else
                    {
                        // تسجيل تفاصيل الخطأ
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("استجابة خطأ من DeepSeek: {StatusCode}, {ErrorContent}", response.StatusCode, errorContent);

                        // التحقق من إمكانية إعادة المحاولة
                        if (retryCount < _maxRetries && (
                            (int)response.StatusCode == 429 || // Rate limit
                            (int)response.StatusCode == 500 || // خطأ في الخادم
                            (int)response.StatusCode == 503)) // الخدمة غير متوفرة
                        {
                            retryCount++;
                            // زيادة تأخير إعادة المحاولة بشكل أسي (1s, 2s, 4s, ...)
                            var delayMs = (int)Math.Pow(2, retryCount) * 1000;
                            _logger.LogWarning("إعادة المحاولة {RetryCount}/{MaxRetries} بعد {DelayMs}ms", retryCount, _maxRetries, delayMs);
                            await Task.Delay(delayMs);
                            continue;
                        }

                        return $"عذراً، واجهت مشكلة في الاتصال بالخدمة (رمز الخطأ: {(int)response.StatusCode}).";
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "خطأ في طلب HTTP");

                    if (retryCount < _maxRetries)
                    {
                        retryCount++;
                        var delayMs = (int)Math.Pow(2, retryCount) * 1000;
                        _logger.LogWarning("إعادة المحاولة {RetryCount}/{MaxRetries} بعد {DelayMs}ms", retryCount, _maxRetries, delayMs);
                        await Task.Delay(delayMs);
                        continue;
                    }

                    return "عذراً، حدث خطأ في الاتصال بالخدمة. يرجى التحقق من اتصالك بالإنترنت والمحاولة مرة أخرى.";
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    _logger.LogError(ex, "انتهاء مهلة الطلب");
                    return "عذراً، استغرقت العملية وقتاً أطول من المتوقع. يرجى تبسيط استعلامك والمحاولة مرة أخرى.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطأ غير متوقع أثناء التواصل مع DeepSeek");
                    return "عذراً، حدث خطأ غير متوقع. يرجى المحاولة مرة أخرى لاحقاً.";
                }
            }
        }

        /// <summary>
        /// تحسين استعلام قانوني بإضافة سياق خاص بسلطنة عمان
        /// </summary>
        private string EnhanceLegalPrompt(string prompt, string language)
        {
            // إضافة تعليمات لتحسين الاستجابة القانونية
            if (language == "ar")
            {
                return $"قم بالإجابة على السؤال التالي المتعلق بالقوانين والإجراءات في سلطنة عُمان، مع الاعتماد فقط على المعلومات الواردة في الملفات المعطاة: \n\n{prompt}";
            }
            else
            {
                return $"Please answer the following question related to laws and procedures in the Sultanate of Oman, relying only on the information provided in the given files: \n\n{prompt}";
            }
        }

        /// <summary>
        /// الحصول على توجيهات النظام القانوني حسب اللغة
        /// </summary>
        private string GetLegalSystemPrompt(string language)
        {
            if (language == "ar")
            {
                return @"أنت مساعد قانوني متخصص في القوانين والإجراءات الحكومية في سلطنة عُمان. التزم بالقواعد التالية:

1. الإجابة فقط على الأسئلة المتعلقة بالقوانين والإجراءات القانونية والحكومية في سلطنة عُمان.
2. الاعتماد فقط على المعلومات المقدمة في الملفات المعطاة، وعدم الافتراض أو التخمين بمعلومات إضافية.
3. الإشارة بوضوح إلى مصدر المعلومات من الملفات المقدمة.
4. استخدام لغة قانونية دقيقة مع شرح المصطلحات الصعبة.
5. الالتزام بالأدب والاحترام في الردود.
6. عند عدم توفر معلومات كافية، اذكر ذلك بوضوح واقترح مصادر رسمية يمكن الرجوع إليها.
7. الإجابة بتنسيق منظم وواضح يسهل على المستخدم فهم الخطوات والإجراءات.

تذكر أن أي معلومات تقدمها يجب أن تكون معتمدة فقط على محتوى الملفات المرفقة، وليست نصيحة قانونية بديلة عن استشارة محامٍ مؤهل.";
            }
            else
            {
                return @"You are a legal assistant specialized in laws and government procedures in the Sultanate of Oman. Adhere to the following rules:

1. Answer only questions related to laws and legal and governmental procedures in the Sultanate of Oman.
2. Rely only on information provided in the given files, without assuming or guessing additional information.
3. Clearly reference the source of information from the provided files.
4. Use precise legal language while explaining difficult terms.
5. Maintain politeness and respect in responses.
6. When sufficient information is not available, clearly state this and suggest official sources for further reference.
7. Answer in an organized and clear format that makes it easy for the user to understand steps and procedures.

Remember that any information you provide should be based solely on the content of the attached files and is not a substitute for consulting a qualified lawyer.";
            }
        }
    }
}