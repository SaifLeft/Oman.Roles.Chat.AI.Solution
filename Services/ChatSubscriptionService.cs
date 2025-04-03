using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.Subscription;
using Models.DTOs.Subscription.Enums;
using Models.DTOs.Subscription.Requests;

namespace Services
{
    /// <summary>
    /// خدمة التكامل بين نظام الدردشة ونظام الاشتراك
    /// </summary>
    public class ChatSubscriptionService
    {
        private readonly IChatDbService _chatService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<ChatSubscriptionService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;

        public ChatSubscriptionService(
            IChatDbService chatService,
            ISubscriptionService subscriptionService,
            ILogger<ChatSubscriptionService> logger,
            ILocalizationService localizationService,
            IConfiguration configuration)
        {
            _chatService = chatService;
            _subscriptionService = subscriptionService;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;
        }

        /// <summary>
        /// التحقق من إمكانية إنشاء غرفة دردشة جديدة بناءً على اشتراك المستخدم
        /// </summary>
        public async Task<BaseResponse<bool>> CanCreateChatRoomAsync(string userId, string language)
        {
            try
            {
                // الحصول على اشتراك المستخدم
                var subscriptionResponse = await _subscriptionService.GetUserSubscriptionAsync(userId, language);

                if (!subscriptionResponse.Success)
                {
                    // إذا لم يكن هناك اشتراك نشط، فلا يمكن إنشاء غرفة دردشة
                    var errorMessage = _localizationService.GetMessage("ActiveSubscriptionRequired", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 403);
                }

                var subscription = subscriptionResponse.Data;

                // التحقق من أن الاشتراك نشط
                if (subscription.Status != SubscriptionStatus.Active && subscription.Status != SubscriptionStatus.Trial)
                {
                    var errorMessage = _localizationService.GetMessage("ActiveSubscriptionRequired", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 403);
                }

                // الحصول على خطة الاشتراك
                var planResponse = await _subscriptionService.GetPlanByIdAsync(subscription.PlanId, language);

                if (!planResponse.Success)
                {
                    var errorMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 404);
                }

                var plan = planResponse.Data;

                // الحصول على عدد غرف الدردشة الحالية للمستخدم
                var userRoomsResponse = await _chatService.GetChatRoomsByUserIdAsync(long.Parse(userId));
                int currentRoomCount = userRoomsResponse.Count;

                // التحقق مما إذا كان المستخدم قد وصل إلى الحد الأقصى من غرف الدردشة
                if (currentRoomCount >= plan.AllowedChatRooms)
                {
                    var errorMessage = _localizationService.GetMessage("ChatRoomLimitReached", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 403, new List<string> {
                        string.Format(_localizationService.GetMessage("ChatRoomLimitInfo", "Errors", language),
                        currentRoomCount, plan.AllowedChatRooms)
                    });
                }

                return BaseResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء التحقق من إمكانية إنشاء غرفة دردشة للمستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionCheckError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// التحقق من إمكانية إضافة ملفات PDF بناءً على اشتراك المستخدم
        /// </summary>
        public async Task<BaseResponse<bool>> CanAddPdfFilesAsync(string userId, int fileCount, long totalSizeBytes, string language)
        {
            try
            {
                // الحصول على اشتراك المستخدم
                var subscriptionResponse = await _subscriptionService.GetUserSubscriptionAsync(userId, language);

                if (!subscriptionResponse.Success)
                {
                    // إذا لم يكن هناك اشتراك نشط، فلا يمكن إضافة ملفات
                    var errorMessage = _localizationService.GetMessage("ActiveSubscriptionRequired", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 403);
                }

                var subscription = subscriptionResponse.Data;

                // التحقق من أن الاشتراك نشط
                if (subscription.Status != SubscriptionStatus.Active && subscription.Status != SubscriptionStatus.Trial)
                {
                    var errorMessage = _localizationService.GetMessage("ActiveSubscriptionRequired", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 403);
                }

                // الحصول على خطة الاشتراك
                var planResponse = await _subscriptionService.GetPlanByIdAsync(subscription.PlanId, language);

                if (!planResponse.Success)
                {
                    var errorMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 404);
                }

                var plan = planResponse.Data;

                // التحقق من عدد الملفات
                if (fileCount > plan.AllowedFiles)
                {
                    var errorMessage = _localizationService.GetMessage("PdfFilesLimitReached", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 403, new List<string> {
                        string.Format(_localizationService.GetMessage("PdfFilesLimitInfo", "Errors", language),
                        fileCount, plan.AllowedFiles)
                    });
                }

                // التحقق من حجم الملفات (تحويل من بايت إلى ميجابايت)
                long totalSizeMb = totalSizeBytes / (1024 * 1024);
                if (totalSizeMb > plan.AllowedFileSizeMb)
                {
                    var errorMessage = _localizationService.GetMessage("PdfFileSizeLimitReached", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 403, new List<string> {
                        string.Format(_localizationService.GetMessage("PdfFileSizeLimitInfo", "Errors", language),
                        totalSizeMb, plan.AllowedFileSizeMb)
                    });
                }

                return BaseResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء التحقق من إمكانية إضافة ملفات PDF للمستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionCheckError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إنشاء اشتراك تجريبي للمستخدم تلقائيًا
        /// </summary>
        public async Task<BaseResponse<UserSubscriptionDTO>> CreateTrialSubscriptionAsync(string userId, string language)
        {
            try
            {
                // التحقق من تمكين الحسابات التجريبية في الإعدادات
                bool enableTrialAccounts = _configuration["SubscriptionSettings:EnableTrialAccounts"] == "true";
                if (!enableTrialAccounts)
                {
                    var errorMessage = _localizationService.GetMessage("TrialAccountsDisabled", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 403);
                }

                // البحث عن خطة التجربة المجانية
                var plans = (await _subscriptionService.GetAllPlansAsync(language)).Data;
                var trialPlan = plans.FirstOrDefault(p => p.IsTrial && p.IsActive);

                if (trialPlan == null)
                {
                    var errorMessage = _localizationService.GetMessage("TrialPlanNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 404);
                }

                // إنشاء طلب اشتراك تجريبي
                var request = new CreateSubscriptionRequestDTO
                {
                    PlanId = trialPlan.Id,
                    PeriodType = SubscriptionPeriodType.Monthly,
                    AutoRenew = false // عدم تجديد الاشتراك التجريبي تلقائيًا
                };

                // إنشاء الاشتراك التجريبي
                return await _subscriptionService.CreateSubscription(userId, request, language);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إنشاء اشتراك تجريبي للمستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("TrialSubscriptionCreationError", "Errors", language);
                return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 500);
            }
        }
    }
}