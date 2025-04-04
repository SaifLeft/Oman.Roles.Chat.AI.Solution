using Microsoft.Extensions.Caching.Memory;
using Models.Common;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    /// <summary>
    /// ميدلوير للحد من معدل الطلبات
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly int _maxRequests; // الحد الأقصى للطلبات المسموح بها
        private readonly TimeSpan _timeWindow; // النافذة الزمنية للحد

        public RateLimitingMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            ILogger<RateLimitingMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _cache = cache;
            _logger = logger;

            // استخراج إعدادات الحد من التكوين
            var rateLimitSettings = configuration.GetSection("RateLimitSettings");
            _maxRequests = rateLimitSettings.GetValue<int>("MaxRequests", 100); // افتراضي: 100 طلب
            var windowInSeconds = rateLimitSettings.GetValue<int>("WindowInSeconds", 60); // افتراضي: 60 ثانية
            _timeWindow = TimeSpan.FromSeconds(windowInSeconds);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // استخراج عنوان IP للطلب
            var ipAddress = GetClientIpAddress(context);

            // استخراج مسار الطلب
            var path = context.Request.Path.ToString();

            // إنشاء مفتاح فريد للمستخدم والمسار
            var key = $"{ipAddress}_{path}";

            // الحصول على عداد الطلبات الحالي
            var counter = _cache.GetOrCreate(key, entry =>
            {
                entry.SetAbsoluteExpiration(_timeWindow);
                return new RateLimitCounter { Count = 0, FirstRequest = DateTime.Now };
            });

            // زيادة العداد
            counter.Count++;

            // تحديث الذاكرة المؤقتة
            _cache.Set(key, counter, _timeWindow);

            // إضافة رؤوس الاستجابة للحد من المعدل
            context.Response.Headers.Add("X-Rate-Limit-Limit", _maxRequests.ToString());
            context.Response.Headers.Add("X-Rate-Limit-Remaining", Math.Max(0, _maxRequests - counter.Count).ToString());
            context.Response.Headers.Add("X-Rate-Limit-Reset", (counter.FirstRequest.Add(_timeWindow) - DateTime.Now).TotalSeconds.ToString());

            // التحقق من تجاوز الحد
            if (counter.Count > _maxRequests)
            {
                _logger.LogWarning("Rate limit exceeded for IP {IpAddress} on path {Path}", ipAddress, path);

                // إنشاء استجابة خطأ
                var response = BaseResponse.FailureResponse(
                    "لقد تجاوزت الحد الأقصى للطلبات. يرجى المحاولة مرة أخرى لاحقاً.",
                    (int)HttpStatusCode.TooManyRequests);

                // تعيين حالة الاستجابة ونوع المحتوى
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";

                // إرسال استجابة الخطأ كـ JSON
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            // تنفيذ الميدلوير التالي في السلسلة
            await _next(context);
        }

        /// <summary>
        /// الحصول على عنوان IP للعميل
        /// </summary>
        private string GetClientIpAddress(HttpContext context)
        {
            // محاولة الحصول على عنوان IP من الرؤوس الشائعة
            string? ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                               context.Request.Headers["X-Real-IP"].FirstOrDefault();

            // إذا لم يتم العثور على عنوان IP في الرؤوس، استخدم عنوان RemoteIpAddress
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress?.ToString();
            }

            // إذا لا يزال فارغًا، استخدم عنوانًا افتراضيًا
            return ipAddress ?? "0.0.0.0";
        }
    }

    /// <summary>
    /// امتداد لتسجيل ميدلوير الحد من المعدل
    /// </summary>
    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }

    /// <summary>
    /// كائن لتتبع عدد الطلبات
    /// </summary>
    public class RateLimitCounter
    {
        public int Count { get; set; }
        public DateTime FirstRequest { get; set; }
    }
}