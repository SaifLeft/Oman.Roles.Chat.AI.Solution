using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace API.Helpers
{
    /// <summary>
    /// مساعد لتحديد لغة المستخدم المفضلة
    /// </summary>
    public static class LanguageHelper
    {
        /// <summary>
        /// الحصول على اللغة المفضلة للمستخدم من الطلب HTTP
        /// </summary>
        /// <param name="request">طلب HTTP</param>
        /// <param name="configuration">إعدادات التطبيق</param>
        /// <returns>رمز اللغة (ar, en)</returns>
        public static string GetPreferredLanguage(HttpRequest request, IConfiguration configuration)
        {
            // أولوية 1: التحقق من وجود لغة محددة في سلسلة الاستعلام
            if (request.Query.ContainsKey("lang"))
            {
                var queryLang = request.Query["lang"].ToString().ToLower();
                if (IsValidLanguage(queryLang))
                {
                    return queryLang;
                }
            }

            // أولوية 2: التحقق من وجود لغة محددة في رأس الطلب
            if (request.Headers.ContainsKey("Accept-Language"))
            {
                var acceptLanguage = request.Headers["Accept-Language"].ToString();
                var preferredLanguage = ParseAcceptLanguageHeader(acceptLanguage);
                if (IsValidLanguage(preferredLanguage))
                {
                    return preferredLanguage;
                }
            }

            // أولوية 3: استخدام اللغة الافتراضية من إعدادات التطبيق
            var defaultLanguage = configuration["DefaultLanguage"];
            if (!string.IsNullOrEmpty(defaultLanguage) && IsValidLanguage(defaultLanguage))
            {
                return defaultLanguage;
            }

            // القيمة الافتراضية إذا لم يتم العثور على لغة مفضلة
            return "ar";
        }

        /// <summary>
        /// تحليل رأس قبول اللغة
        /// </summary>
        private static string ParseAcceptLanguageHeader(string acceptLanguageHeader)
        {
            if (string.IsNullOrEmpty(acceptLanguageHeader))
            {
                return string.Empty;
            }

            // تقسيم الرأس إلى قائمة من اللغات المفضلة
            var languages = acceptLanguageHeader.Split(',');
            if (languages.Length > 0)
            {
                // الحصول على اللغة الأكثر تفضيلًا
                var preferred = languages[0].Trim().Split(';')[0].Trim();
                
                // الحصول على الجزء الأول من اللغة (مثل 'ar' من 'ar-SA')
                if (preferred.Contains('-'))
                {
                    preferred = preferred.Split('-')[0];
                }
                
                return preferred.ToLower();
            }

            return string.Empty;
        }

        /// <summary>
        /// التحقق من صحة رمز اللغة
        /// </summary>
        private static bool IsValidLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                return false;
            }

            // التحقق مما إذا كانت اللغة مدعومة
            // يمكن تغيير هذه القائمة حسب اللغات المدعومة في التطبيق
            var supportedLanguages = new List<string> { "ar", "en" };
            return supportedLanguages.Contains(language.ToLower());
        }
    }
} 