namespace API.Middleware
{
    /// <summary>
    /// ميدلوير لإضافة رؤوس الأمان إلى استجابات HTTP
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // إضافة رؤوس أمان OWASP المعيارية
            
            // منع تخمين نوع المحتوى
            if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            }
            
            // تفعيل الحماية من هجمات XSS في المتصفحات القديمة
            if (!context.Response.Headers.ContainsKey("X-XSS-Protection"))
            {
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            }
            
            // منع تضمين الصفحة في إطار من موقع آخر (لمنع هجمات Clickjacking)
            if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
            }
            
            // تحديد سياسة أمان المحتوى
            if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
            {
                context.Response.Headers.Add(
                    "Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data:; " +
                    "font-src 'self'; " +
                    "connect-src 'self'");
            }
            
            // منع تخزين البيانات الحساسة في ذاكرة التخزين المؤقت للمتصفح
            if (!context.Response.Headers.ContainsKey("Cache-Control"))
            {
                context.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, max-age=0");
            }
            
            // إضافة رأس Referrer-Policy
            if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
            {
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            }
            
            // تنفيذ الميدلوير التالي في السلسلة
            await _next(context);
        }
    }

    /// <summary>
    /// امتداد لتسجيل ميدلوير رؤوس الأمان
    /// </summary>
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
} 