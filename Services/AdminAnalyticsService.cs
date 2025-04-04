using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.Admin;
using Services.Security;

namespace Services
{
    /// <summary>
    /// واجهة خدمة إحصائيات لوحة التحكم للمسؤول
    /// </summary>
    public interface IAdminAnalyticsService
    {
        /// <summary>
        /// الحصول على ملخص لوحة التحكم
        /// </summary>
        Task<BaseResponse<DashboardSummaryDTO>> GetDashboardSummaryAsync(DateTime fromDate, DateTime toDate, string language);
        
        /// <summary>
        /// الحصول على إحصائيات المستخدمين
        /// </summary>
        Task<BaseResponse<UserAnalyticsDTO>> GetUserAnalyticsAsync(DateTime fromDate, DateTime toDate, string language);
        
        /// <summary>
        /// الحصول على إحصائيات الاشتراكات
        /// </summary>
        Task<BaseResponse<SubscriptionAnalyticsDTO>> GetSubscriptionAnalyticsAsync(DateTime fromDate, DateTime toDate, string language);
        
        /// <summary>
        /// الحصول على إحصائيات الاستعلامات
        /// </summary>
        Task<BaseResponse<QueryAnalyticsDTO>> GetQueryAnalyticsAsync(DateTime fromDate, DateTime toDate, string language);
        
        /// <summary>
        /// الحصول على إحصائيات المبيعات والإيرادات
        /// </summary>
        Task<BaseResponse<RevenueAnalyticsDTO>> GetRevenueAnalyticsAsync(DateTime fromDate, DateTime toDate, string language);
    }
    
    /// <summary>
    /// تنفيذ خدمة إحصائيات لوحة التحكم للمسؤول
    /// </summary>
    public class AdminAnalyticsService : IAdminAnalyticsService
    {
        private readonly MuhamiContext _context;
        private readonly ILogger<AdminAnalyticsService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IEncryptionService _encryptionService;
        
        public AdminAnalyticsService(
            MuhamiContext context,
            ILogger<AdminAnalyticsService> logger,
            ILocalizationService localizationService,
            IEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _localizationService = localizationService;
            _encryptionService = encryptionService;
        }
        
        /// <summary>
        /// الحصول على ملخص لوحة التحكم
        /// </summary>
        public async Task<BaseResponse<DashboardSummaryDTO>> GetDashboardSummaryAsync(DateTime fromDate, DateTime toDate, string language)
        {
            try
            {
                // حساب الفترة السابقة بنفس الطول
                var timeDifference = toDate - fromDate;
                var previousFromDate = fromDate.AddDays(-timeDifference.TotalDays);
                var previousToDate = fromDate.AddDays(-1);
                
                // إجمالي عدد المستخدمين
                var totalUsers = await _context.Users.CountAsync(u => !u.IsDeleted);
                
                // المستخدمين الجدد في الفترة المحددة
                var newUsers = await _context.Users
                    .CountAsync(u => u.CreateDate >= fromDate && u.CreateDate <= toDate && !u.IsDeleted);
                
                // المستخدمين الجدد في الفترة السابقة
                var previousNewUsers = await _context.Users
                    .CountAsync(u => u.CreateDate >= previousFromDate && u.CreateDate <= previousToDate && !u.IsDeleted);
                
                // إجمالي الاشتراكات النشطة
                var activeSubscriptions = await _context.UserSubscriptions
                    .CountAsync(s => s.Status == "Active" && !s.IsDeleted && s.EndDate > DateTime.Now);
                
                // إجمالي الإيرادات في الفترة المحددة
                var totalRevenue = await _context.UserSubscriptions
                    .Where(i => i.CreateDate >= fromDate && i.CreateDate <= toDate && i.Status == "Active")
                    .Join(_context.SubscriptionPlans, 
                           sub => sub.PlanId, 
                           plan => plan.Id, 
                           (sub, plan) => new { sub, plan })
                    .SumAsync(x => x.sub.PeriodType == "Monthly" ? x.plan.PriceMonthly : x.plan.PriceYearly);
                
                // إجمالي الإيرادات في الفترة السابقة
                var previousTotalRevenue = await _context.UserSubscriptions
                    .Where(i => i.CreateDate >= previousFromDate && i.CreateDate <= previousToDate && i.Status == "Active")
                    .Join(_context.SubscriptionPlans, 
                           sub => sub.PlanId, 
                           plan => plan.Id, 
                           (sub, plan) => new { sub, plan })
                    .SumAsync(x => x.sub.PeriodType == "Monthly" ? x.plan.PriceMonthly : x.plan.PriceYearly);
                
                // إجمالي عدد الاستعلامات في الفترة المحددة
                var totalQueries = await _context.ChatMessages
                    .CountAsync(m => m.CreateDate >= fromDate && m.CreateDate <= toDate && m.Role == "user");
                
                // حساب معدلات النمو
                var userGrowthRate = CalculateGrowthRate(newUsers, previousNewUsers);
                var revenueGrowthRate = CalculateGrowthRate((float)totalRevenue, (float)previousTotalRevenue);
                
                // حساب متوسط الاستعلامات اليومية
                var dayCount = Math.Max(1, (int)timeDifference.TotalDays);
                var averageDailyQueries = totalQueries / dayCount;
                
                var summary = new DashboardSummaryDTO
                {
                    TotalUsers = totalUsers,
                    NewUsers = newUsers,
                    ActiveSubscriptions = activeSubscriptions,
                    TotalRevenue = totalRevenue,
                    TotalQueries = totalQueries,
                    AverageDailyQueries = averageDailyQueries,
                    UserGrowthRate = userGrowthRate,
                    RevenueGrowthRate = revenueGrowthRate
                };
                
                var successMessage = _localizationService.GetMessage("DashboardSummaryRetrieved", "Messages", language);
                return BaseResponse<DashboardSummaryDTO>.SuccessResponse(summary, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على ملخص لوحة التحكم");
                var errorMessage = _localizationService.GetMessage("DashboardSummaryError", "Errors", language);
                return BaseResponse<DashboardSummaryDTO>.FailureResponse(errorMessage, 500);
            }
        }
        
        /// <summary>
        /// الحصول على إحصائيات المستخدمين
        /// </summary>
        public async Task<BaseResponse<UserAnalyticsDTO>> GetUserAnalyticsAsync(DateTime fromDate, DateTime toDate, string language)
        {
            try
            {
                // إجمالي عدد المستخدمين
                var totalUsers = await _context.Users.CountAsync(u => !u.IsDeleted);
                
                // المستخدمين الجدد في الفترة المحددة
                var newUsers = await _context.Users
                    .CountAsync(u => u.CreateDate >= fromDate && u.CreateDate <= toDate && !u.IsDeleted);
                
                // المستخدمين النشطين في الفترة المحددة (لديهم رسائل)
                var activeUserIds = await _context.ChatMessages
                    .Where(m => m.CreateDate >= fromDate && m.CreateDate <= toDate)
                    .Select(m => m.CreatedByUserId)
                    .Distinct()
                    .ToListAsync();
                
                var activeUsers = activeUserIds.Count;
                
                // حساب نسبة الاحتفاظ (نسبة المستخدمين الذين ظلوا نشطين)
                float retentionRate = 0;
                if (totalUsers > 0)
                {
                    retentionRate = (float)activeUsers / totalUsers * 100;
                }
                
                // توزيع المستخدمين حسب الخطة
                var usersByPlan = await GetUsersByPlanAsync();
                
                // المستخدمين الجدد حسب اليوم
                var newUsersByDay = await GetNewUsersByDayAsync(fromDate, toDate);
                
                var analytics = new UserAnalyticsDTO
                {
                    TotalUsers = totalUsers,
                    NewUsers = newUsers,
                    ActiveUsers = activeUsers,
                    RetentionRate = retentionRate,
                    UsersByPlan = usersByPlan,
                    NewUsersByDay = newUsersByDay
                };
                
                var successMessage = _localizationService.GetMessage("UserAnalyticsRetrieved", "Messages", language);
                return BaseResponse<UserAnalyticsDTO>.SuccessResponse(analytics, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على إحصائيات المستخدمين");
                var errorMessage = _localizationService.GetMessage("UserAnalyticsError", "Errors", language);
                return BaseResponse<UserAnalyticsDTO>.FailureResponse(errorMessage, 500);
            }
        }
        
        /// <summary>
        /// الحصول على إحصائيات الاشتراكات
        /// </summary>
        public async Task<BaseResponse<SubscriptionAnalyticsDTO>> GetSubscriptionAnalyticsAsync(DateTime fromDate, DateTime toDate, string language)
        {
            try
            {
                // إجمالي عدد الاشتراكات النشطة
                var activeSubscriptions = await _context.UserSubscriptions
                    .CountAsync(s => s.Status == "Active" && !s.IsDeleted);
                
                // الاشتراكات الجديدة في الفترة المحددة
                var newSubscriptions = await _context.UserSubscriptions
                    .CountAsync(s => s.CreateDate >= fromDate && s.CreateDate <= toDate && !s.IsDeleted);
                
                // الاشتراكات المنتهية في الفترة المحددة
                var expiredSubscriptions = await _context.UserSubscriptions
                    .CountAsync(s => s.EndDate >= fromDate && s.EndDate <= toDate && s.Status == "Expired" && !s.IsDeleted);
                
                // متوسط مدة الاشتراك (بالأيام)
                var allSubs = await _context.UserSubscriptions.Where(s => !s.IsDeleted).ToListAsync();
                double averageSubscriptionDuration = 0;
                if (allSubs.Any())
                {
                    averageSubscriptionDuration = allSubs
                        .Where(s => s.EndDate.HasValue && s.StartDate <= s.EndDate.Value)
                        .Average(s => (s.EndDate.Value - s.StartDate).TotalDays);
                }
                
                // معدل التجديد (نسبة الاشتراكات التي تم تجديدها)
                double renewalRate = 0;
                var renewableSubscriptions = await _context.UserSubscriptions
                    .CountAsync(s => s.EndDate >= fromDate && s.EndDate <= toDate && !s.IsDeleted);
                
                if (renewableSubscriptions > 0)
                {
                    var renewedSubscriptions = await _context.UserSubscriptions
                        .CountAsync(s => s.LastRenewalDate >= fromDate && s.LastRenewalDate <= toDate && !s.IsDeleted);
                    
                    renewalRate = (double)renewedSubscriptions / renewableSubscriptions * 100;
                }
                
                // توزيع الاشتراكات حسب الخطة
                var distributionByPlan = await GetSubscriptionDistributionByPlanAsync();
                
                // الاشتراكات الجديدة حسب اليوم
                var subscriptionsByDay = await GetSubscriptionsByDayAsync(fromDate, toDate);
                
                var analytics = new SubscriptionAnalyticsDTO
                {
                    ActiveSubscriptions = activeSubscriptions,
                    NewSubscriptions = newSubscriptions,
                    ExpiredSubscriptions = expiredSubscriptions,
                    RenewalRate = (float)renewalRate,
                    ConversionRate = 0,
                    SubscriptionsByPlan = distributionByPlan,
                    NewSubscriptionsByDay = subscriptionsByDay
                };
                
                var successMessage = _localizationService.GetMessage("SubscriptionAnalyticsRetrieved", "Messages", language);
                return BaseResponse<SubscriptionAnalyticsDTO>.SuccessResponse(analytics, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على إحصائيات الاشتراكات");
                var errorMessage = _localizationService.GetMessage("SubscriptionAnalyticsError", "Errors", language);
                return BaseResponse<SubscriptionAnalyticsDTO>.FailureResponse(errorMessage, 500);
            }
        }
        
        /// <summary>
        /// الحصول على إحصائيات الاستعلامات
        /// </summary>
        public async Task<BaseResponse<QueryAnalyticsDTO>> GetQueryAnalyticsAsync(DateTime fromDate, DateTime toDate, string language)
        {
            try
            {
                // إجمالي عدد الاستعلامات
                var totalQueries = await _context.ChatMessages
                    .CountAsync(m => m.CreateDate >= fromDate && m.CreateDate <= toDate && m.Role == "user" && !m.IsDeleted);
                
                // متوسط عدد الاستعلامات اليومية
                var dayCount = Math.Max(1, (int)(toDate - fromDate).TotalDays);
                double averageDailyQueries = (double)totalQueries / dayCount;
                
                // متوسط عدد الاستعلامات لكل مستخدم
                var activeUsers = await _context.ChatMessages
                    .Where(m => m.CreateDate >= fromDate && m.CreateDate <= toDate && m.Role == "user" && !m.IsDeleted)
                    .Select(m => m.CreatedByUserId)
                    .Distinct()
                    .CountAsync();
                
                double averageQueriesPerUser = activeUsers > 0 ? (double)totalQueries / activeUsers : 0;
                
                // الاستعلامات حسب اليوم
                var queriesByDay = await GetQueriesByDayAsync(fromDate, toDate);
                
                // أكثر الكلمات المفتاحية استخداماً
                var topKeywords = await GetTopKeywordsAsync(fromDate, toDate, 10);
                
                // أكثر الاستعلامات تكراراً
                var topQueries = await GetTopQueriesAsync(fromDate, toDate, 10);
                
                var analytics = new QueryAnalyticsDTO
                {
                    TotalQueries = totalQueries,
                    AverageDailyQueries = (float)averageDailyQueries,
                    AverageQueriesPerUser = (float)averageQueriesPerUser,
                    QueriesByDay = queriesByDay,
                    TopKeywords = topKeywords,
                    TopQueries = topQueries
                };
                
                var successMessage = _localizationService.GetMessage("QueryAnalyticsRetrieved", "Messages", language);
                return BaseResponse<QueryAnalyticsDTO>.SuccessResponse(analytics, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على إحصائيات الاستعلامات");
                var errorMessage = _localizationService.GetMessage("QueryAnalyticsError", "Errors", language);
                return BaseResponse<QueryAnalyticsDTO>.FailureResponse(errorMessage, 500);
            }
        }
        
        /// <summary>
        /// الحصول على إحصائيات المبيعات والإيرادات
        /// </summary>
        public async Task<BaseResponse<RevenueAnalyticsDTO>> GetRevenueAnalyticsAsync(DateTime fromDate, DateTime toDate, string language)
        {
            try
            {
                // إجمالي الإيرادات في الفترة المحددة
                var totalRevenue = await _context.UserSubscriptions
                    .Where(s => s.CreateDate >= fromDate && s.CreateDate <= toDate && !s.IsDeleted)
                    .Join(_context.SubscriptionPlans, 
                           sub => sub.PlanId, 
                           plan => plan.Id, 
                           (sub, plan) => new { sub, plan })
                    .SumAsync(x => x.sub.PeriodType == "Monthly" ? x.plan.PriceMonthly : x.plan.PriceYearly);
                
                // متوسط الإيرادات اليومية
                var dayCount = Math.Max(1, (int)(toDate - fromDate).TotalDays);
                var averageDailyRevenue = totalRevenue / dayCount;
                
                // إجمالي عدد المعاملات
                var totalTransactions = await _context.UserSubscriptions
                    .CountAsync(s => s.CreateDate >= fromDate && s.CreateDate <= toDate && !s.IsDeleted);
                
                // متوسط قيمة المعاملة
                var averageTransactionValue = totalTransactions > 0 ? totalRevenue / totalTransactions : 0;
                
                // الإيرادات حسب اليوم
                var revenueByDay = await GetRevenueByDayAsync(fromDate, toDate);
                
                // الإيرادات حسب الخطة
                var revenueByPlan = await GetRevenueByPlanAsync(fromDate, toDate);
                
                // توزيع طرق الدفع
                var paymentMethodDistribution = await GetPaymentMethodDistributionAsync(fromDate, toDate);
                
                var analytics = new RevenueAnalyticsDTO();
                
                // Explicitly set each property with appropriate conversions
                analytics.TotalRevenue = (decimal)totalRevenue;
                analytics.AverageDailyRevenue = (decimal)averageDailyRevenue;
                analytics.TotalTransactions = totalTransactions;
                analytics.AverageTransactionValue = (decimal)averageTransactionValue;
                
                // Convert Dictionary<string, double> to Dictionary<string, decimal>
                analytics.RevenueByDay = new Dictionary<string, decimal>();
                foreach (var item in revenueByDay)
                {
                    analytics.RevenueByDay.Add(item.Key, (decimal)item.Value);
                }
                
                analytics.RevenueByPlan = new Dictionary<string, decimal>();
                foreach (var item in revenueByPlan)
                {
                    analytics.RevenueByPlan.Add(item.Key, (decimal)item.Value);
                }
                
                analytics.PaymentMethodDistribution = paymentMethodDistribution;
                
                var successMessage = _localizationService.GetMessage("RevenueAnalyticsRetrieved", "Messages", language);
                return BaseResponse<RevenueAnalyticsDTO>.SuccessResponse(analytics, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على إحصائيات المبيعات والإيرادات");
                var errorMessage = _localizationService.GetMessage("RevenueAnalyticsError", "Errors", language);
                return BaseResponse<RevenueAnalyticsDTO>.FailureResponse(errorMessage, 500);
            }
        }
        
        #region Helper Methods
        
        /// <summary>
        /// حساب معدل النمو
        /// </summary>
        private float CalculateGrowthRate(float current, float previous)
        {
            if (previous == 0)
                return current > 0 ? 100 : 0;
                
            return (current - previous) / previous * 100;
        }
        
        /// <summary>
        /// الحصول على توزيع المستخدمين حسب الخطة
        /// </summary>
        private async Task<Dictionary<string, int>> GetUsersByPlanAsync()
        {
            var plans = await _context.SubscriptionPlans
                .Where(p => !p.IsDeleted)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            
            var result = new Dictionary<string, int>();
            foreach (var plan in plans)
            {
                var count = await _context.UserSubscriptions
                    .CountAsync(s => s.PlanId == plan.Id && s.Status == "Active" && !s.IsDeleted);
                result.Add(plan.Name, count);
            }
            
            // Add "Free" users (users with no active subscription)
            var usersWithSubs = await _context.UserSubscriptions
                .Where(s => s.Status == "Active" && !s.IsDeleted)
                .Select(s => s.UserId)
                .Distinct()
                .ToListAsync();
            
            var freeUsers = await _context.Users
                .CountAsync(u => !u.IsDeleted && !usersWithSubs.Contains(u.Id));
            
            result.Add("Free", freeUsers);
            
            return result;
        }
        
        /// <summary>
        /// الحصول على المستخدمين الجدد حسب اليوم
        /// </summary>
        private async Task<Dictionary<string, int>> GetNewUsersByDayAsync(DateTime fromDate, DateTime toDate)
        {
            var users = await _context.Users
                .Where(u => u.CreateDate >= fromDate && u.CreateDate <= toDate && !u.IsDeleted)
                .ToListAsync();
            
            var result = new Dictionary<string, int>();
            var currentDate = fromDate.Date;
            
            while (currentDate <= toDate.Date)
            {
                var dateStr = currentDate.ToString("yyyy-MM-dd");
                var count = users.Count(u => u.CreateDate.Date == currentDate);
                result.Add(dateStr, count);
                currentDate = currentDate.AddDays(1);
            }
            
            return result;
        }
        
        /// <summary>
        /// الحصول على توزيع الاشتراكات حسب الخطة
        /// </summary>
        private async Task<Dictionary<string, int>> GetSubscriptionDistributionByPlanAsync()
        {
            var plans = await _context.SubscriptionPlans
                .Where(p => !p.IsDeleted)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            
            var result = new Dictionary<string, int>();
            foreach (var plan in plans)
            {
                var count = await _context.UserSubscriptions
                    .CountAsync(s => s.PlanId == plan.Id && s.Status == "Active" && !s.IsDeleted);
                result.Add(plan.Name, count);
            }
            
            return result;
        }
        
        /// <summary>
        /// الحصول على الاشتراكات الجديدة حسب اليوم
        /// </summary>
        private async Task<Dictionary<string, int>> GetSubscriptionsByDayAsync(DateTime fromDate, DateTime toDate)
        {
            var subscriptions = await _context.UserSubscriptions
                .Where(s => s.CreateDate >= fromDate && s.CreateDate <= toDate && !s.IsDeleted)
                .ToListAsync();
            
            var result = new Dictionary<string, int>();
            var currentDate = fromDate.Date;
            
            while (currentDate <= toDate.Date)
            {
                var dateStr = currentDate.ToString("yyyy-MM-dd");
                var count = subscriptions.Count(s => s.CreateDate.Date == currentDate);
                result.Add(dateStr, count);
                currentDate = currentDate.AddDays(1);
            }
            
            return result;
        }
        
        /// <summary>
        /// الحصول على الاستعلامات حسب اليوم
        /// </summary>
        private async Task<Dictionary<string, int>> GetQueriesByDayAsync(DateTime fromDate, DateTime toDate)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.CreateDate >= fromDate && m.CreateDate <= toDate && m.Role == "user" && !m.IsDeleted)
                .ToListAsync();
            
            var result = new Dictionary<string, int>();
            var currentDate = fromDate.Date;
            
            while (currentDate <= toDate.Date)
            {
                var dateStr = currentDate.ToString("yyyy-MM-dd");
                var count = messages.Count(m => m.CreateDate.Date == currentDate);
                result.Add(dateStr, count);
                currentDate = currentDate.AddDays(1);
            }
            
            return result;
        }
        
        /// <summary>
        /// الحصول على أعلى 10 كلمات مفتاحية في الاستعلامات
        /// </summary>
        private async Task<List<KeywordCountDTO>> GetTopKeywordsAsync(DateTime fromDate, DateTime toDate, int limit)
        {
            var keywords = await _context.ConversationKeywords
                .Where(k => k.CreateDate >= fromDate && k.CreateDate <= toDate && !k.IsDeleted)
                .GroupBy(k => k.Keyword)
                .Select(g => new KeywordCountDTO { Keyword = g.Key, Count = (int)g.Sum(k => k.Count) })
                .OrderByDescending(r => r.Count)
                .Take(limit)
                .ToListAsync();
            
            return keywords;
        }
        
        /// <summary>
        /// الحصول على أكثر 10 استعلامات شيوعًا
        /// </summary>
        private async Task<List<QueryCountDTO>> GetTopQueriesAsync(DateTime fromDate, DateTime toDate, int limit)
        {
            var topQueries = await _context.ChatMessages
                .Where(m => m.CreateDate >= fromDate && m.CreateDate <= toDate && m.Role == "user" && !m.IsDeleted)
                .GroupBy(m => m.Content)
                .Select(g => new QueryCountDTO { Query = g.Key, Count = g.Count() })
                .OrderByDescending(r => r.Count)
                .Take(limit)
                .ToListAsync();
            
            return topQueries;
        }
        
        /// <summary>
        /// الحصول على الإيرادات حسب اليوم
        /// </summary>
        private async Task<Dictionary<string, double>> GetRevenueByDayAsync(DateTime fromDate, DateTime toDate)
        {
            var subscriptions = await _context.UserSubscriptions
                .Where(s => s.CreateDate >= fromDate && s.CreateDate <= toDate && !s.IsDeleted)
                .Join(_context.SubscriptionPlans, 
                       sub => sub.PlanId, 
                       plan => plan.Id, 
                       (sub, plan) => new { 
                           Sub = sub, 
                           Amount = sub.PeriodType == "Monthly" ? plan.PriceMonthly : plan.PriceYearly 
                       })
                .ToListAsync();
            
            var result = new Dictionary<string, double>();
            var currentDate = fromDate.Date;
            
            while (currentDate <= toDate.Date)
            {
                var dateStr = currentDate.ToString("yyyy-MM-dd");
                var amount = subscriptions
                    .Where(s => s.Sub.CreateDate.Date == currentDate)
                    .Sum(s => (double)s.Amount);
                result.Add(dateStr, amount);
                currentDate = currentDate.AddDays(1);
            }
            
            return result;
        }
        
        /// <summary>
        /// الحصول على الإيرادات حسب خطة الاشتراك
        /// </summary>
        private async Task<Dictionary<string, double>> GetRevenueByPlanAsync(DateTime fromDate, DateTime toDate)
        {
            var plans = await _context.SubscriptionPlans
                .Where(p => !p.IsDeleted)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            
            var result = new Dictionary<string, double>();
            
            foreach (var plan in plans)
            {
                var revenue = await _context.UserSubscriptions
                    .Where(s => s.CreateDate >= fromDate && s.CreateDate <= toDate && 
                           s.PlanId == plan.Id && !s.IsDeleted)
                    .Join(_context.SubscriptionPlans, 
                           sub => sub.PlanId, 
                           p => p.Id, 
                           (sub, p) => new { 
                               sub, 
                               Amount = sub.PeriodType == "Monthly" ? p.PriceMonthly : p.PriceYearly 
                           })
                    .SumAsync(x => (double)x.Amount);
                
                result.Add(plan.Name, revenue);
            }
            
            return result;
        }
        
        /// <summary>
        /// الحصول على توزيع طرق الدفع
        /// </summary>
        private async Task<Dictionary<string, int>> GetPaymentMethodDistributionAsync(DateTime fromDate, DateTime toDate)
        {
            var distribution = await _context.UserSubscriptions
                .Where(s => s.CreateDate >= fromDate && s.CreateDate <= toDate && 
                       !s.IsDeleted && s.PaymentMethod != null)
                .GroupBy(s => s.PaymentMethod)
                .Select(g => new { Method = g.Key, Count = g.Count() })
                .ToListAsync();
            
            var result = new Dictionary<string, int>();
            foreach (var item in distribution)
            {
                result.Add(item.Method, item.Count);
            }
            
            return result;
        }
        
        #endregion
    }
} 