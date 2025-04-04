using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.DTOs.AIChat;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Services
{
    /// <summary>
    /// واجهة خدمة DeepSeek للذكاء الاصطناعي
    /// </summary>
    public interface IDeepSeekService
    {
        /// <summary>
        /// الحصول على رد من نموذج DeepSeek
        /// </summary>
        /// <param name="query">استعلام المستخدم</param>
        /// <param name="language">اللغة (العربية الافتراضية)</param>
        /// <returns>رد النموذج</returns>
        Task<DeepSeekResponseDTO> GetResponseAsync(string query, string language = "ar");

        /// <summary>
        /// تنفيذ استعلام قانوني مع سياق محدد
        /// </summary>
        /// <param name="query">استعلام المستخدم</param>
        /// <param name="context">السياق القانوني المعطى</param>
        /// <param name="language">اللغة المطلوبة</param>
        /// <returns>استجابة قانونية</returns>
        Task<string> ExecuteLegalQueryAsync(string query, string context, string language = "ar");
        Task<Models.Common.BaseResponse<string>> ProcessPdfDataAsync(string enrichedContext, string language);
    }

    /// <summary>
    /// تنفيذ خدمة DeepSeek للذكاء الاصطناعي
    /// </summary>
    public class DeepSeekService : IDeepSeekService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeepSeekService> _logger;

        /// <summary>
        /// إنشاء نسخة جديدة من خدمة DeepSeek
        /// </summary>
        public DeepSeekService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<DeepSeekService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // تكوين عنوان API
            string apiUrl = _configuration["DeepSeekSettings:ApiUrl"];
            if (!string.IsNullOrEmpty(apiUrl))
            {
                _httpClient.BaseAddress = new Uri(apiUrl);
            }

            // إضافة مفتاح API إلى الرأس إذا كان متاحًا
            string apiKey = _configuration["DeepSeekSettings:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }
        }

        /// <summary>
        /// الحصول على رد من نموذج DeepSeek
        /// </summary>
        public async Task<DeepSeekResponseDTO> GetResponseAsync(string query, string language = "ar")
        {
            try
            {
                _logger.LogInformation($"Processing query with DeepSeek: {query}");

                // بناء طلب DeepSeek
                var request = new DeepSeekRequestDTO
                {
                    Prompt = query,
                    SystemPrompt = GetSystemPrompt(language)
                };

                // إرسال الطلب
                var response = await SendRequestAsync(request);
                if (response != null)
                {
                    _logger.LogInformation("DeepSeek response received successfully");
                    return response;
                }

                _logger.LogWarning("DeepSeek returned null response");
                return new DeepSeekResponseDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = GetErrorMessage(language),
                    Finished = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting response from DeepSeek API");
                return new DeepSeekResponseDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = GetErrorMessage(language),
                    Finished = false
                };
            }
        }

        /// <summary>
        /// إرسال طلب إلى API نموذج DeepSeek
        /// </summary>
        private async Task<DeepSeekResponseDTO> SendRequestAsync(DeepSeekRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Sending request to DeepSeek API");

                // تحويل الطلب إلى JSON
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // إرسال الطلب إلى API
                var endpoint = _configuration["DeepSeekSettings:Endpoint"] ?? "api/generate";
                var response = await _httpClient.PostAsync(endpoint, content);

                // التحقق من الاستجابة
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("DeepSeek API request successful");
                    return await response.Content.ReadFromJsonAsync<DeepSeekResponseDTO>();
                }

                _logger.LogWarning($"DeepSeek API returned status code: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Error content: {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending request to DeepSeek API");
                throw;
            }
        }

        /// <summary>
        /// الحصول على رسالة الخطأ المناسبة
        /// </summary>
        private string GetErrorMessage(string language)
        {
            return language.ToLower() == "ar"
                ? "عذراً، حدث خطأ أثناء معالجة طلبك. يرجى المحاولة مرة أخرى لاحقاً."
                : "Sorry, an error occurred while processing your request. Please try again later.";
        }

        /// <summary>
        /// الحصول على توجيه النظام المناسب
        /// </summary>
        private string GetSystemPrompt(string language)
        {
            return language.ToLower() == "ar"
                ? "أنت مساعد قانوني ذكي متخصص في القانون العماني. مهمتك هي تقديم إجابات دقيقة ومفيدة للاستفسارات القانونية باللغة العربية. استخدم المعلومات القانونية الدقيقة في إجاباتك، واستشهد بالقوانين واللوائح العمانية ذات الصلة عندما يكون ذلك مناسباً. تحلى بالوضوح والدقة في إجاباتك، وكن محترفاً ومفيداً دائماً."
                : "You are an intelligent legal assistant specialized in Omani law. Your task is to provide accurate and helpful answers to legal inquiries in English. Use accurate legal information in your answers, and cite relevant Omani laws and regulations when appropriate. Be clear and precise in your answers, and always be professional and helpful.";
        }

        /// <summary>
        /// تنفيذ استعلام قانوني مع سياق محدد
        /// </summary>
        /// <param name="query">استعلام المستخدم</param>
        /// <param name="context">السياق القانوني المعطى</param>
        /// <param name="language">اللغة المطلوبة</param>
        /// <returns>استجابة قانونية</returns>
        public async Task<string> ExecuteLegalQueryAsync(string query, string context, string language = "ar")
        {
            try
            {
                _logger.LogInformation($"Executing legal query: {query}");

                // Prepare legal context and query
                var legalContext = string.IsNullOrEmpty(context)
                    ? query
                    : $"Context: {context}\n\nQuery: {query}";

                // Create request with system prompt and user query
                var request = new DeepSeekRequestDTO
                {
                    Prompt = legalContext,
                    SystemPrompt = GetLegalSystemPrompt(language)
                };

                // Send request to DeepSeek
                var response = await SendRequestAsync(request);
                if (response != null)
                {
                    _logger.LogInformation("Legal query response received successfully");
                    return response.Content;
                }

                _logger.LogWarning("DeepSeek returned null response for legal query");
                return GetErrorMessage(language);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing legal query with DeepSeek API");
                return GetErrorMessage(language);
            }
        }

        /// <summary>
        /// الحصول على مطالبة النظام للاستعلامات القانونية
        /// </summary>
        private string GetLegalSystemPrompt(string language)
        {
            return language == "ar"
                ? "أنت مساعد قانوني ذكي متخصص في القوانين العمانية. مهمتك هي تقديم إجابات دقيقة ومفيدة للاستفسارات القانونية باللغة العربية. استخدم معلومات قانونية دقيقة في إجاباتك، واستشهد بالقوانين واللوائح العمانية ذات الصلة عند الاقتضاء. كن واضحًا ودقيقًا في إجاباتك، وكن دائمًا محترفًا ومفيدًا."
                : "You are an intelligent legal assistant specialized in Omani law. Your task is to provide accurate and helpful answers to legal inquiries in English. Use accurate legal information in your answers, and cite relevant Omani laws and regulations when appropriate. Be clear and precise in your answers, and always be professional and helpful.";
        }

        /// <summary>
        /// معالجة بيانات ملفات PDF مع ضمان التقيد بالمعلومات من الملفات فقط
        /// </summary>
        /// <param name="enrichedContext">محتوى مثرى من الملفات</param>
        /// <param name="language">اللغة المطلوبة</param>
        /// <returns>استجابة لمعالجة البيانات من الذكاء الاصطناعي</returns>
        public async Task<Models.Common.BaseResponse<string>> ProcessPdfDataAsync(string enrichedContext, string language)
        {
            try
            {
                _logger.LogInformation("Processing PDF data with DeepSeek");

                // بناء طلب للنموذج مع توجيهات خاصة للتقيد بمحتوى الملفات فقط
                var request = new DeepSeekRequestDTO
                {
                    Prompt = enrichedContext,
                    SystemPrompt = GetPdfContextSystemPrompt(language)
                };

                // إرسال الطلب إلى النموذج
                var response = await SendRequestAsync(request);
                if (response != null)
                {
                    _logger.LogInformation("PDF data processed successfully");
                    return new Models.Common.BaseResponse<string>
                    {
                        Success = true,
                        Data = response.Content
                    };
                }

                _logger.LogWarning("DeepSeek returned null response for PDF data processing");
                return new Models.Common.BaseResponse<string>
                {
                    Success = false,
                    Message = GetErrorMessage(language)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PDF data with DeepSeek API");
                return new Models.Common.BaseResponse<string>
                {
                    Success = false,
                    Message = GetErrorMessage(language)
                };
            }
        }

        /// <summary>
        /// الحصول على مطالبة النظام للتقيد بمحتوى الملفات
        /// </summary>
        private string GetPdfContextSystemPrompt(string language)
        {
            return language == "ar"
                ? "أنت مساعد قانوني مدرب على التشريعات والقوانين العمانية. يجب عليك الإجابة فقط باستخدام المعلومات الموجودة في الوثائق المقدمة. إذا كان السؤال يتعلق بموضوع غير موجود في المستندات، فيجب أن توضح أنك لا تستطيع الإجابة عن هذا السؤال لأنه خارج نطاق المستندات المتاحة. استشهد بأرقام الصفحات أو أقسام المستندات عند الإجابة، وتأكد من الدقة. لا تختلق معلومات ليست موجودة في المستندات."
                : "You are a legal assistant trained on Omani legislation and laws. You must answer only using information contained in the provided documents. If the question relates to a topic not covered in the documents, you should clarify that you cannot answer this question because it is outside the scope of the available documents. Cite page numbers or document sections when answering, and ensure accuracy. Do not make up information that isn't present in the documents.";
        }
    }
}