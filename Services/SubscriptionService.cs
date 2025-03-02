using API.Services;
using AutoMapper;
using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;

namespace Services
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// الحصول على جميع خطط الاشتراك
        /// </summary>
        Task<BaseResponse<List<SubscriptionPlanDTO>>> GetAllPlansAsync(string language);

        /// <summary>
        /// الحصول على خطة اشتراك بواسطة المعرف
        /// </summary>
        Task<BaseResponse<SubscriptionPlanDTO>> GetPlanByIdAsync(string planId, string language);

        /// <summary>
        /// الحصول على اشتراك المستخدم
        /// </summary>
        Task<BaseResponse<UserSubscriptionDTO>> GetUserSubscriptionAsync(string userId, string language);

        /// <summary>
        /// إنشاء اشتراك جديد
        /// </summary>
        Task<BaseResponse<UserSubscriptionDTO>> CreateSubscriptionAsync(string userId, CreateSubscriptionRequest request, string language);

        /// <summary>
        /// تجديد اشتراك
        /// </summary>
        Task<BaseResponse<UserSubscriptionDTO>> RenewSubscriptionAsync(string userId, RenewSubscriptionRequest request, string language);

        /// <summary>
        /// إيقاف التجديد التلقائي
        /// </summary>
        Task<BaseResponse<UserSubscriptionDTO>> CancelAutoRenewalAsync(string userId, string subscriptionId, string language);

        /// <summary>
        /// إلغاء اشتراك
        /// </summary>
        Task<BaseResponse<UserSubscriptionDTO>> CancelSubscriptionAsync(string userId, string subscriptionId, string language);

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
                if (subscription.EndDate < DateTime.Now && subscription.Status != "Expired")
                {
                    subscription.Status = "Expired";
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
        public async Task<BaseResponse<UserSubscriptionDTO>> CreateSubscriptionAsync(string userId, CreateSubscriptionRequest request, string language)
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
                    Type = "NewSubscription",
                    Amount = (double)price,
                    DiscountAmount = (double)discountAmount,
                    TotalAmount = (double)totalAmount,
                    CouponCode = couponCode,
                    InvoiceNumber = invoiceNumber,
                    Status = "Completed",
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
                    var couponValidation = await ValidateCouponInternalAsync(request.CouponCode, subscription.PlanId.ToString());
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
                    Type = "Renewal",
                    Amount = (double)price,
                    DiscountAmount = (double)discountAmount,
                    TotalAmount = (double)totalAmount,
                    CouponCode = couponCode,
                    InvoiceNumber = invoiceNumber,
                    Status = "Completed",
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

                if (!validation.IsValid)
                {
                    return BaseResponse<CouponValidationResponse>.FailureResponse(
                        validation.ErrorMessage ?? "Invalid coupon",
                        400,
                        new List<string> { validation.ErrorMessage ?? "Invalid coupon" });
                }

                return BaseResponse<CouponValidationResponse>.SuccessResponse(validation);
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
        public async Task<BaseResponse<List<FinancialTransaction>>> GetUserTransactionsAsync(string userId, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<List<FinancialTransaction>>.FailureResponse(invalidIdMessage, 400);
                }

                var invoices = await _context.Invoices
                    .Where(i => i.UserId == userIdLong && i.IsDeleted != true)
                    .OrderByDescending(i => i.TransactionDate)
                    .ToListAsync();

                // Map to FinancialTransaction DTOs
                var transactions = new List<FinancialTransaction>();
                foreach (var invoice in invoices)
                {
                    var transaction = new FinancialTransaction
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

                return BaseResponse<List<FinancialTransaction>>.SuccessResponse(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user transactions for user {userId}", userId);
                var errorMessage = _localizationService.GetMessage("TransactionsRetrievalError", "Errors", language);
                return BaseResponse<List<FinancialTransaction>>.FailureResponse(errorMessage, 500);
            }
        }

        #region Helper Methods

        /// <summary>
        /// التحقق من صلاحية كوبون داخليًا
        /// </summary>
        private async Task<CouponValidationResponse> ValidateCouponInternalAsync(string couponCode, string planId)
        {
            var response = new CouponValidationResponse { IsValid = false };

            if (string.IsNullOrEmpty(couponCode))
            {
                response.ErrorMessage = "Coupon code is required";
                return response;
            }
            if (!long.TryParse(planId, out long planIdLong))
            {
                response.ErrorMessage = "Invalid plan ID";
                return response;
            }

            // Find the coupon
            var coupon = await _context.DiscountCoupons
                .FirstOrDefaultAsync(c => c.Code == couponCode && c.IsDeleted != true);

            if (coupon == null)
            {
                response.ErrorMessage = "Coupon not found";
                return response;
            }

            // Check if coupon is active
            if (!coupon.IsActive)
            {
                response.ErrorMessage = "Coupon is not active";
                return response;
            }

            // Check start date
            if (coupon.StartDate.HasValue && coupon.StartDate.Value > DateTime.Now)
            {
                response.ErrorMessage = "Coupon is not yet valid";
                return response;
            }

            // Check end date
            if (coupon.EndDate.HasValue && coupon.EndDate.Value < DateTime.Now)
            {
                response.ErrorMessage = "Coupon has expired";
                return response;
            }

            // Check usage limit
            if (coupon.MaxUses.HasValue && coupon.CurrentUses >= coupon.MaxUses.Value)
            {
                response.ErrorMessage = "Coupon usage limit has been reached";
                return response;
            }

            // Check if coupon applies to this plan
            var couponPlan = await _context.CouponPlans
                .AnyAsync(cp => cp.CouponId == coupon.Id && cp.PlanId == planIdLong && cp.IsDeleted != true);

            // If there are coupon-plan relationships but this plan isn't included
            var hasAnyPlans = await _context.CouponPlans
                .AnyAsync(cp => cp.CouponId == coupon.Id && cp.IsDeleted != true);

            if (hasAnyPlans && !couponPlan)
            {
                response.ErrorMessage = "Coupon is not applicable for this plan";
                return response;
            }

            // Coupon is valid
            response.IsValid = true;
            response.DiscountType = (DiscountType)coupon.DiscountType;
            response.DiscountValue = (decimal)coupon.DiscountValue;

            return response;
            #endregion
        }
    }
}
