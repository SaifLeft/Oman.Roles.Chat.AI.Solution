using API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Transactions;

namespace Services
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// الحصول على جميع خطط الاشتراك
        /// </summary>
        Task<BaseResponse<List<SubscriptionPlan>>> GetAllPlansAsync(string language);

        /// <summary>
        /// الحصول على خطة اشتراك بواسطة المعرف
        /// </summary>
        Task<BaseResponse<SubscriptionPlan>> GetPlanByIdAsync(string planId, string language);

        /// <summary>
        /// الحصول على اشتراك المستخدم
        /// </summary>
        Task<BaseResponse<UserSubscription>> GetUserSubscriptionAsync(string userId, string language);

        /// <summary>
        /// إنشاء اشتراك جديد
        /// </summary>
        Task<BaseResponse<UserSubscription>> CreateSubscriptionAsync(string userId, CreateSubscriptionRequest request, string language);

        /// <summary>
        /// تجديد اشتراك
        /// </summary>
        Task<BaseResponse<UserSubscription>> RenewSubscriptionAsync(string userId, RenewSubscriptionRequest request, string language);

        /// <summary>
        /// إيقاف التجديد التلقائي
        /// </summary>
        Task<BaseResponse<UserSubscription>> CancelAutoRenewalAsync(string userId, string subscriptionId, string language);

        /// <summary>
        /// إلغاء اشتراك
        /// </summary>
        Task<BaseResponse<UserSubscription>> CancelSubscriptionAsync(string userId, string subscriptionId, string language);

        /// <summary>
        /// التحقق من صلاحية كوبون
        /// </summary>
        Task<BaseResponse<CouponValidationResponse>> ValidateCouponAsync(ValidateCouponRequest request, string language);

        /// <summary>
        /// الحصول على معاملات المستخدم
        /// </summary>
        Task<BaseResponse<List<FinancialTransaction>>> GetUserTransactionsAsync(string userId, string language);
    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SubscriptionService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly string _plansPath;
        private readonly string _subscriptionsPath;
        private readonly string _couponsPath;
        private readonly string _transactionsPath;

        private readonly ConcurrentDictionary<string, SubscriptionPlan> _plans = new();
        private readonly ConcurrentDictionary<string, UserSubscription> _subscriptions = new();
        private readonly ConcurrentDictionary<string, DiscountCoupon> _coupons = new();
        private readonly ConcurrentDictionary<string, List<FinancialTransaction>> _userTransactions = new();

        private readonly object _fileLock = new object();

        // خطط افتراضية
        private readonly List<SubscriptionPlan> _defaultPlans = new()
        {
            new SubscriptionPlan
            {
                Id = "basic",
                Name = "الخطة الأساسية",
                Description = "خطة أساسية للاستخدام الفردي",
                PriceMonthly = 9.99m,
                PriceYearly = 99.99m,
                AllowedChatRooms = 3,
                AllowedFiles = 10,
                AllowedFileSizeMb = 50,
                IsActive = true,
                IsTrial = false,
                Features = new List<string>
                {
                    "إنشاء 3 غرف دردشة",
                    "رفع 10 ملفات PDF",
                    "حجم الملف حتى 50 ميجابايت",
                    "دعم فني بالبريد الإلكتروني"
                }
            },
            new SubscriptionPlan
            {
                Id = "pro",
                Name = "الخطة الاحترافية",
                Description = "خطة احترافية للاستخدام المتقدم",
                PriceMonthly = 24.99m,
                PriceYearly = 249.99m,
                AllowedChatRooms = 10,
                AllowedFiles = 50,
                AllowedFileSizeMb = 200,
                IsActive = true,
                IsTrial = false,
                Features = new List<string>
                {
                    "إنشاء 10 غرف دردشة",
                    "رفع 50 ملف PDF",
                    "حجم الملف حتى 200 ميجابايت",
                    "تخصيص قواعد الدردشة",
                    "دعم فني على مدار الساعة"
                }
            },
            new SubscriptionPlan
            {
                Id = "enterprise",
                Name = "خطة المؤسسات",
                Description = "خطة متكاملة للشركات والمؤسسات",
                PriceMonthly = 99.99m,
                PriceYearly = 999.99m,
                AllowedChatRooms = 100,
                AllowedFiles = 500,
                AllowedFileSizeMb = 500,
                IsActive = true,
                IsTrial = false,
                Features = new List<string>
                {
                    "غرف دردشة غير محدودة",
                    "رفع 500 ملف PDF",
                    "حجم الملف حتى 500 ميجابايت",
                    "تخصيص كامل للقواعد",
                    "دعم فني بالأولوية",
                    "تكامل مع أنظمة الشركة",
                    "تقارير وتحليلات متقدمة"
                }
            },
            new SubscriptionPlan
            {
                Id = "trial",
                Name = "الخطة التجريبية",
                Description = "خطة تجريبية مجانية لمدة 14 يوم",
                PriceMonthly = 0,
                PriceYearly = 0,
                AllowedChatRooms = 1,
                AllowedFiles = 3,
                AllowedFileSizeMb = 10,
                IsActive = true,
                IsTrial = true,
                TrialDays = 14,
                Features = new List<string>
                {
                    "إنشاء غرفة دردشة واحدة",
                    "رفع 3 ملفات PDF",
                    "حجم الملف حتى 10 ميجابايت",
                    "دعم فني أساسي"
                }
            }
        };

        // كوبونات افتراضية
        private readonly List<DiscountCoupon> _defaultCoupons = new()
        {
            new DiscountCoupon
            {
                Id = "welcome2023",
                Code = "WELCOME2023",
                Description = "خصم 20% للمستخدمين الجدد",
                DiscountType = DiscountType.Percentage,
                DiscountValue = 20,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 12, 31),
                MaxUses = 1000,
                CurrentUses = 0,
                IsActive = true
            },
            new DiscountCoupon
            {
                Id = "summer2023",
                Code = "SUMMER2023",
                Description = "خصم صيف 2023 بقيمة 15%",
                DiscountType = DiscountType.Percentage,
                DiscountValue = 15,
                StartDate = new DateTime(2023, 6, 1),
                EndDate = new DateTime(2023, 8, 31),
                MaxUses = 500,
                CurrentUses = 0,
                IsActive = true
            }
        };

        public SubscriptionService(
            IConfiguration configuration,
            ILogger<SubscriptionService> logger,
            ILocalizationService localizationService)
        {
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;

            // تحديد مسارات حفظ البيانات
            var subscriptionDataPath = _configuration["SubscriptionSettings:DataPath"] ??
                                      Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SubscriptionData");

            _plansPath = Path.Combine(subscriptionDataPath, "Plans");
            _subscriptionsPath = Path.Combine(subscriptionDataPath, "Subscriptions");
            _couponsPath = Path.Combine(subscriptionDataPath, "Coupons");
            _transactionsPath = Path.Combine(subscriptionDataPath, "Transactions");

            // التأكد من وجود المسارات
            Directory.CreateDirectory(_plansPath);
            Directory.CreateDirectory(_subscriptionsPath);
            Directory.CreateDirectory(_couponsPath);
            Directory.CreateDirectory(_transactionsPath);

            // تحميل البيانات
            LoadPlans();
            LoadSubscriptions();
            LoadCoupons();
            LoadTransactions();
        }

        /// <summary>
        /// الحصول على جميع خطط الاشتراك
        /// </summary>
        public async Task<BaseResponse<List<SubscriptionPlan>>> GetAllPlansAsync(string language)
        {
            try
            {
                var plans = _plans.Values.Where(p => p.IsActive).ToList();
                return BaseResponse<List<SubscriptionPlan>>.SuccessResponse(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على خطط الاشتراك");
                var errorMessage = _localizationService.GetMessage("PlansRetrievalError", "Errors", language);
                return BaseResponse<List<SubscriptionPlan>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على خطة اشتراك بواسطة المعرف
        /// </summary>
        public async Task<BaseResponse<SubscriptionPlan>> GetPlanByIdAsync(string planId, string language)
        {
            try
            {
                if (!_plans.TryGetValue(planId, out var plan) || !plan.IsActive)
                {
                    var errorMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<SubscriptionPlan>.FailureResponse(errorMessage, 404);
                }

                return BaseResponse<SubscriptionPlan>.SuccessResponse(plan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على خطة الاشتراك {planId}", planId);
                var errorMessage = _localizationService.GetMessage("PlanRetrievalError", "Errors", language);
                return BaseResponse<SubscriptionPlan>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على اشتراك المستخدم
        /// </summary>
        public async Task<BaseResponse<UserSubscription>> GetUserSubscriptionAsync(string userId, string language)
        {
            try
            {
                var subscription = _subscriptions.Values
                    .Where(s => s.UserId == userId && s.Status != SubscriptionStatus.Cancelled)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefault();

                if (subscription == null)
                {
                    var errorMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 404);
                }

                // التحقق من حالة الاشتراك وتحديثها إذا لزم الأمر
                if (subscription.EndDate < DateTime.UtcNow)
                {
                    if (subscription.Status != SubscriptionStatus.Expired)
                    {
                        subscription.Status = SubscriptionStatus.Expired;
                        subscription.UpdatedAt = DateTime.UtcNow;
                        SaveSubscription(subscription);
                    }
                }

                return BaseResponse<UserSubscription>.SuccessResponse(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على اشتراك المستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionRetrievalError", "Errors", language);
                return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إنشاء اشتراك جديد
        /// </summary>
        public async Task<BaseResponse<UserSubscription>> CreateSubscriptionAsync(string userId, CreateSubscriptionRequest request, string language)
        {
            try
            {
                // التحقق من وجود اشتراك نشط للمستخدم
                var existingSubscription = _subscriptions.Values
                    .Where(s => s.UserId == userId &&
                               (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial))
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefault();

                if (existingSubscription != null && existingSubscription.EndDate > DateTime.UtcNow)
                {
                    var errorMessage = _localizationService.GetMessage("ActiveSubscriptionExists", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 400);
                }

                // التحقق من وجود الخطة
                if (!_plans.TryGetValue(request.PlanId, out var plan) || !plan.IsActive)
                {
                    var errorMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 404);
                }

                // حساب سعر الاشتراك
                decimal price = request.PeriodType == SubscriptionPeriodType.Monthly
                    ? plan.PriceMonthly
                    : plan.PriceYearly;

                decimal discountAmount = 0;
                string? couponCode = null;

                // التحقق من كوبون الخصم
                if (!string.IsNullOrEmpty(request.CouponCode))
                {
                    var couponValidation = await ValidateCouponInternalAsync(request.CouponCode, request.PlanId);
                    if (couponValidation.IsValid)
                    {
                        if (couponValidation.DiscountType == DiscountType.Percentage)
                        {
                            discountAmount = price * (couponValidation.DiscountValue / 100);
                        }
                        else
                        {
                            discountAmount = couponValidation.DiscountValue;
                        }

                        couponCode = request.CouponCode;
                    }
                }

                // حساب المبلغ النهائي
                decimal totalAmount = price - discountAmount;
                if (totalAmount < 0) totalAmount = 0;

                // حساب تاريخ انتهاء الاشتراك
                DateTime startDate = DateTime.UtcNow;
                DateTime endDate = request.PeriodType == SubscriptionPeriodType.Monthly
                    ? startDate.AddMonths(1)
                    : startDate.AddYears(1);

                // تحديد حالة الاشتراك
                var status = plan.IsTrial ? SubscriptionStatus.Trial : SubscriptionStatus.Active;

                // إذا كانت خطة تجريبية، استخدام مدة التجربة المحددة
                if (plan.IsTrial && plan.TrialDays.HasValue)
                {
                    endDate = startDate.AddDays(plan.TrialDays.Value);
                }

                // إنشاء رقم فاتورة فريد
                string invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";

                // إنشاء الاشتراك
                var subscription = new UserSubscription
                {
                    UserId = userId,
                    PlanId = plan.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    PeriodType = request.PeriodType,
                    Status = status,
                    AutoRenew = request.AutoRenew,
                    LastInvoiceNumber = invoiceNumber
                };

                // إنشاء معاملة مالية
                var transaction = new FinancialTransaction
                {
                    UserId = userId,
                    SubscriptionId = subscription.Id,
                    PlanId = plan.Id,
                    Type = TransactionType.NewSubscription,
                    Amount = price,
                    DiscountAmount = discountAmount,
                    TotalAmount = totalAmount,
                    CouponCode = couponCode,
                    InvoiceNumber = invoiceNumber,
                    Status = TransactionStatus.Completed
                };

                // تحديث استخدام الكوبون إذا وجد
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = _coupons.Values.FirstOrDefault(c => c.Code.Equals(couponCode, StringComparison.OrdinalIgnoreCase));
                    if (coupon != null)
                    {
                        coupon.CurrentUses++;
                        SaveCoupon(coupon);
                    }
                }

                // حفظ الاشتراك والمعاملة
                _subscriptions[subscription.Id] = subscription;
                SaveSubscription(subscription);

                // حفظ المعاملة
                if (!_userTransactions.TryGetValue(userId, out var userTransactions))
                {
                    userTransactions = new List<FinancialTransaction>();
                    _userTransactions[userId] = userTransactions;
                }
                userTransactions.Add(transaction);
                SaveTransaction(transaction);

                var successMessage = _localizationService.GetMessage("SubscriptionCreated", "Messages", language);
                return BaseResponse<UserSubscription>.SuccessResponse(subscription, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إنشاء اشتراك جديد للمستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionCreationError", "Errors", language);
                return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تجديد اشتراك
        /// </summary>
        public async Task<BaseResponse<UserSubscription>> RenewSubscriptionAsync(string userId, RenewSubscriptionRequest request, string language)
        {
            try
            {
                // التحقق من وجود الاشتراك
                if (!_subscriptions.TryGetValue(request.SubscriptionId, out var subscription))
                {
                    var errorMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 404);
                }

                // التحقق من ملكية الاشتراك
                if (subscription.UserId != userId)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedOperation", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 403);
                }

                // التحقق من حالة الاشتراك
                if (subscription.Status == SubscriptionStatus.Cancelled)
                {
                    var errorMessage = _localizationService.GetMessage("CannotRenewCancelledSubscription", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 400);
                }

                // التحقق من وجود الخطة
                if (!_plans.TryGetValue(subscription.PlanId, out var plan) || !plan.IsActive)
                {
                    var errorMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 404);
                }

                // حساب سعر التجديد
                decimal price = subscription.PeriodType == SubscriptionPeriodType.Monthly
                    ? plan.PriceMonthly
                    : plan.PriceYearly;

                decimal discountAmount = 0;
                string? couponCode = null;

                // التحقق من كوبون الخصم
                if (!string.IsNullOrEmpty(request.CouponCode))
                {
                    var couponValidation = await ValidateCouponInternalAsync(request.CouponCode, subscription.PlanId);
                    if (couponValidation.IsValid)
                    {
                        if (couponValidation.DiscountType == DiscountType.Percentage)
                        {
                            discountAmount = price * (couponValidation.DiscountValue / 100);
                        }
                        else
                        {
                            discountAmount = couponValidation.DiscountValue;
                        }

                        couponCode = request.CouponCode;
                    }
                }

                // حساب المبلغ النهائي
                decimal totalAmount = price - discountAmount;
                if (totalAmount < 0) totalAmount = 0;

                // حساب تاريخ انتهاء الاشتراك الجديد
                DateTime renewalStartDate = subscription.EndDate > DateTime.UtcNow
                    ? subscription.EndDate // استخدام تاريخ الانتهاء الحالي إذا لم ينته بعد
                    : DateTime.UtcNow; // استخدام التاريخ الحالي إذا انتهى بالفعل

                DateTime renewalEndDate = subscription.PeriodType == SubscriptionPeriodType.Monthly
                    ? renewalStartDate.AddMonths(1)
                    : renewalStartDate.AddYears(1);

                // إنشاء رقم فاتورة فريد
                string invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";

                // تحديث الاشتراك
                subscription.StartDate = renewalStartDate;
                subscription.EndDate = renewalEndDate;
                subscription.Status = SubscriptionStatus.Active;
                subscription.LastRenewalDate = DateTime.UtcNow;
                subscription.LastInvoiceNumber = invoiceNumber;
                subscription.UpdatedAt = DateTime.UtcNow;

                // إنشاء معاملة مالية
                var transaction = new FinancialTransaction
                {
                    UserId = userId,
                    SubscriptionId = subscription.Id,
                    PlanId = subscription.PlanId,
                    Type = TransactionType.Renewal,
                    Amount = price,
                    DiscountAmount = discountAmount,
                    TotalAmount = totalAmount,
                    CouponCode = couponCode,
                    InvoiceNumber = invoiceNumber,
                    Status = TransactionStatus.Completed
                };

                // تحديث استخدام الكوبون إذا وجد
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = _coupons.Values.FirstOrDefault(c => c.Code.Equals(couponCode, StringComparison.OrdinalIgnoreCase));
                    if (coupon != null)
                    {
                        coupon.CurrentUses++;
                        SaveCoupon(coupon);
                    }
                }

                // حفظ التغييرات
                SaveSubscription(subscription);

                // حفظ المعاملة
                if (!_userTransactions.TryGetValue(userId, out var userTransactions))
                {
                    userTransactions = new List<FinancialTransaction>();
                    _userTransactions[userId] = userTransactions;
                }
                userTransactions.Add(transaction);
                SaveTransaction(transaction);

                var successMessage = _localizationService.GetMessage("SubscriptionRenewed", "Messages", language);
                return BaseResponse<UserSubscription>.SuccessResponse(subscription, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تجديد الاشتراك {subscriptionId} للمستخدم {userId}", request.SubscriptionId, userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionRenewalError", "Errors", language);
                return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إيقاف التجديد التلقائي
        /// </summary>
        public async Task<BaseResponse<UserSubscription>> CancelAutoRenewalAsync(string userId, string subscriptionId, string language)
        {
            try
            {
                // التحقق من وجود الاشتراك
                if (!_subscriptions.TryGetValue(subscriptionId, out var subscription))
                {
                    var errorMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 404);
                }

                // التحقق من ملكية الاشتراك
                if (subscription.UserId != userId)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedOperation", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 403);
                }

                // تحديث حالة التجديد التلقائي
                subscription.AutoRenew = false;
                subscription.UpdatedAt = DateTime.UtcNow;
                subscription.Notes = (subscription.Notes ?? "") + $"\nAuto-renewal cancelled on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";

                // حفظ التغييرات
                SaveSubscription(subscription);

                var successMessage = _localizationService.GetMessage("AutoRenewalCancelled", "Messages", language);
                return BaseResponse<UserSubscription>.SuccessResponse(subscription, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إيقاف التجديد التلقائي للاشتراك {subscriptionId} للمستخدم {userId}", subscriptionId, userId);
                var errorMessage = _localizationService.GetMessage("AutoRenewalCancellationError", "Errors", language);
                return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إلغاء اشتراك
        /// </summary>
        public async Task<BaseResponse<UserSubscription>> CancelSubscriptionAsync(string userId, string subscriptionId, string language)
        {
            try
            {
                // التحقق من وجود الاشتراك
                if (!_subscriptions.TryGetValue(subscriptionId, out var subscription))
                {
                    var errorMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 404);
                }

                // التحقق من ملكية الاشتراك
                if (subscription.UserId != userId)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedOperation", "Errors", language);
                    return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 403);
                }

                // تحديث حالة الاشتراك
                subscription.Status = SubscriptionStatus.Cancelled;
                subscription.AutoRenew = false;
                subscription.UpdatedAt = DateTime.UtcNow;
                subscription.Notes = (subscription.Notes ?? "") + $"\nSubscription cancelled on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";

                // حفظ التغييرات
                SaveSubscription(subscription);

                var successMessage = _localizationService.GetMessage("SubscriptionCancelled", "Messages", language);
                return BaseResponse<UserSubscription>.SuccessResponse(subscription, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إلغاء الاشتراك {subscriptionId} للمستخدم {userId}", subscriptionId, userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionCancellationError", "Errors", language);
                return BaseResponse<UserSubscription>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// التحقق من صلاحية كوبون
        /// </summary>
        public async Task<BaseResponse<CouponValidationResponse>> ValidateCouponAsync(ValidateCouponRequest request, string language)
        {
            try
            {
                var validation = await ValidateCouponInternalAsync(request.CouponCode, request.PlanId);

                if (!validation.IsValid)
                {
                    return BaseResponse<CouponValidationResponse>.FailureResponse(validation.ErrorMessage ?? "Invalid coupon", 400, new List<string> { validation.ErrorMessage ?? "Invalid coupon" });
                }

                return BaseResponse<CouponValidationResponse>.SuccessResponse(validation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء التحقق من صلاحية الكوبون {couponCode}", request.CouponCode);
                var errorMessage = _localizationService.GetMessage("CouponValidationError", "Errors", language);
                return BaseResponse<CouponValidationResponse>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على معاملات المستخدم
        /// </summary>
        public async Task<BaseResponse<List<FinancialTransaction>>> GetUserTransactionsAsync(string userId, string language)
        {
            try
            {
                if (!_userTransactions.TryGetValue(userId, out var transactions))
                {
                    return BaseResponse<List<FinancialTransaction>>.SuccessResponse(new List<FinancialTransaction>());
                }

                var orderedTransactions = transactions.OrderByDescending(t => t.TransactionDate).ToList();
                return BaseResponse<List<FinancialTransaction>>.SuccessResponse(orderedTransactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على معاملات المستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("TransactionsRetrievalError", "Errors", language);
                return BaseResponse<List<FinancialTransaction>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// التحقق من صلاحية كوبون داخليًا
        /// </summary>
        private async Task<CouponValidationResponse> ValidateCouponInternalAsync(string couponCode, string planId)
        {
            var response = new CouponValidationResponse { IsValid = false };

            // البحث عن الكوبون
            var coupon = _coupons.Values.FirstOrDefault(c => c.Code.Equals(couponCode, StringComparison.OrdinalIgnoreCase));

            if (coupon == null)
            {
                response.ErrorMessage = "Coupon not found";
                return response;
            }

            // التحقق من نشاط الكوبون
            if (!coupon.IsActive)
            {
                response.ErrorMessage = "Coupon is not active";
                return response;
            }

            // التحقق من تاريخ الصلاحية
            if (coupon.StartDate > DateTime.UtcNow)
            {
                response.ErrorMessage = "Coupon is not yet valid";
                return response;
            }

            if (coupon.EndDate.HasValue && coupon.EndDate.Value < DateTime.UtcNow)
            {
                response.ErrorMessage = "Coupon has expired";
                return response;
            }

            // التحقق من عدد الاستخدامات
            if (coupon.MaxUses.HasValue && coupon.CurrentUses >= coupon.MaxUses.Value)
            {
                response.ErrorMessage = "Coupon usage limit has been reached";
                return response;
            }

            // التحقق من تطبيق الكوبون على الخطة
            if (coupon.ApplicablePlanIds.Count > 0 && !coupon.ApplicablePlanIds.Contains(planId))
            {
                response.ErrorMessage = "Coupon is not applicable for this plan";
                return response;
            }

            // الكوبون صالح
            response.IsValid = true;
            response.DiscountType = coupon.DiscountType;
            response.DiscountValue = coupon.DiscountValue;

            return response;
        }

        #region Data Loading and Saving Methods

        /// <summary>
        /// تحميل خطط الاشتراك
        /// </summary>
        private void LoadPlans()
        {
            try
            {
                var files = Directory.GetFiles(_plansPath, "*.json");

                if (files.Length == 0)
                {
                    // إنشاء الخطط الافتراضية إذا لم توجد خطط
                    foreach (var plan in _defaultPlans)
                    {
                        _plans[plan.Id] = plan;
                        SavePlan(plan);
                    }
                }
                else
                {
                    foreach (var file in files)
                    {
                        var json = File.ReadAllText(file);
                        var plan = JsonSerializer.Deserialize<SubscriptionPlan>(json);
                        if (plan != null)
                        {
                            _plans[plan.Id] = plan;
                        }
                    }
                }

                _logger.LogInformation("تم تحميل {count} خطة اشتراك", _plans.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل خطط الاشتراك");
            }
        }

        /// <summary>
        /// تحميل الاشتراكات
        /// </summary>
        private void LoadSubscriptions()
        {
            try
            {
                var files = Directory.GetFiles(_subscriptionsPath, "*.json");
                foreach (var file in files)
                {
                    var json = File.ReadAllText(file);
                    var subscription = JsonSerializer.Deserialize<UserSubscription>(json);
                    if (subscription != null)
                    {
                        _subscriptions[subscription.Id] = subscription;
                    }
                }

                _logger.LogInformation("تم تحميل {count} اشتراك", _subscriptions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل الاشتراكات");
            }
        }

        /// <summary>
        /// تحميل كوبونات الخصم
        /// </summary>
        private void LoadCoupons()
        {
            try
            {
                var files = Directory.GetFiles(_couponsPath, "*.json");

                if (files.Length == 0)
                {
                    // إنشاء الكوبونات الافتراضية إذا لم توجد كوبونات
                    foreach (var coupon in _defaultCoupons)
                    {
                        _coupons[coupon.Id] = coupon;
                        SaveCoupon(coupon);
                    }
                }
                else
                {
                    foreach (var file in files)
                    {
                        var json = File.ReadAllText(file);
                        var coupon = JsonSerializer.Deserialize<DiscountCoupon>(json);
                        if (coupon != null)
                        {
                            _coupons[coupon.Id] = coupon;
                        }
                    }
                }

                _logger.LogInformation("تم تحميل {count} كوبون خصم", _coupons.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل كوبونات الخصم");
            }
        }

        /// <summary>
        /// تحميل المعاملات المالية
        /// </summary>
        private void LoadTransactions()
        {
            try
            {
                var files = Directory.GetFiles(_transactionsPath, "*.json");
                foreach (var file in files)
                {
                    var json = File.ReadAllText(file);
                    var transaction = JsonSerializer.Deserialize<FinancialTransaction>(json);
                    if (transaction != null)
                    {
                        if (!_userTransactions.TryGetValue(transaction.UserId, out var userTransactions))
                        {
                            userTransactions = new List<FinancialTransaction>();
                            _userTransactions[transaction.UserId] = userTransactions;
                        }
                        userTransactions.Add(transaction);
                    }
                }

                _logger.LogInformation("تم تحميل معاملات لـ {count} مستخدم", _userTransactions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل المعاملات المالية");
            }
        }

        /// <summary>
        /// حفظ خطة اشتراك
        /// </summary>
        private void SavePlan(SubscriptionPlan plan)
        {
            try
            {
                lock (_fileLock)
                {
                    var planPath = Path.Combine(_plansPath, $"{plan.Id}.json");
                    var json = JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(planPath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حفظ خطة الاشتراك {planId}", plan.Id);
            }
        }

        /// <summary>
        /// حفظ اشتراك
        /// </summary>
        private void SaveSubscription(UserSubscription subscription)
        {
            try
            {
                lock (_fileLock)
                {
                    var subscriptionPath = Path.Combine(_subscriptionsPath, $"{subscription.Id}.json");
                    var json = JsonSerializer.Serialize(subscription, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(subscriptionPath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حفظ الاشتراك {subscriptionId}", subscription.Id);
            }
        }

        /// <summary>
        /// حفظ كوبون خصم
        /// </summary>
        private void SaveCoupon(DiscountCoupon coupon)
        {
            try
            {
                lock (_fileLock)
                {
                    var couponPath = Path.Combine(_couponsPath, $"{coupon.Id}.json");
                    var json = JsonSerializer.Serialize(coupon, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(couponPath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حفظ كوبون الخصم {couponId}", coupon.Id);
            }
        }

        /// <summary>
        /// حفظ معاملة مالية
        /// </summary>
        private void SaveTransaction(UserSubscription transaction)
        {
            try
            {
                lock (_fileLock)
                {
                    var transactionPath = Path.Combine(_transactionsPath, $"{transaction.Id}.json");
                    var json = JsonSerializer.Serialize(transaction, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(transactionPath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حفظ المعاملة المالية {transactionId}", transaction.Id);
            }
        }

        #endregion
    }
}