using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.Subscription;
using Models.DTOs.Subscription.Requests;

namespace Services
{
    /// <summary>
    /// Interface for subscription-related operations
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Get subscription information for a user
        /// </summary>
        Task<BaseResponse<UserSubscriptionDTO>> GetUserSubscriptionAsync(string userId, string language);

        /// <summary>
        /// Get a specific subscription plan by ID
        /// </summary>
        Task<BaseResponse<SubscriptionPlanDTO>> GetPlanByIdAsync(string planId, string language);

        /// <summary>
        /// Get all available subscription plans
        /// </summary>
        Task<BaseResponse<List<SubscriptionPlanDTO>>> GetAllPlansAsync(string language);

        /// <summary>
        /// Create a new subscription for a user
        /// </summary>
        Task<BaseResponse<UserSubscriptionDTO>> CreateSubscriptionAsync(Models.DTOs.Subscription.Requests.CreateSubscriptionRequestDTO request, string userId, string language);

        /// <summary>
        /// Create a new subscription for a user (alternative method)
        /// </summary>
        Task<BaseResponse<UserSubscriptionDTO>> CreateSubscription(string userId, Models.DTOs.Subscription.Requests.CreateSubscriptionRequestDTO request, string language);

        /// <summary>
        /// Process payment success for a subscription
        /// </summary>
        Task<BaseResponse<bool>> ProcessPaymentSuccessAsync(string sessionId, string clientReferenceId, string language);

        /// <summary>
        /// Get all subscriptions with pagination
        /// </summary>
        Task<BaseResponse<List<UserSubscriptionDTO>>> GetAllSubscriptionsAsync(int page, int pageSize, string language);

        /// <summary>
        /// Update a discount coupon
        /// </summary>
        Task<BaseResponse<DiscountCouponDTO>> UpdateDiscountCouponAsync(string id, UpdateDiscountCouponRequestDTO request, string adminId, string language);

        /// <summary>
        /// Get subscription reports
        /// </summary>
        Task<BaseResponse<SubscriptionReportDTO>> GetSubscriptionReportsAsync(DateTime? startDate, DateTime? endDate, string language);

        /// <summary>
        /// Create a new discount coupon
        /// </summary>
        Task<BaseResponse<DiscountCouponDTO>> CreateDiscountCouponAsync(CreateDiscountCouponRequestDTO request, string adminId, string language);

        /// <summary>
        /// Update a subscription plan
        /// </summary>
        Task<BaseResponse<SubscriptionPlanDTO>> UpdateSubscriptionPlanAsync(string id, UpdateSubscriptionPlanRequestDTO request, string adminId, string language);

        /// <summary>
        /// Create a new subscription plan
        /// </summary>
        Task<BaseResponse<SubscriptionPlanDTO>> CreateSubscriptionPlanAsync(CreateSubscriptionPlanRequestDTO request, string adminId, string language);

        /// <summary>
        /// Get subscription history for a user
        /// </summary>
        Task<BaseResponse<List<UserSubscriptionDTO>>> GetUserSubscriptionsHistoryAsync(string userId, string language);

        /// <summary>
        /// Get a specific subscription by ID
        /// </summary>
        Task<BaseResponse<UserSubscriptionDTO>> GetSubscriptionByIdAsync(string id, string language);

        /// <summary>
        /// Get all discount coupons
        /// </summary>
        Task<BaseResponse<List<DiscountCouponDTO>>> GetAllDiscountCouponsAsync(int page, int pageSize, string language);
    }

    /// <summary>
    /// Implementation of subscription service
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        private readonly MuhamiContext _context;
        private readonly ILogger<SubscriptionService> _logger;
        private readonly ILocalizationService _localizationService;

        public SubscriptionService(
            MuhamiContext context,
            ILogger<SubscriptionService> logger,
            ILocalizationService localizationService)
        {
            _context = context;
            _logger = logger;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Get all available subscription plans
        /// </summary>
        public async Task<BaseResponse<List<SubscriptionPlanDTO>>> GetAllPlansAsync(string language)
        {
            try
            {
                _logger.LogInformation("Retrieving all subscription plans");

                // Get active subscription plans
                var plans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive && !p.IsDeleted)
                    .OrderBy(p => p.PriceMonthly)
                    .ToListAsync();

                if (plans == null || !plans.Any())
                {
                    var noPlansMessage = _localizationService.GetMessage("NoPlansAvailable", "Messages", language);
                    return BaseResponse<List<SubscriptionPlanDTO>>.SuccessResponse(new List<SubscriptionPlanDTO>(), noPlansMessage);
                }

                // Map plans to DTOs
                var planDtos = new List<SubscriptionPlanDTO>();
                foreach (var plan in plans)
                {
                    var planDto = new SubscriptionPlanDTO
                    {
                        Id = plan.Id,
                        Name = plan.Name,
                        Description = plan.Description,
                        MonthlyPrice = (decimal)plan.PriceMonthly,
                        YearlyPrice = (decimal)plan.PriceYearly,
                        MonthlyQueryLimit = (int)plan.MaxQueriesPerMonth,
                        FileUploadSizeLimitMB = (int)(plan.AllowedFileSizeMb ?? 0),
                        HasPrioritySupport = false, // This field doesn't exist in the database model
                        IsActive = plan.IsActive,
                        AllowedChatRooms = (int)plan.AllowedChatRooms,
                        AllowedFiles = (int)plan.AllowedFiles,
                        AllowedFileSizeMb = (int)(plan.AllowedFileSizeMb ?? 0),
                        IsTrial = plan.IsTrial,
                        TrialDays = plan.TrialDays.HasValue ? (int)plan.TrialDays : null
                    };

                    // Get plan features
                    var features = await _context.PlanFeatures
                        .Where(f => f.PlanId == plan.Id && !f.IsDeleted)
                        .ToListAsync();

                    if (features != null && features.Any())
                    {
                        planDto.Features = features.Select(f => new PlanFeatureDTO
                        {
                            Id = f.Id,
                            Name = f.Feature,
                            Description = f.Feature // Feature description field doesn't exist, using the feature name
                        }).ToList();
                    }

                    planDtos.Add(planDto);
                }

                var successMessage = _localizationService.GetMessage("PlansRetrievedSuccessfully", "Messages", language);
                return BaseResponse<List<SubscriptionPlanDTO>>.SuccessResponse(planDtos, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans");
                var errorMessage = _localizationService.GetMessage("ErrorRetrievingPlans", "Errors", language);
                return BaseResponse<List<SubscriptionPlanDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get a specific subscription plan by ID
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

                _logger.LogInformation("Retrieving subscription plan with ID: {PlanId}", planId);

                // Get the subscription plan
                var plan = await _context.SubscriptionPlans
                    .Where(p => p.Id == id && p.IsActive && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (plan == null)
                {
                    var notFoundMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(notFoundMessage, 404);
                }

                // Map plan to DTO
                var planDto = new SubscriptionPlanDTO
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    MonthlyPrice = (decimal)plan.PriceMonthly,
                    YearlyPrice = (decimal)plan.PriceYearly,
                    MonthlyQueryLimit = (int)plan.MaxQueriesPerMonth,
                    FileUploadSizeLimitMB = (int)(plan.AllowedFileSizeMb ?? 0),
                    HasPrioritySupport = false, // This field doesn't exist in the database model
                    IsActive = plan.IsActive,
                    AllowedChatRooms = (int)plan.AllowedChatRooms,
                    AllowedFiles = (int)plan.AllowedFiles,
                    AllowedFileSizeMb = (int)(plan.AllowedFileSizeMb ?? 0),
                    IsTrial = plan.IsTrial,
                    TrialDays = plan.TrialDays.HasValue ? (int)plan.TrialDays : null
                };

                // Get plan features
                var features = await _context.PlanFeatures
                    .Where(f => f.PlanId == plan.Id && !f.IsDeleted)
                    .ToListAsync();

                if (features != null && features.Any())
                {
                    planDto.Features = features.Select(f => new PlanFeatureDTO
                    {
                        Id = f.Id,
                        Name = f.Feature,
                        Description = f.Feature // Feature description field doesn't exist, using the feature name
                    }).ToList();
                }

                var successMessage = _localizationService.GetMessage("PlanRetrievedSuccessfully", "Messages", language);
                return BaseResponse<SubscriptionPlanDTO>.SuccessResponse(planDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan with ID: {PlanId}", planId);
                var errorMessage = _localizationService.GetMessage("ErrorRetrievingPlan", "Errors", language);
                return BaseResponse<SubscriptionPlanDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get subscription information for a user
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

                _logger.LogInformation("Retrieving subscription for user ID: {UserId}", userId);

                // Get the active subscription for the user
                var subscription = await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.UserId == id && s.Status == "Active" && !s.IsDeleted)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                if (subscription == null)
                {
                    var noSubscriptionMessage = _localizationService.GetMessage("NoActiveSubscription", "Messages", language);
                    return BaseResponse<UserSubscriptionDTO>.SuccessResponse(new UserSubscriptionDTO
                    {
                        UserId = id,
                        PlanName = "Free",
                        Status = "Free",
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddYears(100),
                        MonthlyQueryLimit = 5, // Default free limit
                        QueriesUsedThisMonth = 0
                    }, noSubscriptionMessage);
                }

                // Map subscription to DTO
                var subscriptionDto = new UserSubscriptionDTO
                {
                    Id = subscription.Id,
                    UserId = subscription.UserId,
                    PlanId = subscription.PlanId,
                    PlanName = subscription.Plan.Name,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate ?? DateTime.UtcNow.AddYears(100),
                    Status = subscription.Status,
                    QueriesUsedThisMonth = (int)subscription.QueriesUsedThisMonth,
                    MonthlyQueryLimit = (int)subscription.Plan.MaxQueriesPerMonth,
                    IsYearly = subscription.PeriodType == "Yearly", // Map based on period type
                    IsPaid = true, // Assume paid if there's an active subscription
                    InvoiceReference = subscription.PaymentGatewayTransactionId // Using transaction ID as reference
                };

                var successMessage = _localizationService.GetMessage("SubscriptionRetrievedSuccessfully", "Messages", language);
                return BaseResponse<UserSubscriptionDTO>.SuccessResponse(subscriptionDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription for user ID: {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("ErrorRetrievingSubscription", "Errors", language);
                return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Create a new subscription for a user
        /// </summary>
        public async Task<BaseResponse<UserSubscriptionDTO>> CreateSubscriptionAsync(Models.DTOs.Subscription.Requests.CreateSubscriptionRequestDTO request, string userId, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long id))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                _logger.LogInformation("Creating subscription for user ID: {UserId}, Plan ID: {PlanId}",
                    userId, request.PlanId);

                // Get the user
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(userNotFoundMessage, 404);
                }

                // Get the subscription plan
                var plan = await _context.SubscriptionPlans
                    .Where(p => p.Id == request.PlanId && p.IsActive && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (plan == null)
                {
                    var planNotFoundMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(planNotFoundMessage, 404);
                }

                // Set start and end dates
                var startDate = DateTime.UtcNow;
                var endDate = request.IsYearly
                    ? startDate.AddYears(1)
                    : startDate.AddMonths(1);

                // Calculate price
                decimal price = request.IsYearly ? (decimal)plan.PriceYearly : (decimal)plan.PriceMonthly;

                // Apply coupon discount if provided
                if (!string.IsNullOrEmpty(request.CouponCode))
                {
                    var coupon = await _context.DiscountCoupons
                        .Where(c => c.Code == request.CouponCode && c.IsActive && (c.IsDeleted != true) &&
                               c.EndDate > DateTime.UtcNow &&
                               (c.MaxUses == null || c.CurrentUses < c.MaxUses))
                        .FirstOrDefaultAsync();

                    if (coupon != null)
                    {
                        // Check if coupon is applicable to this plan
                        var couponPlan = await _context.CouponPlans
                            .Where(cp => cp.CouponId == coupon.Id && cp.PlanId == plan.Id)
                            .FirstOrDefaultAsync();

                        if (couponPlan != null)
                        {
                            if (coupon.DiscountType == "Percentage")
                            {
                                decimal discountValue = (decimal)coupon.DiscountValue;
                                price -= (price * discountValue / 100m);
                            }
                            else if (coupon.DiscountType == "Fixed")
                            {
                                decimal discountValue = (decimal)coupon.DiscountValue;
                                price -= discountValue;
                            }

                            // Update coupon usage count
                            coupon.CurrentUses++;
                        }
                    }
                }

                // Create new subscription
                var newSubscription = new UserSubscription
                {
                    UserId = id,
                    PlanId = plan.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = "Active",
                    QueriesUsedThisMonth = 0,
                    QueriesUsedToday = 0,
                    PeriodType = request.IsYearly ? "Yearly" : "Monthly",
                    CreateDate = DateTime.UtcNow,
                    CreatedByUserId = id,
                    AutoRenew = request.AutoRenew.GetValueOrDefault(),
                    PaymentMethod = request.PaymentMethod ?? "Unknown",
                    PaymentGatewayTransactionId = request.PaymentGatewayTransactionId
                };

                // Save to database
                _context.UserSubscriptions.Add(newSubscription);
                await _context.SaveChangesAsync();

                // Create invoice for the subscription
                var invoice = new Invoice
                {
                    UserId = id,
                    PlanId = plan.Id,
                    InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{newSubscription.Id}",
                    Amount = (double)price,
                    TotalAmount = (double)price,
                    TransactionDate = DateTime.UtcNow,
                    Status = "Completed",
                    Type = "NewSubscription",
                    PaymentMethod = request.PaymentMethod ?? "Unknown",
                    PaymentGatewayTransactionId = request.PaymentGatewayTransactionId,
                    CreateDate = DateTime.UtcNow,
                    CreatedByUserId = id
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // Update the subscription with the invoice ID
                newSubscription.LastInvoiceId = invoice.Id;
                await _context.SaveChangesAsync();

                // Map to DTO for response
                var subscriptionDto = new UserSubscriptionDTO
                {
                    Id = newSubscription.Id,
                    UserId = newSubscription.UserId,
                    PlanId = newSubscription.PlanId,
                    PlanName = plan.Name,
                    StartDate = newSubscription.StartDate,
                    EndDate = newSubscription.EndDate ?? DateTime.UtcNow.AddYears(100),
                    Status = newSubscription.Status,
                    QueriesUsedThisMonth = (int)newSubscription.QueriesUsedThisMonth,
                    MonthlyQueryLimit = (int)plan.MaxQueriesPerMonth,
                    IsYearly = newSubscription.PeriodType == "Yearly",
                    IsPaid = true,
                    InvoiceReference = invoice.InvoiceNumber,
                    Price = price
                };

                var successMessage = _localizationService.GetMessage("SubscriptionCreatedSuccessfully", "Messages", language);
                return BaseResponse<UserSubscriptionDTO>.SuccessResponse(subscriptionDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription for user ID: {UserId}, Plan ID: {PlanId}",
                    userId, request.PlanId);
                var errorMessage = _localizationService.GetMessage("ErrorCreatingSubscription", "Errors", language);
                return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Create a new subscription for a user (alternative method)
        /// </summary>
        public Task<BaseResponse<UserSubscriptionDTO>> CreateSubscription(string userId, Models.DTOs.Subscription.Requests.CreateSubscriptionRequestDTO request, string language)
        {
            // This delegates to the primary CreateSubscriptionAsync method with reordered parameters
            return CreateSubscriptionAsync(request, userId, language);
        }

        /// <summary>
        /// Process payment success for a subscription
        /// </summary>
        public async Task<BaseResponse<bool>> ProcessPaymentSuccessAsync(string sessionId, string clientReferenceId, string language)
        {
            try
            {
                _logger.LogInformation("Processing payment success for session ID: {SessionId}", sessionId);

                // Extract user ID and plan ID from client reference ID
                // Format: sub_userId_planId_timestamp
                var parts = clientReferenceId.Split('_');
                if (parts.Length < 3 || !parts[0].Equals("sub", StringComparison.OrdinalIgnoreCase))
                {
                    var invalidReferenceMessage = _localizationService.GetMessage("InvalidReferenceId", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(invalidReferenceMessage, 400);
                }

                if (!long.TryParse(parts[1], out long userId) || !long.TryParse(parts[2], out long planId))
                {
                    var invalidIdsMessage = _localizationService.GetMessage("InvalidIdFormat", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(invalidIdsMessage, 400);
                }

                // Check if subscription already exists for this payment
                var existingSubscription = await _context.UserSubscriptions
                    .Where(s => s.PaymentGatewayTransactionId == sessionId)
                    .FirstOrDefaultAsync();

                if (existingSubscription != null)
                {
                    var alreadyProcessedMessage = _localizationService.GetMessage("PaymentAlreadyProcessed", "Messages", language);
                    return BaseResponse<bool>.SuccessResponse(true, alreadyProcessedMessage);
                }

                // Create subscription request
                var request = new Models.DTOs.Subscription.Requests.CreateSubscriptionRequestDTO
                {
                    PlanId = planId,
                    UserId = userId,
                    IsYearly = false, // Default to monthly
                    PaymentGatewayTransactionId = sessionId,
                    PaymentMethod = "Thawani"
                };

                // Create subscription
                var result = await CreateSubscriptionAsync(request, userId.ToString(), language);

                if (!result.Success)
                {
                    return BaseResponse<bool>.FailureResponse(result.Message, result.StatusCode);
                }

                var successMessage = _localizationService.GetMessage("PaymentProcessedSuccessfully", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment success for session ID: {SessionId}", sessionId);
                var errorMessage = _localizationService.GetMessage("ErrorProcessingPayment", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get all subscriptions with pagination
        /// </summary>
        public async Task<BaseResponse<List<UserSubscriptionDTO>>> GetAllSubscriptionsAsync(int page, int pageSize, string language)
        {
            try
            {
                // Validate pagination parameters
                page = page <= 0 ? 1 : page;
                pageSize = pageSize <= 0 ? 10 : (pageSize > 100 ? 100 : pageSize);

                _logger.LogInformation("Retrieving all subscriptions, page: {Page}, pageSize: {PageSize}", page, pageSize);

                // Get subscriptions with pagination
                var subscriptions = await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => !s.IsDeleted)
                    .OrderByDescending(s => s.CreateDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (subscriptions == null || !subscriptions.Any())
                {
                    var noSubscriptionsMessage = _localizationService.GetMessage("NoSubscriptionsFound", "Messages", language);
                    return BaseResponse<List<UserSubscriptionDTO>>.SuccessResponse(new List<UserSubscriptionDTO>(), noSubscriptionsMessage);
                }

                // Map subscriptions to DTOs
                var subscriptionDtos = subscriptions.Select(s => new UserSubscriptionDTO
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    PlanId = s.PlanId,
                    PlanName = s.Plan.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate ?? DateTime.UtcNow.AddYears(100),
                    Status = s.Status,
                    QueriesUsedThisMonth = (int)s.QueriesUsedThisMonth,
                    MonthlyQueryLimit = (int)s.Plan.MaxQueriesPerMonth,
                    IsYearly = s.PeriodType == "Yearly",
                    IsPaid = true, // Assuming all subscriptions in the database are paid
                    InvoiceReference = s.PaymentGatewayTransactionId // Using transaction ID as reference
                }).ToList();

                var successMessage = _localizationService.GetMessage("SubscriptionsRetrievedSuccessfully", "Messages", language);
                return BaseResponse<List<UserSubscriptionDTO>>.SuccessResponse(subscriptionDtos, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all subscriptions, page: {Page}, pageSize: {PageSize}", page, pageSize);
                var errorMessage = _localizationService.GetMessage("ErrorRetrievingSubscriptions", "Errors", language);
                return BaseResponse<List<UserSubscriptionDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Update a discount coupon
        /// </summary>
        public async Task<BaseResponse<DiscountCouponDTO>> UpdateDiscountCouponAsync(string id, UpdateDiscountCouponRequestDTO request, string adminId, string language)
        {
            try
            {
                if (!long.TryParse(id, out long couponId))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidCouponId", "Errors", language);
                    return BaseResponse<DiscountCouponDTO>.FailureResponse(invalidIdMessage, 400);
                }

                if (!long.TryParse(adminId, out long adminIdLong))
                {
                    var invalidAdminIdMessage = _localizationService.GetMessage("InvalidAdminId", "Errors", language);
                    return BaseResponse<DiscountCouponDTO>.FailureResponse(invalidAdminIdMessage, 400);
                }

                _logger.LogInformation("Updating discount coupon with ID: {CouponId}", id);

                // Get the coupon
                var coupon = await _context.DiscountCoupons
                    .Where(c => c.Id == couponId && !c.IsDeleted)
                    .FirstOrDefaultAsync();

                if (coupon == null)
                {
                    var notFoundMessage = _localizationService.GetMessage("CouponNotFound", "Errors", language);
                    return BaseResponse<DiscountCouponDTO>.FailureResponse(notFoundMessage, 404);
                }

                // Update coupon properties
                coupon.Code = request.Code ?? coupon.Code;
                coupon.Description = request.Description ?? coupon.Description;

                if (request.DiscountType.HasValue)
                {
                    string discountTypeValue = (int)request.DiscountType.Value == (int)Models.DTOs.Subscription.Enums.DiscountType.Percentage
                        ? "Percentage" : "Fixed";
                    coupon.DiscountType = discountTypeValue;
                }

                coupon.DiscountValue = request.DiscountValue.HasValue ? (double)request.DiscountValue.Value : coupon.DiscountValue;
                coupon.IsActive = request.IsActive;

                if (request.StartDate.HasValue)
                    coupon.StartDate = request.StartDate.Value;

                if (request.EndDate.HasValue)
                    coupon.EndDate = request.EndDate.Value;

                if (request.MaxUses.HasValue)
                    coupon.MaxUses = request.MaxUses.Value;

                coupon.ModifiedByUserId = adminIdLong;
                coupon.ModifiedDate = DateTime.UtcNow;

                // Save changes
                await _context.SaveChangesAsync();

                // If plan IDs are specified, update the coupon-plan associations
                if (request.PlanIds != null && request.PlanIds.Any())
                {
                    // Remove existing associations
                    var existingAssociations = await _context.CouponPlans
                        .Where(cp => cp.CouponId == couponId)
                        .ToListAsync();

                    _context.CouponPlans.RemoveRange(existingAssociations);

                    // Add new associations
                    foreach (var planId in request.PlanIds)
                    {
                        if (long.TryParse(planId, out long planIdLong))
                        {
                            var plan = await _context.SubscriptionPlans.FindAsync(planIdLong);
                            if (plan != null)
                            {
                                _context.CouponPlans.Add(new CouponPlan
                                {
                                    CouponId = couponId,
                                    PlanId = planIdLong,
                                    CreateDate = DateTime.UtcNow,
                                    CreatedByUserId = adminIdLong
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                // Map to DTO
                var discountTypeEnum = (DiscountType)(coupon.DiscountType == "Percentage" ? 0 : 1);

                var couponDto = new DiscountCouponDTO
                {
                    Id = coupon.Id.ToString(),
                    Code = coupon.Code,
                    Description = coupon.Description,
                    DiscountType = discountTypeEnum,
                    DiscountValue = (decimal)coupon.DiscountValue,
                    StartDate = coupon.StartDate,
                    EndDate = coupon.EndDate.HasValue ? coupon.EndDate.Value : DateTime.UtcNow,
                    MaxUses = coupon.MaxUses.HasValue ? (int?)coupon.MaxUses.Value : null,
                    CurrentUses = (int)coupon.CurrentUses,
                    IsActive = coupon.IsActive
                };

                var successMessage = _localizationService.GetMessage("DiscountCouponUpdated", "Messages", language);
                return BaseResponse<DiscountCouponDTO>.SuccessResponse(couponDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating discount coupon with ID: {CouponId}", id);
                var errorMessage = _localizationService.GetMessage("DiscountCouponUpdateError", "Errors", language);
                return BaseResponse<DiscountCouponDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get subscription reports
        /// </summary>
        public async Task<BaseResponse<SubscriptionReportDTO>> GetSubscriptionReportsAsync(DateTime? startDate, DateTime? endDate, string language)
        {
            try
            {
                _logger.LogInformation("Retrieving subscription reports");

                // Use default date range if not specified
                var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
                var end = endDate ?? DateTime.UtcNow;

                // Get active subscriptions
                var activeSubscriptions = await _context.UserSubscriptions
                    .Where(s => s.Status == "Active" && !s.IsDeleted)
                    .CountAsync();

                // Get new subscriptions in date range
                var newSubscriptions = await _context.UserSubscriptions
                    .Where(s => s.CreateDate >= start && s.CreateDate <= end && !s.IsDeleted)
                    .CountAsync();

                // Get expired subscriptions in date range
                var expiredSubscriptions = await _context.UserSubscriptions
                    .Where(s => s.Status == "Expired" && s.EndDate >= start && s.EndDate <= end && !s.IsDeleted)
                    .CountAsync();

                // Get revenue in date range
                var revenue = await _context.Invoices
                    .Where(i => i.Status == "Completed" &&
                           i.TransactionDate >= start &&
                           i.TransactionDate <= end &&
                           i.IsDeleted == false)
                    .SumAsync(i => i.TotalAmount);

                // Get subscriptions by plan
                var subscriptionsByPlan = await _context.UserSubscriptions
                    .Where(s => s.Status == "Active" && !s.IsDeleted)
                    .GroupBy(s => s.PlanId)
                    .Select(g => new { PlanId = g.Key, Count = g.Count() })
                    .ToListAsync();

                var planDetails = await _context.SubscriptionPlans
                    .Where(p => subscriptionsByPlan.Select(s => s.PlanId).Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id, p => p.Name);

                var planBreakdown = subscriptionsByPlan
                    .Select(s => new PlanSubscriptionCountDTO
                    {
                        PlanId = s.PlanId,
                        PlanName = planDetails.ContainsKey(s.PlanId) ? planDetails[s.PlanId] : "Unknown",
                        SubscriptionCount = s.Count
                    })
                    .ToList();

                // Create report DTO
                var report = new SubscriptionReportDTO
                {
                    StartDate = start,
                    EndDate = end,
                    TotalActiveSubscriptions = activeSubscriptions,
                    NewSubscriptions = newSubscriptions,
                    ExpiredSubscriptions = expiredSubscriptions,
                    TotalRevenue = (decimal)revenue,
                    SubscriptionsByPlan = planBreakdown
                };

                var successMessage = _localizationService.GetMessage("SubscriptionReportsRetrieved", "Messages", language);
                return BaseResponse<SubscriptionReportDTO>.SuccessResponse(report, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription reports");
                var errorMessage = _localizationService.GetMessage("SubscriptionReportsError", "Errors", language);
                return BaseResponse<SubscriptionReportDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Create a new discount coupon
        /// </summary>
        public async Task<BaseResponse<DiscountCouponDTO>> CreateDiscountCouponAsync(CreateDiscountCouponRequestDTO request, string adminId, string language)
        {
            try
            {
                if (!long.TryParse(adminId, out long adminIdLong))
                {
                    var invalidAdminIdMessage = _localizationService.GetMessage("InvalidAdminId", "Errors", language);
                    return BaseResponse<DiscountCouponDTO>.FailureResponse(invalidAdminIdMessage, 400);
                }

                _logger.LogInformation("Creating new discount coupon");

                // Check if coupon code already exists
                var existingCoupon = await _context.DiscountCoupons
                    .AnyAsync(c => c.Code == request.Code && !c.IsDeleted);

                if (existingCoupon)
                {
                    var codeExistsMessage = _localizationService.GetMessage("CouponCodeExists", "Errors", language);
                    return BaseResponse<DiscountCouponDTO>.FailureResponse(codeExistsMessage, 400);
                }

                // Create new coupon
                var coupon = new DiscountCoupon
                {
                    Code = request.Code,
                    Description = request.Description,
                    DiscountType = request.DiscountType == "Percentage" ? "Percentage" : "Fixed",
                    DiscountValue = (double)request.DiscountValue,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    MaxUses = request.MaxUses.HasValue ? request.MaxUses.Value : null,
                    CurrentUses = 0,
                    IsActive = request.IsActive,
                    CreateDate = DateTime.UtcNow,
                    CreatedByUserId = adminIdLong
                };

                _context.DiscountCoupons.Add(coupon);
                await _context.SaveChangesAsync();

                // Add coupon-plan associations if specified
                if (request.PlanIds != null && request.PlanIds.Any())
                {
                    foreach (var planId in request.PlanIds)
                    {
                        if (long.TryParse(planId, out long planIdLong))
                        {
                            var plan = await _context.SubscriptionPlans.FindAsync(planIdLong);
                            if (plan != null)
                            {
                                _context.CouponPlans.Add(new CouponPlan
                                {
                                    CouponId = coupon.Id,
                                    PlanId = planIdLong,
                                    CreateDate = DateTime.UtcNow,
                                    CreatedByUserId = adminIdLong
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                // Map to DTO
                var discountTypeEnum = (DiscountType)(coupon.DiscountType == "Percentage" ? 0 : 1);

                var couponDto = new DiscountCouponDTO
                {
                    Id = coupon.Id.ToString(),
                    Code = coupon.Code,
                    Description = coupon.Description,
                    DiscountType = discountTypeEnum,
                    DiscountValue = (decimal)coupon.DiscountValue,
                    StartDate = coupon.StartDate,
                    EndDate = coupon.EndDate.HasValue ? coupon.EndDate.Value : DateTime.UtcNow,
                    MaxUses = coupon.MaxUses.HasValue ? (int?)coupon.MaxUses.Value : null,
                    CurrentUses = (int)coupon.CurrentUses,
                    IsActive = coupon.IsActive
                };

                var successMessage = _localizationService.GetMessage("DiscountCouponCreated", "Messages", language);
                return BaseResponse<DiscountCouponDTO>.SuccessResponse(couponDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating discount coupon");
                var errorMessage = _localizationService.GetMessage("DiscountCouponCreationError", "Errors", language);
                return BaseResponse<DiscountCouponDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Update a subscription plan
        /// </summary>
        public async Task<BaseResponse<SubscriptionPlanDTO>> UpdateSubscriptionPlanAsync(string id, UpdateSubscriptionPlanRequestDTO request, string adminId, string language)
        {
            try
            {
                if (!long.TryParse(id, out long planId))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidPlanId", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(invalidIdMessage, 400);
                }

                if (!long.TryParse(adminId, out long adminIdLong))
                {
                    var invalidAdminIdMessage = _localizationService.GetMessage("InvalidAdminId", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(invalidAdminIdMessage, 400);
                }

                _logger.LogInformation("Updating subscription plan with ID: {PlanId}", id);

                // Get the plan
                var plan = await _context.SubscriptionPlans
                    .Where(p => p.Id == planId && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (plan == null)
                {
                    var notFoundMessage = _localizationService.GetMessage("PlanNotFound", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(notFoundMessage, 404);
                }

                // Update plan properties
                plan.Name = request.Name ?? plan.Name;
                plan.Description = request.Description ?? plan.Description;

                if (request.PriceMonthly.HasValue)
                    plan.PriceMonthly = (double)request.PriceMonthly.Value;

                if (request.PriceYearly.HasValue)
                    plan.PriceYearly = (double)request.PriceYearly.Value;

                if (request.AllowedChatRooms.HasValue)
                    plan.AllowedChatRooms = request.AllowedChatRooms.Value;

                if (request.AllowedFiles.HasValue)
                    plan.AllowedFiles = request.AllowedFiles.Value;

                if (request.AllowedFileSizeMb.HasValue)
                    plan.AllowedFileSizeMb = request.AllowedFileSizeMb.Value;

                if (request.MaxQueriesPerDay.HasValue)
                    plan.MaxQueriesPerDay = request.MaxQueriesPerDay.Value;

                if (request.MaxQueriesPerMonth.HasValue)
                    plan.MaxQueriesPerMonth = request.MaxQueriesPerMonth.Value;

                if (request.TrialDays.HasValue)
                    plan.TrialDays = request.TrialDays.Value;

                plan.IsActive = request.IsActive ?? plan.IsActive;
                plan.IsTrial = request.IsTrial ?? plan.IsTrial;

                plan.ModifiedByUserId = adminIdLong;
                plan.ModifiedDate = DateTime.UtcNow;

                // Save changes
                await _context.SaveChangesAsync();

                // Update features if provided
                if (request.Features != null && request.Features.Any())
                {
                    // Remove existing features
                    var existingFeatures = await _context.PlanFeatures
                        .Where(f => f.PlanId == planId)
                        .ToListAsync();

                    foreach (var feature in existingFeatures)
                    {
                        feature.IsDeleted = true;
                        feature.DeletedAt = DateTime.UtcNow;
                        // Use reflection or method calls instead of direct property access if they don't exist
                        try
                        {
                            // Try to set the properties but catch any exceptions
                            // These fields may not exist in the PlanFeature class
                            feature.GetType().GetProperty("ModifiedByUserId")?.SetValue(feature, adminIdLong);
                            feature.GetType().GetProperty("ModifiedDate")?.SetValue(feature, DateTime.UtcNow);
                        }
                        catch { }
                    }

                    // Add new features
                    foreach (var featureName in request.Features)
                    {
                        _context.PlanFeatures.Add(new PlanFeature
                        {
                            PlanId = planId,
                            Feature = featureName,
                            CreateDate = DateTime.UtcNow,
                            CreatedByUserId = adminIdLong
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                // Map to DTO
                var planDto = new SubscriptionPlanDTO
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    MonthlyPrice = (decimal)plan.PriceMonthly,
                    YearlyPrice = (decimal)plan.PriceYearly,
                    MonthlyQueryLimit = (int)plan.MaxQueriesPerMonth,
                    FileUploadSizeLimitMB = (int)(plan.AllowedFileSizeMb ?? 0),
                    HasPrioritySupport = false, // This field doesn't exist in the database model
                    IsActive = plan.IsActive,
                    AllowedChatRooms = (int)plan.AllowedChatRooms,
                    AllowedFiles = (int)plan.AllowedFiles,
                    AllowedFileSizeMb = (int)(plan.AllowedFileSizeMb ?? 0),
                    IsTrial = plan.IsTrial,
                    TrialDays = plan.TrialDays.HasValue ? (int)plan.TrialDays : null
                };

                // Get updated features
                var features = await _context.PlanFeatures
                    .Where(f => f.PlanId == planId && !f.IsDeleted)
                    .ToListAsync();

                if (features != null && features.Any())
                {
                    planDto.Features = features.Select(f => new PlanFeatureDTO
                    {
                        Id = f.Id,
                        Name = f.Feature,
                        Description = f.Feature // Feature description field doesn't exist, using the feature name
                    }).ToList();
                }

                var successMessage = _localizationService.GetMessage("SubscriptionPlanUpdated", "Messages", language);
                return BaseResponse<SubscriptionPlanDTO>.SuccessResponse(planDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan with ID: {PlanId}", id);
                var errorMessage = _localizationService.GetMessage("SubscriptionPlanUpdateError", "Errors", language);
                return BaseResponse<SubscriptionPlanDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Create a new subscription plan
        /// </summary>
        public async Task<BaseResponse<SubscriptionPlanDTO>> CreateSubscriptionPlanAsync(CreateSubscriptionPlanRequestDTO request, string adminId, string language)
        {
            try
            {
                if (!long.TryParse(adminId, out long adminIdLong))
                {
                    var invalidAdminIdMessage = _localizationService.GetMessage("InvalidAdminId", "Errors", language);
                    return BaseResponse<SubscriptionPlanDTO>.FailureResponse(invalidAdminIdMessage, 400);
                }

                _logger.LogInformation("Creating new subscription plan");

                // Create new plan
                var plan = new SubscriptionPlan
                {
                    Name = request.Name,
                    Description = request.Description,
                    PriceMonthly = (double)request.PriceMonthly,
                    PriceYearly = (double)request.PriceYearly,
                    AllowedChatRooms = request.AllowedChatRooms,
                    AllowedFiles = request.AllowedFiles,
                    AllowedFileSizeMb = request.AllowedFileSizeMb,
                    // Only set these if they exist on the request
                    MaxQueriesPerDay = request.MaxQueriesPerDay,
                    MaxQueriesPerMonth = request.MaxQueriesPerMonth,

                    TrialDays = request.TrialDays,
                    IsActive = request.IsActive,
                    IsTrial = request.IsTrial,
                    CreateDate = DateTime.UtcNow,
                    CreatedByUserId = adminIdLong
                };

                _context.SubscriptionPlans.Add(plan);
                await _context.SaveChangesAsync();

                // Add features if provided
                if (request.Features != null && request.Features.Any())
                {
                    foreach (var featureName in request.Features)
                    {
                        _context.PlanFeatures.Add(new PlanFeature
                        {
                            PlanId = plan.Id,
                            Feature = featureName,
                            CreateDate = DateTime.UtcNow,
                            CreatedByUserId = adminIdLong
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                // Map to DTO
                var planDto = new SubscriptionPlanDTO
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    MonthlyPrice = (decimal)plan.PriceMonthly,
                    YearlyPrice = (decimal)plan.PriceYearly,
                    MonthlyQueryLimit = (int)plan.MaxQueriesPerMonth,
                    FileUploadSizeLimitMB = (int)(plan.AllowedFileSizeMb ?? 0),
                    HasPrioritySupport = false, // This field doesn't exist in the database model
                    IsActive = plan.IsActive,
                    AllowedChatRooms = (int)plan.AllowedChatRooms,
                    AllowedFiles = (int)plan.AllowedFiles,
                    AllowedFileSizeMb = (int)(plan.AllowedFileSizeMb ?? 0),
                    IsTrial = plan.IsTrial,
                    TrialDays = plan.TrialDays.HasValue ? (int)plan.TrialDays : null
                };

                // Get features
                var features = await _context.PlanFeatures
                    .Where(f => f.PlanId == plan.Id && !f.IsDeleted)
                    .ToListAsync();

                if (features != null && features.Any())
                {
                    planDto.Features = features.Select(f => new PlanFeatureDTO
                    {
                        Id = f.Id,
                        Name = f.Feature,
                        Description = f.Feature // Feature description field doesn't exist, using the feature name
                    }).ToList();
                }

                var successMessage = _localizationService.GetMessage("SubscriptionPlanCreated", "Messages", language);
                return BaseResponse<SubscriptionPlanDTO>.SuccessResponse(planDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                var errorMessage = _localizationService.GetMessage("SubscriptionPlanCreationError", "Errors", language);
                return BaseResponse<SubscriptionPlanDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get subscription history for a user
        /// </summary>
        public async Task<BaseResponse<List<UserSubscriptionDTO>>> GetUserSubscriptionsHistoryAsync(string userId, string language)
        {
            try
            {
                if (!long.TryParse(userId, out long id))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<List<UserSubscriptionDTO>>.FailureResponse(invalidIdMessage, 400);
                }

                _logger.LogInformation("Retrieving subscription history for user ID: {UserId}", userId);

                // Get all subscriptions for the user
                var subscriptions = await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.UserId == id && !s.IsDeleted)
                    .OrderByDescending(s => s.CreateDate)
                    .ToListAsync();

                if (subscriptions == null || !subscriptions.Any())
                {
                    var noSubscriptionsMessage = _localizationService.GetMessage("NoSubscriptionsFound", "Messages", language);
                    return BaseResponse<List<UserSubscriptionDTO>>.SuccessResponse(new List<UserSubscriptionDTO>(), noSubscriptionsMessage);
                }

                // Map subscriptions to DTOs
                var subscriptionDtos = subscriptions.Select(s => new UserSubscriptionDTO
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    PlanId = s.PlanId,
                    PlanName = s.Plan.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate ?? DateTime.UtcNow.AddYears(100),
                    Status = s.Status,
                    QueriesUsedThisMonth = (int)s.QueriesUsedThisMonth,
                    MonthlyQueryLimit = (int)s.Plan.MaxQueriesPerMonth,
                    IsYearly = s.PeriodType == "Yearly",
                    IsPaid = true, // Assuming all subscriptions in the database are paid
                    InvoiceReference = s.PaymentGatewayTransactionId // Using transaction ID as reference
                }).ToList();

                var successMessage = _localizationService.GetMessage("UserSubscriptionsHistoryRetrieved", "Messages", language);
                return BaseResponse<List<UserSubscriptionDTO>>.SuccessResponse(subscriptionDtos, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription history for user ID: {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("UserSubscriptionsHistoryError", "Errors", language);
                return BaseResponse<List<UserSubscriptionDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get a specific subscription by ID
        /// </summary>
        public async Task<BaseResponse<UserSubscriptionDTO>> GetSubscriptionByIdAsync(string id, string language)
        {
            try
            {
                if (!long.TryParse(id, out long subscriptionId))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidSubscriptionId", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(invalidIdMessage, 400);
                }

                _logger.LogInformation("Retrieving subscription with ID: {SubscriptionId}", id);

                // Get the subscription
                var subscription = await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.Id == subscriptionId && !s.IsDeleted)
                    .FirstOrDefaultAsync();

                if (subscription == null)
                {
                    var notFoundMessage = _localizationService.GetMessage("SubscriptionNotFound", "Errors", language);
                    return BaseResponse<UserSubscriptionDTO>.FailureResponse(notFoundMessage, 404);
                }

                // Map subscription to DTO
                var subscriptionDto = new UserSubscriptionDTO
                {
                    Id = subscription.Id,
                    UserId = subscription.UserId,
                    PlanId = subscription.PlanId,
                    PlanName = subscription.Plan.Name,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate ?? DateTime.UtcNow.AddYears(100),
                    Status = subscription.Status,
                    QueriesUsedThisMonth = (int)subscription.QueriesUsedThisMonth,
                    MonthlyQueryLimit = (int)subscription.Plan.MaxQueriesPerMonth,
                    IsYearly = subscription.PeriodType == "Yearly",
                    IsPaid = true, // Assume paid if there's a subscription record
                    InvoiceReference = subscription.PaymentGatewayTransactionId // Using transaction ID as reference
                };

                var successMessage = _localizationService.GetMessage("SubscriptionRetrieved", "Messages", language);
                return BaseResponse<UserSubscriptionDTO>.SuccessResponse(subscriptionDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription with ID: {SubscriptionId}", id);
                var errorMessage = _localizationService.GetMessage("SubscriptionRetrievalError", "Errors", language);
                return BaseResponse<UserSubscriptionDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get all discount coupons
        /// </summary>
        public async Task<BaseResponse<List<DiscountCouponDTO>>> GetAllDiscountCouponsAsync(int page, int pageSize, string language)
        {
            try
            {
                // Validate pagination parameters
                page = page <= 0 ? 1 : page;
                pageSize = pageSize <= 0 ? 10 : (pageSize > 100 ? 100 : pageSize);

                _logger.LogInformation("Retrieving all discount coupons, page: {Page}, pageSize: {PageSize}", page, pageSize);

                // Get coupons with pagination
                var coupons = await _context.DiscountCoupons
                    .Where(c => c.IsDeleted == false)
                    .OrderByDescending(c => c.CreateDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (coupons == null || !coupons.Any())
                {
                    var noCouponsMessage = _localizationService.GetMessage("NoCouponsFound", "Messages", language);
                    return BaseResponse<List<DiscountCouponDTO>>.SuccessResponse(new List<DiscountCouponDTO>(), noCouponsMessage);
                }

                // Get all coupon-plan associations for these coupons
                var couponIds = coupons.Select(c => c.Id).ToList();
                var couponPlans = await _context.CouponPlans
                    .Where(cp => couponIds.Contains(cp.CouponId))
                    .ToListAsync();

                // Group by coupon ID
                var couponPlanGroups = couponPlans
                    .GroupBy(cp => cp.CouponId)
                    .ToDictionary(g => g.Key, g => g.Select(cp => cp.PlanId).ToList());

                // Get plan names
                var planIds = couponPlans.Select(cp => cp.PlanId).Distinct().ToList();
                var plans = await _context.SubscriptionPlans
                    .Where(p => planIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id, p => p.Name);

                // Map coupons to DTOs
                var couponDtos = coupons.Select(c =>
                {
                    var discountTypeEnum = (DiscountType)(c.DiscountType == "Percentage" ? 0 : 1);

                    return new DiscountCouponDTO
                    {
                        Id = c.Id.ToString(),
                        Code = c.Code,
                        Description = c.Description,
                        DiscountType = discountTypeEnum,
                        DiscountValue = (decimal)c.DiscountValue,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate.HasValue ? c.EndDate.Value : DateTime.UtcNow,
                        MaxUses = c.MaxUses.HasValue ? (int?)c.MaxUses.Value : null,
                        CurrentUses = (int)c.CurrentUses,
                        IsActive = c.IsActive,
                        ApplicablePlans = couponPlanGroups.ContainsKey(c.Id)
                            ? couponPlanGroups[c.Id]
                                .Select(pid => new PlanReferenceDTO
                                {
                                    Id = pid,
                                    Name = plans.ContainsKey(pid) ? plans[pid] : "Unknown"
                                })
                                .ToList()
                            : new List<PlanReferenceDTO>()
                    };
                }).ToList();

                var successMessage = _localizationService.GetMessage("DiscountCouponsRetrieved", "Messages", language);
                return BaseResponse<List<DiscountCouponDTO>>.SuccessResponse(couponDtos, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving discount coupons");
                var errorMessage = _localizationService.GetMessage("DiscountCouponsRetrievalError", "Errors", language);
                return BaseResponse<List<DiscountCouponDTO>>.FailureResponse(errorMessage, 500);
            }
        }
    }
}
