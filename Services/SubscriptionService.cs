using AutoMapper;
using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.Subscription;
using Models.DTOs.Subscription.Enums;
using Models.DTOs.Subscription.Requests;
using Models.DTOs.Subscription.Responses;

namespace Services
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Get all subscriptions
        /// </summary>
        Task<BaseResponse<List<SubscriptionPlanDTO>>> GetAllSubscriptionsAsync(int page, int pageSize, string language);

        /// <summary>
        /// Get specific subscription
        /// </summary>
        Task<BaseResponse<SubscriptionPlanDTO>> GetSubscriptionByIdAsync(string id, string language);

        /// <summary>
        /// Get subscriptions for a specific user
        /// </summary>
        Task<BaseResponse<List<UserSubscriptionDTO>>> GetUserSubscriptionsHistoryAsync(string userId, string language);

        /// <summary>
        /// Create a new subscription plan
        /// </summary>
        Task<BaseResponse<SubscriptionPlanDTO>> CreateSubscriptionPlanAsync(CreateSubscriptionPlanRequestDTO request, string adminId, string language);

        /// <summary>
        /// Update a subscription plan
        /// </summary>
        Task<BaseResponse<SubscriptionPlanDTO>> UpdateSubscriptionPlanAsync(string id, UpdateSubscriptionPlanRequestDTO request, string adminId, string language);

        /// <summary>
        /// Create a new discount coupon
        /// </summary>
        Task<BaseResponse<DiscountCouponDTO>> CreateDiscountCouponAsync(CreateDiscountCouponRequestDTO request, string adminId, string language);

        /// <summary>
        /// Get all discount coupons
        /// </summary>
        Task<BaseResponse<List<DiscountCouponDTO>>> GetAllDiscountCouponsAsync(int page, int pageSize, string language);

        /// <summary>
        /// Update a discount coupon
        /// </summary>
        Task<BaseResponse<DiscountCouponDTO>> UpdateDiscountCouponAsync(string id, UpdateDiscountCouponRequestDTO request, string adminId, string language);

        /// <summary>
        /// Get subscription reports
        /// </summary>
        Task<BaseResponse<SubscriptionReportDTO>> GetSubscriptionReportsAsync(DateTime? startDate, DateTime? endDate, string language);
        Task<BaseResponse<UserSubscriptionDTO>> CreateSubscription(string userId, CreateSubscriptionRequestDTO request, string language);
    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly MuhamiContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SubscriptionService> _logger;
        private readonly ILocalizationService _localizationService;

        public SubscriptionService(
            MuhamiContext context,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<SubscriptionService> logger,
            ILocalizationService localizationService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;
        }

        public async Task<BaseResponse<List<SubscriptionPlanDTO>>> GetAllSubscriptionsAsync(int page, int pageSize, string language)
        {
            try
            {
                var subscriptions = await _context.SubscriptionPlans
                    .Where(s => s.IsDeleted != true)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var subscriptionDtos = _mapper.Map<List<SubscriptionPlanDTO>>(subscriptions);
                return BaseResponse<List<SubscriptionPlanDTO>>.SuccessResponse(subscriptionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all subscriptions");
                var errorMessage = _localizationService.GetMessage("SubscriptionsRetrievalError", "Errors", language);
                return BaseResponse<List<SubscriptionPlanDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        public async Task<BaseResponse<SubscriptionPlanDTO>> GetSubscriptionByIdAsync(string id, string language)
        {
            try
            {
                if (!long.TryParse(id, out long subscriptionId))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidSubscriptionId", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(invalidIdMessage, 400);
                }

                var subscription = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.IsDeleted != true);

                if (subscription == null)
                {
                    var notFoundMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(notFoundMessage, 404);
                }

                var subscriptionDto = _mapper.Map<SubscriptionPlanDTO>(subscription);
                return BaseResponse<SubscriptionPlanDTO>.SuccessResponse(subscriptionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription {SubscriptionId}", id);
                var errorMessage = _localizationService.GetMessage("SubscriptionRetrievalError", "Errors", language);
                return BaseResponse<SubscriptionPlanDTO>.FailureResponse(errorMessage, 500);
            }
        }

        public async Task<BaseResponse<List<UserSubscriptionDTO>>> GetUserSubscriptionsHistoryAsync(string userId, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<List<UserSubscriptionDTO>>.FailureResponse(invalidIdMessage, 400);
                }

                var subscriptions = await _context.UserSubscriptions
                    .Where(s => s.UserId == userIdLong && s.IsDeleted != true)
                    .OrderByDescending(s => s.CreateDate)
                    .ToListAsync();

                var subscriptionDtos = _mapper.Map<List<UserSubscriptionDTO>>(subscriptions);
                return BaseResponse<List<UserSubscriptionDTO>>.SuccessResponse(subscriptionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user subscriptions history {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("UserSubscriptionsRetrievalError", "Errors", language);
                return BaseResponse<List<UserSubscriptionDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        public async Task<BaseResponse<SubscriptionPlanDTO>> CreateSubscriptionPlanAsync(CreateSubscriptionPlanRequestDTO request, string adminId, string language)
        {
            try
            {
                if (!long.TryParse(adminId, out long adminIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidAdminId", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(invalidIdMessage, 400);
                }

                var subscriptionPlan = new SubscriptionPlan
                {
                    Name = request.Name,
                    Description = request.Description,
                    PriceMonthly = request.PriceMonthly,
                    PriceYearly = request.PriceYearly,
                    IsTrial = request.IsTrial,
                    TrialDays = request.TrialDays,
                    IsActive = true,
                    CreatedByUserId = adminIdLong,
                    CreateDate = DateTime.Now
                };

                _context.SubscriptionPlans.Add(subscriptionPlan);
                await _context.SaveChangesAsync();

                var subscriptionDto = _mapper.Map<SubscriptionPlanDTO>(subscriptionPlan);
                return BaseResponse<SubscriptionPlanDTO>.SuccessResponse(subscriptionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                var errorMessage = _localizationService.GetMessage("SubscriptionPlanCreationError", "Errors", language);
                return BaseResponse<SubscriptionPlanDTO>.FailureResponse(errorMessage, 500);
            }
        }

        public async Task<BaseResponse<SubscriptionPlanDTO>> UpdateSubscriptionPlanAsync(string id, UpdateSubscriptionPlanRequestDTO request, string adminId, string language)
        {
            try
            {
                if (!long.TryParse(id, out long planId) || !long.TryParse(adminId, out long adminIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidId", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(invalidIdMessage, 400);
                }

                var plan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.Id == planId && p.IsDeleted != true);

                if (plan == null)
                {
                    var notFoundMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(notFoundMessage, 404);
                }

                plan.Name = request.Name;
                plan.Description = request.Description;
                plan.PriceMonthly = (double)request.PriceMonthly;
                plan.PriceYearly = (double)request.PriceYearly;
                plan.IsTrial = request.IsTrial ?? false;
                plan.TrialDays = request.TrialDays;
                plan.IsActive = request.IsActive ?? false;
                plan.ModifiedByUserId = adminIdLong;
                plan.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                var planDto = _mapper.Map<SubscriptionPlanDTO>(plan);
                return BaseResponse<SubscriptionPlanDTO>.SuccessResponse(planDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan {PlanId}", id);
                var errorMessage = _localizationService.GetMessage("SubscriptionPlanUpdateError", "Errors", language);
                return BaseResponse<SubscriptionPlanDTO>.FailureResponse(errorMessage, 500);
            }
        }

        public async Task<BaseResponse<DiscountCouponDTO>> CreateDiscountCouponAsync(CreateDiscountCouponRequestDTO request, string adminId, string language)
        {
            try
            {
                if (!long.TryParse(adminId, out long adminIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidAdminId", "Errors", language);
                    return BaseResponse<DiscountCouponDTO>.FailureResponse(invalidIdMessage, 400);
                }

                var coupon = new DiscountCoupon
                {
                    Code = request.Code,
                    Description = request.Description,
                    DiscountType = request.DiscountType.ToString(),
                    DiscountValue = (double)request.DiscountValue,
                    MaxUses = request.MaxUses,
                    CurrentUses = 0,
                    StartDate = DateTime.Now,
                    EndDate = request.ExpiryDate,
                    IsActive = true,
                    CreatedByUserId = adminIdLong,
                    CreateDate = DateTime.Now
                };

                _context.DiscountCoupons.Add(coupon);
                await _context.SaveChangesAsync();

                var couponDto = _mapper.Map<DiscountCouponDTO>(coupon);
                return BaseResponse<DiscountCouponDTO>.SuccessResponse(couponDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating discount coupon");
                var errorMessage = _localizationService.GetMessage("DiscountCouponCreationError", "Errors", language);
                return BaseResponse<DiscountCouponDTO>.FailureResponse(errorMessage, 500);
            }
        }

        public async Task<BaseResponse<List<DiscountCouponDTO>>> GetAllDiscountCouponsAsync(int page, int pageSize, string language)
        {
            try
            {
                var coupons = await _context.DiscountCoupons
                    .Where(c => c.IsDeleted != true)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var couponDtos = _mapper.Map<List<DiscountCouponDTO>>(coupons);
                return BaseResponse<List<DiscountCouponDTO>>.SuccessResponse(couponDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving discount coupons");
                var errorMessage = _localizationService.GetMessage("DiscountCouponsRetrievalError", "Errors", language);
                return BaseResponse<List<DiscountCouponDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        public async Task<BaseResponse<DiscountCouponDTO>> UpdateDiscountCouponAsync(string id, UpdateDiscountCouponRequestDTO request, string adminId, string language)
        {
            try
            {
                if (!long.TryParse(id, out long couponId) || !long.TryParse(adminId, out long adminIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidId", "Errors", language);
                    return BaseResponse<DiscountCouponDTO>.FailureResponse(invalidIdMessage, 400);
                }

                var coupon = await _context.DiscountCoupons
                    .FirstOrDefaultAsync(c => c.Id == couponId && c.IsDeleted != true);

                if (coupon == null)
                {
                    var notFoundMessage = _localizationService.GetMessage("CouponNotFound", "Errors", language);
                    return BaseResponse<DiscountCouponDTO>.FailureResponse(notFoundMessage, 404);
                }

                coupon.Description = request.Description;
                coupon.DiscountType = request.DiscountType.ToString();
                coupon.DiscountValue = (double)request.DiscountValue;
                coupon.MaxUses = request.MaxUses;
                coupon.EndDate = request.ExpiryDate;
                coupon.IsActive = request.IsActive;
                coupon.ModifiedByUserId = adminIdLong;
                coupon.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                var couponDto = _mapper.Map<DiscountCouponDTO>(coupon);
                return BaseResponse<DiscountCouponDTO>.SuccessResponse(couponDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating discount coupon {CouponId}", id);
                var errorMessage = _localizationService.GetMessage("DiscountCouponUpdateError", "Errors", language);
                return BaseResponse<DiscountCouponDTO>.FailureResponse(errorMessage, 500);
            }
        }

        public async Task<BaseResponse<SubscriptionReportDTO>> GetSubscriptionReportsAsync(DateTime? startDate, DateTime? endDate, string language)
        {
            try
            {
                var query = _context.UserSubscriptions.Where(s => s.IsDeleted != true);

                if (startDate.HasValue)
                    query = query.Where(s => s.CreateDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(s => s.CreateDate <= endDate.Value);

                var subscriptions = await query.ToListAsync();

                var report = new SubscriptionReportDTO
                {
                    TotalSubscriptions = subscriptions.Count,
                    ActiveSubscriptions = subscriptions.Count(s => s.Status == "Active"),
                    ExpiredSubscriptions = subscriptions.Count(s => s.Status == "Expired"),
                    CancelledSubscriptions = subscriptions.Count(s => s.Status == "Cancelled"),
                    TrialSubscriptions = subscriptions.Count(s => s.Status == "Trial")
                };

                return BaseResponse<SubscriptionReportDTO>.SuccessResponse(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating subscription reports");
                var errorMessage = _localizationService.GetMessage("SubscriptionReportsError", "Errors", language);
                return BaseResponse<SubscriptionReportDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على جميع خطط الاشتراك
        /// </summary>
        public async Task<BaseResponse<List<SubscriptionPlanDTO>>> GetAllPlansAsync(string language)
        {
            try
            {
                var plans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive && p.IsDeleted != true)
                    .ToListAsync();

                var planDtos = _mapper.Map<List<SubscriptionPlanDTO>>(plans);

                // Load features for each plan
                foreach (var plan in planDtos)
                {
                    var features = await _context.PlanFeatures
                        .Where(f => f.PlanId.ToString() == plan.Id && f.IsDeleted != true)
                        .Select(f => f.Feature)
                        .ToListAsync();

                    plan.Features = features;
                }

                return BaseResponse<List<SubscriptionPlanDTO>>.SuccessResponse(planDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans");
                var errorMessage = _localizationService.GetMessage("PlansRetrievalError", "Errors", language);
                return BaseResponse<List<SubscriptionPlanDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على خطة اشتراك بواسطة المعرف
        /// </summary>
        public async Task<BaseResponse<SubscriptionPlanDTO>> GetPlanByIdAsync(string planId, string language)
        {
            try
            {
                if (!long.TryParse(planId, out long id))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidPlanId", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(invalidIdMessage, 400);
                }

                var plan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive && p.IsDeleted != true);

                if (plan == null)
                {
                    var errorMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(errorMessage, 404);
                }

                var planDto = _mapper.Map<SubscriptionPlanDTO>(plan);

                // Load features
                planDto.Features = await _context.PlanFeatures
                    .Where(f => f.PlanId == id && f.IsDeleted != true)
                    .Select(f => f.Feature)
                    .ToListAsync();

                return BaseResponse<SubscriptionPlanDTO>.SuccessResponse(planDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan {planId}", planId);
                var errorMessage = _localizationService.GetMessage("PlanRetrievalError", "Errors", language);
                return BaseResponse<SubscriptionPlanDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على اشتراك المستخدم
        /// </summary>
        public async Task<BaseResponse<UserSubscriptionDTO>> GetUserSubscriptionAsync(string userId, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long id))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                var subscription = await _context.UserSubscriptions
                    .Where(s => s.UserId == id && s.Status != "Cancelled" && s.IsDeleted != true)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                if (subscription == null)
                {
                    var errorMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 404);
                }

                // Check if subscription has expired and update if necessary
                if (subscription.EndDate < DateTime.Now && subscription.Status != SubscriptionStatus.Expired.ToString())
                {
                    subscription.Status = SubscriptionStatus.Expired.ToString();
                    subscription.ModifiedDate = DateTime.Now;
                    subscription.ModifiedByUserId = id;
                    await _context.SaveChangesAsync();
                }

                var subscriptionDto = _mapper.Map<UserSubscriptionDTO>(subscription);
                return BaseResponse<UserSubscriptionDTO>.SuccessResponse(subscriptionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user subscription for user {userId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionRetrievalError", "Errors", language);
                return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إنشاء اشتراك جديد
        /// </summary>
        public async Task<BaseResponse<UserSubscriptionDTO>> CreateSubscription(string userId, CreateSubscriptionRequestDTO request, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                if (!long.TryParse(request.PlanId, out long planIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidPlanId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                // Check for existing active subscription
                var existingSubscription = await _context.UserSubscriptions
                    .Where(s => s.UserId == userIdLong &&
                              (s.Status == "Active" || s.Status == "Trial") &&
                              s.IsDeleted != true)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                if (existingSubscription != null && existingSubscription.EndDate > DateTime.Now)
                {
                    var errorMessage = _localizationService.GetMessage("ActiveSubscriptionExists", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 400);
                }

                // Verify plan exists
                var plan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.Id == planIdLong && p.IsActive && p.IsDeleted != true);

                if (plan == null)
                {
                    var errorMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 404);
                }

                // Calculate subscription price
                decimal price = request.PeriodType == SubscriptionPeriodType.Monthly
                    ? (decimal)plan.PriceMonthly
                    : (decimal)plan.PriceYearly;

                decimal discountAmount = 0;
                string? couponCode = null;

                // Validate coupon if provided
                if (!string.IsNullOrEmpty(request.CouponCode))
                {
                    BaseResponse<CouponValidationResponse> couponValidationRes = await ValidateCouponInternalAsync(request.CouponCode, request.PlanId);

                    if (!couponValidationRes.Success)
                    {
                        var errorMessage = _localizationService.GetMessage("InvalidCouponCode", "Errors", language);
                        return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 400);
                    }
                    var couponValidation = couponValidationRes.Data;
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

                // Calculate final amount
                decimal totalAmount = price - discountAmount;
                if (totalAmount < 0) totalAmount = 0;

                // Calculate subscription dates
                DateTime startDate = DateTime.Now;
                DateTime endDate = request.PeriodType == SubscriptionPeriodType.Monthly
                    ? startDate.AddMonths(1)
                    : startDate.AddYears(1);

                // Set subscription status based on plan type
                var status = plan.IsTrial ? "Trial" : "Active";

                // Adjust end date for trial plans
                if (plan.IsTrial && plan.TrialDays.HasValue)
                {
                    endDate = startDate.AddDays(plan.TrialDays.Value);
                }

                // Generate unique invoice number
                string invoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";

                // Create subscription
                var subscription = new UserSubscription
                {
                    UserId = userIdLong,
                    PlanId = planIdLong,
                    Status = status,
                    StartDate = startDate,
                    EndDate = endDate,
                    PeriodType = request.PeriodType == SubscriptionPeriodType.Monthly ? "Monthly" : "Yearly",
                    AutoRenew = request.AutoRenew,
                    LastRenewalDate = null,
                    LastInvoiceId = null,
                    CreatedByUserId = userIdLong,
                    CreateDate = DateTime.Now
                };

                _context.UserSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                // Create financial transaction
                var transaction = new Invoice
                {
                    PlanId = planIdLong,
                    UserId = userIdLong,
                    Type = TransactionType.NewSubscription.ToString(),
                    Amount = (double)price,
                    DiscountAmount = (double)discountAmount,
                    TotalAmount = (double)totalAmount,
                    CouponCode = couponCode,
                    InvoiceNumber = invoiceNumber,
                    Status = TransactionStatus.Completed.ToString(),
                    TransactionDate = DateTime.Now,
                    CreatedByUserId = userIdLong,
                    CreateDate = DateTime.Now
                };

                _context.Invoices.Add(transaction);

                // Update coupon usage if applicable
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = await _context.DiscountCoupons
                        .FirstOrDefaultAsync(c => c.Code == couponCode && c.IsDeleted != true);

                    if (coupon != null)
                    {
                        coupon.CurrentUses++;
                        coupon.ModifiedByUserId = userIdLong;
                        coupon.ModifiedDate = DateTime.Now;
                    }
                }

                // Set subscription's last invoice ID
                subscription.LastInvoiceId = transaction.Id;
                await _context.SaveChangesAsync();

                var subscriptionDto = _mapper.Map<UserSubscriptionDTO>(subscription);
                var successMessage = _localizationService.GetMessage("SubscriptionCreated", "Messages", language);
                return BaseResponse<UserSubscriptionDTO>.SuccessResponse(subscriptionDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription for user {userId}", userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionCreationError", "Errors", language);
                return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تجديد اشتراك
        /// </summary>
        public async Task<BaseResponse<UserSubscriptionDTO>> RenewSubscriptionAsync(string userId, RenewSubscriptionRequest request, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                if (!long.TryParse(request.SubscriptionId, out long subscriptionIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidSubscriptionId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                // Get the subscription
                var subscription = await _context.UserSubscriptions
                    .FirstOrDefaultAsync(s => s.Id == subscriptionIdLong && s.IsDeleted != true);

                if (subscription == null)
                {
                    var errorMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 404);
                }

                // Verify user owns the subscription
                if (subscription.UserId != userIdLong)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedOperation", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 403);
                }

                // Check subscription status
                if (subscription.Status == "Cancelled")
                {
                    var errorMessage = _localizationService.GetMessage("CannotRenewCancelledSubscription", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 400);
                }

                // Get the plan
                var plan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.Id == subscription.PlanId && p.IsActive && p.IsDeleted != true);

                if (plan == null)
                {
                    var errorMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 404);
                }

                // Calculate renewal price
                bool isMonthly = subscription.PeriodType == "Monthly";
                decimal price = isMonthly ? (decimal)plan.PriceMonthly : (decimal)plan.PriceYearly;
                decimal discountAmount = 0;
                string? couponCode = null;

                // Validate coupon if provided
                if (!string.IsNullOrEmpty(request.CouponCode))
                {
                    var couponValidationRep = await ValidateCouponInternalAsync(request.CouponCode, subscription.PlanId.ToString());

                    if (!couponValidationRep.Success)
                    {
                        var errorMessage = _localizationService.GetMessage("InvalidCouponCode", "Errors", language);
                        return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 400);
                    }
                    var couponValidation = couponValidationRep.Data;
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

                // Calculate final amount
                decimal totalAmount = price - discountAmount;
                if (totalAmount < 0) totalAmount = 0;

                // Calculate renewal dates
                DateTime renewalStartDate = subscription.EndDate.Value > DateTime.Now
                    ? subscription.EndDate.Value // Use current end date if subscription is still active
                    : DateTime.Now; // Use now if subscription has expired

                DateTime renewalEndDate = isMonthly
                    ? renewalStartDate.AddMonths(1)
                    : renewalStartDate.AddYears(1);

                // Generate unique invoice number
                string invoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";

                // Update subscription
                subscription.StartDate = renewalStartDate;
                subscription.EndDate = renewalEndDate;
                subscription.Status = "Active";
                subscription.LastRenewalDate = DateTime.Now;
                subscription.ModifiedByUserId = userIdLong;
                subscription.ModifiedDate = DateTime.Now;

                // Create financial transaction
                var transaction = new Invoice
                {
                    PlanId = subscription.PlanId,
                    UserId = userIdLong,
                    Type = TransactionType.Renewal.ToString(),
                    Amount = (double)price,
                    DiscountAmount = (double)discountAmount,
                    TotalAmount = (double)totalAmount,
                    CouponCode = couponCode,
                    InvoiceNumber = invoiceNumber,
                    Status = TransactionStatus.Completed.ToString(),
                    TransactionDate = DateTime.Now,
                    CreatedByUserId = userIdLong,
                    CreateDate = DateTime.Now
                };

                _context.Invoices.Add(transaction);
                await _context.SaveChangesAsync();

                // Update coupon usage if applicable
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = await _context.DiscountCoupons
                        .FirstOrDefaultAsync(c => c.Code == couponCode && c.IsDeleted != true);

                    if (coupon != null)
                    {
                        coupon.CurrentUses++;
                        coupon.ModifiedByUserId = userIdLong;
                        coupon.ModifiedDate = DateTime.Now;
                    }
                }

                // Set subscription's last invoice ID
                subscription.LastInvoiceId = transaction.Id;
                await _context.SaveChangesAsync();

                var subscriptionDto = _mapper.Map<UserSubscriptionDTO>(subscription);
                var successMessage = _localizationService.GetMessage("SubscriptionRenewed", "Messages", language);
                return BaseResponse<UserSubscriptionDTO>.SuccessResponse(subscriptionDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing subscription {subscriptionId} for user {userId}", request.SubscriptionId, userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionRenewalError", "Errors", language);
                return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إيقاف التجديد التلقائي
        /// </summary>
        public async Task<BaseResponse<UserSubscriptionDTO>> CancelAutoRenewalAsync(string userId, string subscriptionId, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                if (!long.TryParse(subscriptionId, out long subscriptionIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidSubscriptionId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                // Get the subscription
                var subscription = await _context.UserSubscriptions
                    .FirstOrDefaultAsync(s => s.Id == subscriptionIdLong && s.IsDeleted != true);

                if (subscription == null)
                {
                    var errorMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 404);
                }

                // Verify user owns the subscription
                if (subscription.UserId != userIdLong)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedOperation", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 403);
                }

                // Update auto-renewal status
                subscription.AutoRenew = false;
                subscription.ModifiedByUserId = userIdLong;
                subscription.ModifiedDate = DateTime.Now;
                subscription.Notes = (subscription.Notes ?? "") + $"\nAuto-renewal cancelled on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                await _context.SaveChangesAsync();

                var subscriptionDto = _mapper.Map<UserSubscriptionDTO>(subscription);
                var successMessage = _localizationService.GetMessage("AutoRenewalCancelled", "Messages", language);
                return BaseResponse<UserSubscriptionDTO>.SuccessResponse(subscriptionDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling auto-renewal for subscription {subscriptionId} for user {userId}", subscriptionId, userId);
                var errorMessage = _localizationService.GetMessage("AutoRenewalCancellationError", "Errors", language);
                return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إلغاء اشتراك
        /// </summary>
        public async Task<BaseResponse<UserSubscriptionDTO>> CancelSubscriptionAsync(string userId, string subscriptionId, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                if (!long.TryParse(subscriptionId, out long subscriptionIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidSubscriptionId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                // Get the subscription
                var subscription = await _context.UserSubscriptions
                    .FirstOrDefaultAsync(s => s.Id == subscriptionIdLong && s.IsDeleted != true);

                if (subscription == null)
                {
                    var errorMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 404);
                }

                // Verify user owns the subscription
                if (subscription.UserId != userIdLong)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedOperation", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 403);
                }

                // Update subscription status
                subscription.Status = "Cancelled";
                subscription.AutoRenew = false;
                subscription.ModifiedByUserId = userIdLong;
                subscription.ModifiedDate = DateTime.Now;
                subscription.Notes = (subscription.Notes ?? "") + $"\nSubscription cancelled on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                await _context.SaveChangesAsync();

                var subscriptionDto = _mapper.Map<UserSubscriptionDTO>(subscription);
                var successMessage = _localizationService.GetMessage("SubscriptionCancelled", "Messages", language);
                return BaseResponse<UserSubscriptionDTO>.SuccessResponse(subscriptionDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription {subscriptionId} for user {userId}", subscriptionId, userId);
                var errorMessage = _localizationService.GetMessage("SubscriptionCancellationError", "Errors", language);
                return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 500);
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

                if (!validation.Success)
                {
                    return BaseResponse<CouponValidationResponse>.FailureResponse("Invalid coupon", 400, validation.Errors);
                }
                if (!validation.Data.IsValid)
                {
                    return BaseResponse<CouponValidationResponse>.FailureResponse("Coupon is not valid", 400);
                }

                return BaseResponse<CouponValidationResponse>.SuccessResponse(validation.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating coupon {couponCode}", request.CouponCode);
                var errorMessage = _localizationService.GetMessage("CouponValidationError", "Errors", language);
                return BaseResponse<CouponValidationResponse>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على معاملات المستخدم
        /// </summary>
        public async Task<BaseResponse<List<FinancialTransactionDTO>>> GetUserTransactionsAsync(string userId, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<List<FinancialTransactionDTO>>.FailureResponse(invalidIdMessage, 400);
                }

                var invoices = await _context.Invoices
                    .Where(i => i.UserId == userIdLong && i.IsDeleted != true)
                    .OrderByDescending(i => i.TransactionDate)
                    .ToListAsync();

                // Map to FinancialTransactionDTO DTOs
                var transactions = new List<FinancialTransactionDTO>();
                foreach (var invoice in invoices)
                {
                    var transaction = new FinancialTransactionDTO
                    {
                        Id = invoice.Id.ToString(),
                        UserId = invoice.UserId.ToString(),
                        PlanId = invoice.PlanId.ToString(),
                        Type = Enum.Parse<TransactionType>(invoice.Type),
                        Amount = (decimal)invoice.Amount,
                        DiscountAmount = (decimal)invoice.DiscountAmount,
                        TotalAmount = (decimal)invoice.TotalAmount,
                        CouponCode = invoice.CouponCode,
                        InvoiceNumber = invoice.InvoiceNumber,
                        Status = Enum.Parse<TransactionStatus>(invoice.Status),
                        PaymentGatewayTransactionId = invoice.PaymentGatewayTransactionId,
                        PaymentMethod = invoice.PaymentMethod,
                        TransactionDate = invoice.TransactionDate,
                        Notes = invoice.Notes
                    };

                    transactions.Add(transaction);
                }

                return BaseResponse<List<FinancialTransactionDTO>>.SuccessResponse(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user transactions for user {userId}", userId);
                var errorMessage = _localizationService.GetMessage("TransactionsRetrievalError", "Errors", language);
                return BaseResponse<List<FinancialTransactionDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        #region Helper Methods

        /// <summary>
        /// التحقق من صلاحية كوبون داخليًا
        /// </summary>
        private async Task<BaseResponse<CouponValidationResponse>> ValidateCouponInternalAsync(string couponCode, string planId)
        {
            var response = new CouponValidationResponse { IsValid = false };

            if (string.IsNullOrEmpty(couponCode))
            {
                return BaseResponse<CouponValidationResponse>.FailureResponse("Coupon code is required", 400);
            }
            if (!long.TryParse(planId, out long planIdLong))
            {
                return BaseResponse<CouponValidationResponse>.FailureResponse("Invalid plan ID", 400);
            }

            // Find the coupon
            var coupon = await _context.DiscountCoupons
                .FirstOrDefaultAsync(c => c.Code == couponCode && c.IsDeleted != true);

            if (coupon == null)
            {
                return BaseResponse<CouponValidationResponse>.FailureResponse("Coupon not found", 404);
            }

            // Check if coupon is active
            if (!coupon.IsActive)
            {
                return BaseResponse<CouponValidationResponse>.FailureResponse("Coupon is not active", 400);
            }

            // Check start date
            if (coupon.StartDate.HasValue && coupon.StartDate.Value > DateTime.Now)
            {
                return BaseResponse<CouponValidationResponse>.FailureResponse("Coupon is not yet valid", 400);
            }

            // Check end date
            if (coupon.EndDate.HasValue && coupon.EndDate.Value < DateTime.Now)
            {
                return BaseResponse<CouponValidationResponse>.FailureResponse("Coupon has expired", 400);
            }

            // Check usage limit
            if (coupon.MaxUses.HasValue && coupon.CurrentUses >= coupon.MaxUses.Value)
            {
                return BaseResponse<CouponValidationResponse>.FailureResponse("Coupon usage limit has been reached", 400);
            }

            // Check if coupon applies to this plan
            var couponPlan = await _context.CouponPlans
                .AnyAsync(cp => cp.CouponId == coupon.Id && cp.PlanId == planIdLong && cp.IsDeleted != true);

            // If there are coupon-plan relationships but this plan isn't included
            var hasAnyPlans = await _context.CouponPlans
                .AnyAsync(cp => cp.CouponId == coupon.Id && cp.IsDeleted != true);

            if (hasAnyPlans && !couponPlan)
            {
                return BaseResponse<CouponValidationResponse>.FailureResponse("Coupon is not applicable for this plan", 400);
            }

            // Coupon is valid
            response.IsValid = true;
            response.DiscountType = Enum.Parse<DiscountType>(coupon.DiscountType);
            response.DiscountValue = (decimal)coupon.DiscountValue;

            return BaseResponse<CouponValidationResponse>.SuccessResponse(response);
            #endregion
        }

    }
}
