namespace Helpers
{
    /// <summary>
    /// فئة مساعدة للتعامل مع اللغة
    /// </summary>
    public static class LanguageHelper
    {
        /// <summary>
        /// الحصول على اللغة المفضلة من طلب HTTP
        /// </summary>
        /// <param name="request">طلب HTTP</param>
        /// <param name="configuration">إعدادات التطبيق</param>
        /// <returns>رمز اللغة المفضلة (مثل "ar" أو "en")</returns>
        public static string GetPreferredLanguage(HttpRequest request, IConfiguration configuration)
        {
            // الحصول على اللغة الافتراضية من الإعدادات
            var defaultLanguage = configuration["Localization:DefaultLanguage"] ?? "en";

            // الحصول على اللغات المدعومة من الإعدادات
            var supportedLanguages = configuration.GetSection("Localization:SupportedLanguages")
                .Get<string[]>() ?? new[] { "en", "ar" };

            // محاولة استخراج اللغة من رأس "Accept-Language"
            var acceptLanguage = request.Headers["Accept-Language"].FirstOrDefault();
            if (string.IsNullOrEmpty(acceptLanguage))
            {
                return defaultLanguage;
            }

            // تقسيم Accept-Language إلى قائمة من اللغات المفضلة
            var preferredLanguages = acceptLanguage.Split(',')
                .Select(lang => lang.Split(';')[0].Trim().ToLower())
                .ToList();

            // البحث عن أول لغة مدعومة في قائمة اللغات المفضلة
            foreach (var lang in preferredLanguages)
            {
                // التعامل مع الحالة الخاصة للغة العربية
                if (lang == "ar" || lang.StartsWith("ar-"))
                {
                    return "ar";
                }

                // التعامل مع الحالة الخاصة للغة الإنجليزية
                if (lang == "en" || lang.StartsWith("en-"))
                {
                    return "en";
                }

                // البحث عن أي لغة مدعومة أخرى
                var matchedLanguage = supportedLanguages.FirstOrDefault(sl =>
                    sl == lang || lang.StartsWith($"{sl}-"));

                if (!string.IsNullOrEmpty(matchedLanguage))
                {
                    return matchedLanguage;
                }
            }

            // إذا لم يتم العثور على لغة مدعومة، استخدم اللغة الافتراضية
            return defaultLanguage;
        }
    }
}