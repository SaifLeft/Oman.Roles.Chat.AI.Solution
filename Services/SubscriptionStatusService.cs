using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.Subscription;
using Services.Security;

namespace Services
{
    /// <summary>
    /// واجهة خدمات حالة الاشتراك
    /// </summary>
    public interface ISubscriptionStatusService
    {
        /// <summary>
        /// الحصول على حالة اشتراك المستخدم
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المفضلة</param>
        /// <returns>حالة الاشتراك</returns>
        Task<BaseResponse<SubscriptionStatusDTO>> GetUserSubscriptionStatusAsync(long userId, string language);

        /// <summary>
        /// تحديث عدد الاستعلامات المستخدمة للمستخدم
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المفضلة</param>
        /// <returns>نتيجة التحديث</returns>
        Task<BaseResponse<bool>> IncrementQueryCountAsync(long userId, string language);

        /// <summary>
        /// التحقق من صلاحية اشتراك المستخدم
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المفضلة</param>
        /// <returns>نتيجة التحقق</returns>
        Task<BaseResponse<bool>> IsSubscriptionValidAsync(long userId, string language);

        /// <summary>
        /// إعادة تعيين عداد الاستعلامات الشهرية
        /// </summary>
        /// <param name="userId">معرف المستخدم (اختياري للإعادة لمستخدم محدد)</param>
        /// <param name="language">اللغة المفضلة</param>
        /// <returns>نتيجة إعادة التعيين</returns>
        Task<BaseResponse<bool>> ResetMonthlyQueryCountersAsync(long? userId, string language);
    }

    /// <summary>
    /// تنفيذ خدمة التحقق من حالة الاشتراك
    /// </summary>
    public class SubscriptionStatusService : ISubscriptionStatusService
    {
        private readonly MuhamiContext _context;
        private readonly ILogger<SubscriptionStatusService> _logger;
        private readonly IEncryptionService _encryptionService;
        private readonly ILocalizationService _localizationService;

        public SubscriptionStatusService(
            MuhamiContext context,
            ILogger<SubscriptionStatusService> logger,
            IEncryptionService encryptionService,
            ILocalizationService localizationService)
        {
            _context = context;
            _logger = logger;
            _encryptionService = encryptionService;
            _localizationService = localizationService;
        }

        /// <summary>
        /// التحقق من صلاحية اشتراك المستخدم
        /// </summary>
        public async Task<bool> IsSubscriptionValidAsync(long userId)
        {
            try
            {
                // البحث عن اشتراك نشط للمستخدم
                var activeSubscription = await _context.UserSubscriptions
                    .Where(s => s.UserId == userId && s.Status == "Active" && !s.IsDeleted)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                if (activeSubscription == null)
                {
                    // لا يوجد اشتراك نشط، تحقق من الخطة المجانية
                    var freeQueryUsage = await GetFreeQueryUsageAsync(userId);
                    return freeQueryUsage < GetFreeQueryLimit();
                }

                // تحقق من انتهاء صلاحية الاشتراك
                return activeSubscription.EndDate > DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء التحقق من صلاحية اشتراك المستخدم {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// الحصول على حالة اشتراك المستخدم
        /// </summary>
        public async Task<BaseResponse<SubscriptionStatusDTO>> GetSubscriptionStatusAsync(long userId, string language)
        {
            try
            {
                // البحث عن المستخدم
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    var errorMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<SubscriptionStatusDTO>.FailureResponse(errorMessage, 404);
                }

                // البحث عن اشتراك نشط للمستخدم
                var activeSubscription = await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.UserId == userId && s.Status == "Active" && !s.IsDeleted)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                var status = new SubscriptionStatusDTO
                {
                    UserId = userId,
                    IsActive = activeSubscription != null && activeSubscription.EndDate > DateTime.UtcNow,
                    PlanName = activeSubscription?.Plan.Name ?? "Free"
                };

                if (activeSubscription != null)
                {
                    status.StartDate = activeSubscription.StartDate;
                    status.EndDate = activeSubscription.EndDate;
                    status.DaysRemaining = (int)Math.Max(0, (activeSubscription.EndDate.Value - DateTime.UtcNow).TotalDays);
                    status.Features = await GetSubscriptionFeaturesAsync(activeSubscription.PlanId);
                }
                else
                {
                    // الخطة المجانية
                    status.StartDate = DateTime.UtcNow;
                    status.EndDate = DateTime.UtcNow.AddYears(100); // الخطة المجانية لا تنتهي
                    status.DaysRemaining = 36500; // حوالي 100 سنة
                    status.Features = await GetFreePlanFeaturesAsync();
                }

                var successMessage = _localizationService.GetMessage("SubscriptionStatusRetrieved", "Messages", language);
                return BaseResponse<SubscriptionStatusDTO>.SuccessResponse(status, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على حالة اشتراك المستخدم {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionStatusError", "Errors", language);
                return BaseResponse<SubscriptionStatusDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على استخدام الاستعلامات للمستخدم
        /// </summary>
        public async Task<BaseResponse<QueryUsageDTO>> GetQueryUsageAsync(long userId, string language)
        {
            try
            {
                // البحث عن اشتراك نشط للمستخدم
                var activeSubscription = await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.UserId == userId && s.Status == "Active" && !s.IsDeleted)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                var queryUsage = new QueryUsageDTO
                {
                    UserId = userId
                };

                if (activeSubscription != null && activeSubscription.EndDate > DateTime.UtcNow)
                {
                    // اشتراك مدفوع
                    queryUsage.IsPremium = true;
                    queryUsage.QueryLimit = (int)activeSubscription.Plan.MaxQueriesPerDay;
                    queryUsage.QueryUsage = activeSubscription.QueriesUsedToday != null ? (int)activeSubscription.QueriesUsedToday : 0;
                    queryUsage.RemainingQueries = Math.Max(0, queryUsage.QueryLimit - queryUsage.QueryUsage);
                }
                else
                {
                    // الخطة المجانية
                    queryUsage.IsPremium = false;
                    queryUsage.QueryLimit = GetFreeQueryLimit();
                    queryUsage.QueryUsage = await GetFreeQueryUsageAsync(userId);
                    queryUsage.RemainingQueries = Math.Max(0, queryUsage.QueryLimit - queryUsage.QueryUsage);
                }

                var successMessage = _localizationService.GetMessage("QueryUsageRetrieved", "Messages", language);
                return BaseResponse<QueryUsageDTO>.SuccessResponse(queryUsage, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على استخدام الاستعلامات للمستخدم {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("QueryUsageError", "Errors", language);
                return BaseResponse<QueryUsageDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث استخدام الاستعلامات للمستخدم
        /// </summary>
        public async Task<bool> IncrementQueryUsageAsync(long userId)
        {
            try
            {
                // البحث عن اشتراك نشط للمستخدم
                var activeSubscription = await _context.UserSubscriptions
                    .Where(s => s.UserId == userId && s.Status == "Active" && !s.IsDeleted)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                if (activeSubscription != null && activeSubscription.EndDate > DateTime.UtcNow)
                {
                    // تحديث استخدام الاستعلامات للمشترك المدفوع
                    var today = DateTime.UtcNow.Date;
                    var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

                    // إعادة تعيين العداد اليومي إذا كان آخر استعلام في يوم سابق
                    if (activeSubscription.LastQueryDate == null || activeSubscription.LastQueryDate.Value.Date < today)
                    {
                        activeSubscription.QueriesUsedToday = 0;
                    }

                    // إعادة تعيين العداد الشهري إذا كان آخر استعلام في شهر سابق
                    if (activeSubscription.LastQueryDate == null || activeSubscription.LastQueryDate.Value.Date < firstDayOfMonth)
                    {
                        activeSubscription.QueriesUsedThisMonth = 0;
                    }

                    // زيادة العدادات
                    activeSubscription.QueriesUsedToday = (activeSubscription.QueriesUsedToday != null ? activeSubscription.QueriesUsedToday : 0) + 1;
                    activeSubscription.QueriesUsedThisMonth = (activeSubscription.QueriesUsedThisMonth != null ? activeSubscription.QueriesUsedThisMonth : 0) + 1;
                    activeSubscription.LastQueryDate = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    // تحديث استخدام الاستعلامات للمستخدم المجاني
                    var today = DateTime.UtcNow.Date;
                    var queryKey = $"free_query_usage:{userId}:{today:yyyy-MM-dd}";
                    var monthlyQueryKey = $"free_query_usage:{userId}:{today:yyyy-MM}";

                    // الحصول على الاستخدام الحالي
                    var dailyUsage = await GetFreeQueryUsageAsync(userId);
                    var monthlyUsage = await GetFreeMonthlyQueryUsageAsync(userId);

                    // تخزين الاستخدام المحدث
                    await StoreQueryUsageAsync(queryKey, dailyUsage + 1);
                    await StoreQueryUsageAsync(monthlyQueryKey, monthlyUsage + 1);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث استخدام الاستعلامات للمستخدم {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// التحقق من قدرة المستخدم على إجراء استعلام
        /// </summary>
        public async Task<bool> CanMakeQueryAsync(long userId)
        {
            try
            {
                // البحث عن اشتراك نشط للمستخدم
                var activeSubscription = await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.UserId == userId && s.Status == "Active" && !s.IsDeleted)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                if (activeSubscription != null && activeSubscription.EndDate > DateTime.UtcNow)
                {
                    // التحقق من حدود الاستخدام للمشترك المدفوع
                    var queryUsage = activeSubscription.QueriesUsedToday != null ? activeSubscription.QueriesUsedToday : 0;
                    var queryLimit = activeSubscription.Plan.MaxQueriesPerDay;

                    var monthlyUsage = activeSubscription.QueriesUsedThisMonth != null ? activeSubscription.QueriesUsedThisMonth : 0;
                    var monthlyLimit = activeSubscription.Plan.MaxQueriesPerMonth;

                    return queryUsage < queryLimit && monthlyUsage < monthlyLimit;
                }
                else
                {
                    // التحقق من حدود الاستخدام للمستخدم المجاني
                    var freeUsage = await GetFreeQueryUsageAsync(userId);
                    var freeLimit = GetFreeQueryLimit();

                    var monthlyUsage = await GetFreeMonthlyQueryUsageAsync(userId);
                    var monthlyLimit = freeLimit * 30;

                    return freeUsage < freeLimit && monthlyUsage < monthlyLimit;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء التحقق من قدرة المستخدم على إجراء استعلام {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// إرسال إشعار قبل انتهاء الاشتراك
        /// </summary>
        public async Task SendExpirationNotificationsAsync()
        {
            try
            {
                // الحصول على الاشتراكات التي ستنتهي خلال الأيام القادمة
                var in3Days = DateTime.UtcNow.AddDays(3);
                var in7Days = DateTime.UtcNow.AddDays(7);

                var expiringSubscriptions = await _context.UserSubscriptions
                    .Include(s => s.User)
                    .Include(s => s.Plan)
                    .Where(s => s.Status == "Active" && !s.IsDeleted &&
                           s.EndDate > DateTime.UtcNow &&
                           (s.EndDate <= in3Days || s.EndDate <= in7Days))
                    .ToListAsync();

                foreach (var subscription in expiringSubscriptions)
                {
                    var daysLeft = (int)(subscription.EndDate.Value - DateTime.UtcNow).TotalDays;

                    // إضافة منطق لإرسال إشعارات البريد الإلكتروني/الرسائل النصية هنا
                    _logger.LogInformation("إرسال إشعار انتهاء الاشتراك للمستخدم {UserId}. أيام متبقية: {DaysLeft}",
                        subscription.UserId, daysLeft);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إرسال إشعارات انتهاء الاشتراك");
            }
        }

        /// <summary>
        /// الحصول على حالة اشتراك المستخدم (الواجهة الجديدة)
        /// </summary>
        public async Task<BaseResponse<SubscriptionStatusDTO>> GetUserSubscriptionStatusAsync(long userId, string language)
        {
            try
            {
                // البحث عن المستخدم
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    var errorMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<SubscriptionStatusDTO>.FailureResponse(errorMessage, 404);
                }

                // البحث عن اشتراك نشط للمستخدم
                var activeSubscription = await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.UserId == userId && s.Status == "Active" && !s.IsDeleted)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                var status = new SubscriptionStatusDTO
                {
                    UserId = userId,
                    IsActive = activeSubscription != null && activeSubscription.EndDate > DateTime.UtcNow,
                    PlanName = activeSubscription?.Plan.Name ?? "Free",
                    Status = activeSubscription?.Status ?? "Free"
                };

                if (activeSubscription != null)
                {
                    status.StartDate = activeSubscription.StartDate;
                    status.EndDate = activeSubscription.EndDate;
                    status.DaysRemaining = activeSubscription.EndDate.HasValue ? (long)(activeSubscription.EndDate.Value - DateTime.UtcNow).TotalDays : 0;
                    status.MonthlyQueryLimit = activeSubscription.Plan.MaxQueriesPerMonth;
                    status.QueriesUsedThisMonth = activeSubscription.QueriesUsedThisMonth;
                    //status.FileUploadSizeLimitMB = activeSubscription.Plan.MaxFileUploadSizeMB;
                    //status.IsTrial = activeSubscription.IsTrial;
                }
                else
                {
                    // الخطة المجانية
                    status.StartDate = DateTime.UtcNow;
                    status.EndDate = DateTime.UtcNow.AddYears(100); // الخطة المجانية لا تنتهي
                    status.DaysRemaining = 36500; // حوالي 100 سنة
                    status.MonthlyQueryLimit = GetFreeQueryLimit();
                    status.QueriesUsedThisMonth = await GetFreeMonthlyQueryUsageAsync(userId);
                    status.FileUploadSizeLimitMB = 2; // حد أقصى للخطة المجانية
                    status.IsTrial = false;
                }

                var successMessage = _localizationService.GetMessage("SubscriptionStatusRetrieved", "Messages", language);
                return BaseResponse<SubscriptionStatusDTO>.SuccessResponse(status, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على حالة اشتراك المستخدم {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionStatusError", "Errors", language);
                return BaseResponse<SubscriptionStatusDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث عدد الاستعلامات المستخدمة للمستخدم (الواجهة الجديدة)
        /// </summary>
        public async Task<BaseResponse<bool>> IncrementQueryCountAsync(long userId, string language)
        {
            try
            {
                // البحث عن اشتراك نشط للمستخدم
                var activeSubscription = await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.UserId == userId && s.Status == "Active" && !s.IsDeleted)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                if (activeSubscription != null && activeSubscription.EndDate > DateTime.UtcNow)
                {
                    // تحديث عدد الاستعلامات المستخدمة
                    activeSubscription.QueriesUsedThisMonth = (activeSubscription.QueriesUsedThisMonth) + 1;
                    activeSubscription.QueriesUsedToday = (activeSubscription.QueriesUsedToday) + 1;
                    activeSubscription.LastQueryDate = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    // تحديث عدد الاستعلامات للمستخدم المجاني
                    int currentUsage = await GetFreeMonthlyQueryUsageAsync(userId);
                    await StoreQueryUsageAsync($"free_monthly_queries_{userId}", currentUsage + 1);

                    int dailyUsage = await GetFreeQueryUsageAsync(userId);
                    await StoreQueryUsageAsync($"free_daily_queries_{userId}", dailyUsage + 1);
                }

                var successMessage = _localizationService.GetMessage("QueryCountIncremented", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث عدد الاستعلامات للمستخدم {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("QueryCountUpdateError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// التحقق من صلاحية اشتراك المستخدم (الواجهة الجديدة)
        /// </summary>
        public async Task<BaseResponse<bool>> IsSubscriptionValidAsync(long userId, string language)
        {
            try
            {
                bool isValid = await IsSubscriptionValidAsync(userId);

                if (isValid)
                {
                    var successMessage = _localizationService.GetMessage("SubscriptionValid", "Messages", language);
                    return BaseResponse<bool>.SuccessResponse(true, successMessage);
                }
                else
                {
                    var errorMessage = _localizationService.GetMessage("SubscriptionInvalid", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 400);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء التحقق من صلاحية اشتراك المستخدم {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionValidationError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إعادة تعيين عداد الاستعلامات الشهرية (الواجهة الجديدة)
        /// </summary>
        public async Task<BaseResponse<bool>> ResetMonthlyQueryCountersAsync(long? userId, string language)
        {
            try
            {
                if (userId.HasValue)
                {
                    // إعادة تعيين لمستخدم محدد
                    var subscription = await _context.UserSubscriptions
                        .Where(s => s.UserId == userId && s.Status == "Active" && !s.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (subscription != null)
                    {
                        subscription.QueriesUsedThisMonth = 0;
                        await _context.SaveChangesAsync();
                    }

                    // إعادة تعيين للمستخدم المجاني
                    await StoreQueryUsageAsync($"free_monthly_queries_{userId}", 0);
                }
                else
                {
                    // إعادة تعيين لجميع المستخدمين
                    var activeSubscriptions = await _context.UserSubscriptions
                        .Where(s => s.Status == "Active" && !s.IsDeleted)
                        .ToListAsync();

                    foreach (var subscription in activeSubscriptions)
                    {
                        subscription.QueriesUsedThisMonth = 0;
                    }

                    await _context.SaveChangesAsync();

                    // يمكن إضافة منطق لإعادة تعيين عدادات المستخدمين المجانيين هنا
                    // هذا يتطلب استخراج قائمة بجميع المستخدمين المجانيين
                }

                var successMessage = _localizationService.GetMessage("QueryCountersReset", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إعادة تعيين عدادات الاستعلامات الشهرية");
                var errorMessage = _localizationService.GetMessage("QueryCountersResetError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        #region Helper Methods

        /// <summary>
        /// الحصول على استخدام الاستعلامات المجانية للمستخدم اليوم
        /// </summary>
        private async Task<int> GetFreeQueryUsageAsync(long userId)
        {
            var today = DateTime.UtcNow.Date;
            var queryKey = $"free_query_usage:{userId}:{today:yyyy-MM-dd}";

            // في التنفيذ الفعلي، يمكن استخدام Redis/Memcached
            // هنا نستخدم طريقة أبسط لغرض العرض فقط
            var todayCount = await _context.ChatMessages
                .CountAsync(m => m.CreatedByUserId == userId &&
                            m.CreateDate.Date == today &&
                            m.Role == "user" &&
                            !m.IsDeleted);

            return todayCount;
        }

        /// <summary>
        /// الحصول على استخدام الاستعلامات المجانية للمستخدم هذا الشهر
        /// </summary>
        private async Task<int> GetFreeMonthlyQueryUsageAsync(long userId)
        {
            var today = DateTime.UtcNow.Date;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var monthlyQueryKey = $"free_query_usage:{userId}:{today:yyyy-MM}";

            // في التنفيذ الفعلي، يمكن استخدام Redis/Memcached
            // هنا نستخدم طريقة أبسط لغرض العرض فقط
            var monthCount = await _context.ChatMessages
                .CountAsync(m => m.CreatedByUserId == userId &&
                            m.CreateDate >= firstDayOfMonth &&
                            m.CreateDate.Date <= today &&
                            m.Role == "user" &&
                            !m.IsDeleted);

            return monthCount;
        }

        /// <summary>
        /// تخزين استخدام الاستعلامات
        /// </summary>
        private Task StoreQueryUsageAsync(string key, int value)
        {
            // في التنفيذ الفعلي، يمكن استخدام Redis/Memcached
            // هنا نعود فقط بمهمة مكتملة لغرض العرض فقط
            return Task.CompletedTask;
        }

        /// <summary>
        /// الحصول على حد الاستعلامات المجانية
        /// </summary>
        private int GetFreeQueryLimit()
        {
            // يمكن أن يكون هذا من الإعدادات
            return 5;
        }

        /// <summary>
        /// الحصول على ميزات الخطة المجانية
        /// </summary>
        private Task<List<string>> GetFreePlanFeaturesAsync()
        {
            var features = new List<string>
            {
                $"MaxQueries:{GetFreeQueryLimit()}",
                "MaxChatRooms:1",
                "MaxFiles:0",
                "AdvancedAnalytics:false",
                "PrioritySupport:false"
            };

            return Task.FromResult(features);
        }

        /// <summary>
        /// الحصول على ميزات خطة الاشتراك
        /// </summary>
        private async Task<List<string>> GetSubscriptionFeaturesAsync(long planId)
        {
            var plan = await _context.SubscriptionPlans
                .Where(p => p.Id == planId && !p.IsDeleted)
                .FirstOrDefaultAsync();

            if (plan == null)
            {
                return await GetFreePlanFeaturesAsync();
            }

            var features = new List<string>();

            var planFeatures = await _context.PlanFeatures
                .Where(f => f.PlanId == planId && !f.IsDeleted)
                .ToListAsync();

            foreach (var feature in planFeatures)
            {
                features.Add($"{feature.Feature}");
            }

            // إضافة ميزات إضافية من الخطة
            features.Add($"MaxQueries:{plan.MaxQueriesPerDay}");
            features.Add($"MaxChatRooms:{plan.AllowedChatRooms}");
            features.Add($"MaxFiles:{plan.AllowedFiles}");
            features.Add($"MaxFileSize:{(plan.AllowedFileSizeMb != null ? plan.AllowedFileSizeMb : 0)}");

            return features;
        }

        #endregion
    }
}