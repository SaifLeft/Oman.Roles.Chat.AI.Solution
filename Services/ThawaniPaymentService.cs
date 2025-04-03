using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.Subscription.Enums;
using Models.DTOs.Subscription.Requests;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services
{
    /// <summary>
    /// واجهة خدمة الدفع باستخدام ثواني
    /// </summary>
    public interface IThawaniPaymentService
    {
        /// <summary>
        /// إنشاء جلسة دفع جديدة
        /// </summary>
        /// <param name="amount">المبلغ بالريال العماني</param>
        /// <param name="customerId">معرف العميل</param>
        /// <param name="customerEmail">البريد الإلكتروني للعميل</param>
        /// <param name="customerName">اسم العميل</param>
        /// <param name="subscriptionPlanId">معرف خطة الاشتراك</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>معلومات جلسة الدفع</returns>
        Task<BaseResponse<ThawaniSessionResponse>> CreatePaymentSessionAsync(
            decimal amount,
            string customerId,
            string customerEmail,
            string customerName,
            string subscriptionPlanId,
            string language);

        /// <summary>
        /// التحقق من حالة الدفع
        /// </summary>
        /// <param name="sessionId">معرف جلسة الدفع</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>حالة الدفع</returns>
        Task<BaseResponse<ThawaniPaymentStatusResponse>> CheckPaymentStatusAsync(string sessionId, string language);

        /// <summary>
        /// معالجة إشعار الدفع
        /// </summary>
        /// <param name="payload">بيانات الإشعار</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>نتيجة المعالجة</returns>
        Task<BaseResponse<bool>> HandlePaymentWebhookAsync(string payload, string language);
    }

    /// <summary>
    /// تنفيذ خدمة الدفع باستخدام ثواني
    /// </summary>
    public class ThawaniPaymentService : IThawaniPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ThawaniPaymentService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly ISubscriptionService _subscriptionService;

        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly string _baseUrl;
        private readonly string _successUrl;
        private readonly string _cancelUrl;
        private readonly string _webhookUrl;

        public ThawaniPaymentService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<ThawaniPaymentService> logger,
            ILocalizationService localizationService,
            ISubscriptionService subscriptionService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;
            _subscriptionService = subscriptionService;

            // استخراج بيانات التكوين
            _apiKey = _configuration["Thawani:ApiKey"] ?? throw new InvalidOperationException("Thawani API Key is missing in configuration");
            _secretKey = _configuration["Thawani:SecretKey"] ?? throw new InvalidOperationException("Thawani Secret Key is missing in configuration");
            _baseUrl = _configuration["Thawani:BaseUrl"] ?? "https://uatcheckout.thawani.om/api/v1/";
            _successUrl = _configuration["Thawani:SuccessUrl"] ?? "https://yourdomain.com/payment/success";
            _cancelUrl = _configuration["Thawani:CancelUrl"] ?? "https://yourdomain.com/payment/cancel";
            _webhookUrl = _configuration["Thawani:WebhookUrl"] ?? "https://yourdomain.com/api/payment/webhook";

            // تهيئة عميل HTTP
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("thawani-api-key", _apiKey);
        }

        /// <summary>
        /// إنشاء جلسة دفع جديدة
        /// </summary>
        public async Task<BaseResponse<ThawaniSessionResponse>> CreatePaymentSessionAsync(
            decimal amount,
            string customerId,
            string customerEmail,
            string customerName,
            string subscriptionPlanId,
            string language)
        {
            try
            {
                _logger.LogInformation("Creating Thawani payment session for customer: {CustomerId}, plan: {PlanId}", customerId, subscriptionPlanId);

                // تحويل المبلغ إلى بيسة (1 ريال = 1000 بيسة)
                var amountInBaisa = (int)(amount * 1000);

                // إنشاء طلب إنشاء جلسة دفع
                var sessionRequest = new ThawaniSessionRequest
                {
                    ClientReferenceId = $"sub_{customerId}_{subscriptionPlanId}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Products = new List<ThawaniProduct>
                    {
                        new ThawaniProduct
                        {
                            Name = "Subscription Plan",
                            Quantity = 1,
                            UnitAmount = amountInBaisa
                        }
                    },
                    CustomerDetails = new ThawaniCustomerDetails
                    {
                        Email = customerEmail,
                        Name = customerName
                    },
                    SuccessUrl = _successUrl,
                    CancelUrl = _cancelUrl
                };

                // إرسال الطلب إلى ثواني
                var content = new StringContent(JsonSerializer.Serialize(sessionRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}checkout/session", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Thawani payment session creation failed: {ErrorContent}", errorContent);
                    var errorMessage = _localizationService.GetMessage("PaymentSessionCreationFailed", "Errors", language);
                    return BaseResponse<ThawaniSessionResponse>.FailureResponse(errorMessage, (int)response.StatusCode);
                }

                // قراءة الاستجابة
                var responseContent = await response.Content.ReadAsStringAsync();
                var sessionResponse = JsonSerializer.Deserialize<ThawaniSessionResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (sessionResponse == null)
                {
                    var errorMessage = _localizationService.GetMessage("PaymentSessionResponseInvalid", "Errors", language);
                    return BaseResponse<ThawaniSessionResponse>.FailureResponse(errorMessage, 500);
                }

                _logger.LogInformation("Thawani payment session created successfully: {SessionId}", sessionResponse.SessionId);
                var successMessage = _localizationService.GetMessage("PaymentSessionCreated", "Messages", language);
                return BaseResponse<ThawaniSessionResponse>.SuccessResponse(sessionResponse, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Thawani payment session");
                var errorMessage = _localizationService.GetMessage("PaymentSessionCreationError", "Errors", language);
                return BaseResponse<ThawaniSessionResponse>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// التحقق من حالة الدفع
        /// </summary>
        public async Task<BaseResponse<ThawaniPaymentStatusResponse>> CheckPaymentStatusAsync(string sessionId, string language)
        {
            try
            {
                _logger.LogInformation("Checking Thawani payment status for session: {SessionId}", sessionId);

                // إرسال طلب التحقق من حالة الدفع
                var response = await _httpClient.GetAsync($"{_baseUrl}checkout/session/{sessionId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Thawani payment status check failed: {ErrorContent}", errorContent);
                    var errorMessage = _localizationService.GetMessage("PaymentStatusCheckFailed", "Errors", language);
                    return BaseResponse<ThawaniPaymentStatusResponse>.FailureResponse(errorMessage, (int)response.StatusCode);
                }

                // قراءة الاستجابة
                var responseContent = await response.Content.ReadAsStringAsync();
                var statusResponse = JsonSerializer.Deserialize<ThawaniPaymentStatusResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (statusResponse == null)
                {
                    var errorMessage = _localizationService.GetMessage("PaymentStatusResponseInvalid", "Errors", language);
                    return BaseResponse<ThawaniPaymentStatusResponse>.FailureResponse(errorMessage, 500);
                }

                _logger.LogInformation("Thawani payment status checked successfully: {SessionId}, Status: {Status}", sessionId, statusResponse.Status);
                var successMessage = _localizationService.GetMessage("PaymentStatusChecked", "Messages", language);
                return BaseResponse<ThawaniPaymentStatusResponse>.SuccessResponse(statusResponse, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Thawani payment status");
                var errorMessage = _localizationService.GetMessage("PaymentStatusCheckError", "Errors", language);
                return BaseResponse<ThawaniPaymentStatusResponse>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// معالجة إشعار الدفع
        /// </summary>
        public async Task<BaseResponse<bool>> HandlePaymentWebhookAsync(string payload, string language)
        {
            try
            {
                _logger.LogInformation("Handling Thawani payment webhook");

                // تحليل بيانات الإشعار
                var webhookData = JsonSerializer.Deserialize<ThawaniWebhookPayload>(payload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (webhookData == null)
                {
                    var errorMessage = _localizationService.GetMessage("PaymentWebhookInvalid", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 400);
                }

                // التحقق من صحة الإشعار
                if (webhookData.Event != "checkout.session.completed")
                {
                    _logger.LogInformation("Ignoring non-completion webhook event: {Event}", webhookData.Event);
                    return BaseResponse<bool>.SuccessResponse(true, "Webhook processed");
                }

                // استخراج معرف العميل ومعرف خطة الاشتراك من معرف المرجع
                var referenceId = webhookData.Data.ClientReferenceId;
                var parts = referenceId.Split('_');

                if (parts.Length < 3 || !parts[0].Equals("sub", StringComparison.OrdinalIgnoreCase))
                {
                    var errorMessage = _localizationService.GetMessage("PaymentReferenceInvalid", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 400);
                }

                var userId = parts[1];
                var planId = parts[2];

                // إنشاء طلب اشتراك جديد
                var subscriptionRequest = new CreateSubscriptionRequestDTO
                {
                    PlanId = planId,
                    PeriodType = SubscriptionPeriodType.Monthly, // يمكن تعديله حسب الحاجة
                    PaymentGatewayTransactionId = webhookData.Data.SessionId,
                    PaymentMethod = "Thawani"
                };

                // إنشاء الاشتراك
                var result = await _subscriptionService.CreateSubscription(userId, subscriptionRequest, language);

                if (!result.Success)
                {
                    _logger.LogError("Failed to create subscription after successful payment: {Error}", result.Message);
                    return BaseResponse<bool>.FailureResponse(result.Message, result.StatusCode);
                }

                _logger.LogInformation("Subscription created successfully after payment: {SubscriptionId}", result.Data.Id);
                var successMessage = _localizationService.GetMessage("PaymentWebhookProcessed", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling Thawani payment webhook");
                var errorMessage = _localizationService.GetMessage("PaymentWebhookError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }
    }

    #region Thawani Models

    /// <summary>
    /// نموذج طلب إنشاء جلسة دفع ثواني
    /// </summary>
    public class ThawaniSessionRequest
    {
        [JsonPropertyName("client_reference_id")]
        public string ClientReferenceId { get; set; } = string.Empty;

        [JsonPropertyName("products")]
        public List<ThawaniProduct> Products { get; set; } = new List<ThawaniProduct>();

        [JsonPropertyName("customer_details")]
        public ThawaniCustomerDetails CustomerDetails { get; set; } = new ThawaniCustomerDetails();

        [JsonPropertyName("success_url")]
        public string SuccessUrl { get; set; } = string.Empty;

        [JsonPropertyName("cancel_url")]
        public string CancelUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج منتج ثواني
    /// </summary>
    public class ThawaniProduct
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("unit_amount")]
        public int UnitAmount { get; set; }
    }

    /// <summary>
    /// نموذج تفاصيل العميل ثواني
    /// </summary>
    public class ThawaniCustomerDetails
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج استجابة جلسة دفع ثواني
    /// </summary>
    public class ThawaniSessionResponse
    {
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = string.Empty;

        [JsonPropertyName("payment_url")]
        public string PaymentUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج استجابة حالة دفع ثواني
    /// </summary>
    public class ThawaniPaymentStatusResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("client_reference_id")]
        public string ClientReferenceId { get; set; } = string.Empty;

        [JsonPropertyName("customer")]
        public string Customer { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج بيانات إشعار ثواني
    /// </summary>
    public class ThawaniWebhookPayload
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public ThawaniWebhookData Data { get; set; } = new ThawaniWebhookData();
    }

    /// <summary>
    /// نموذج بيانات إشعار ثواني
    /// </summary>
    public class ThawaniWebhookData
    {
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = string.Empty;

        [JsonPropertyName("client_reference_id")]
        public string ClientReferenceId { get; set; } = string.Empty;
    }

    #endregion
}