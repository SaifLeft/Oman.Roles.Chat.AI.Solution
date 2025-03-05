using System.Globalization;

namespace Maui.Service
{
    public interface ILocalizationService
    {
        Task SetLanguageAsync(string language);
        Task<string> GetCurrentLanguageAsync();
        event EventHandler<LanguageChangedEventArgs> LanguageChanged;
        string GetLocalizedString(string key);
        FlowDirection GetFlowDirection();
    }

    public class LanguageChangedEventArgs : EventArgs
    {
        public string Language { get; set; }
        public FlowDirection FlowDirection { get; set; }
    }

    public class LocalizationService : ILocalizationService
    {
        private readonly IPreferencesService _preferencesService;
        private Dictionary<string, Dictionary<string, string>> _localizedStrings;
        private const string LanguageKey = "CurrentLanguage";
        private string _currentLanguage;

        public event EventHandler<LanguageChangedEventArgs> LanguageChanged;

        public LocalizationService(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
            InitializeLocalizedStrings();
        }

        private void InitializeLocalizedStrings()
        {
            _localizedStrings = new Dictionary<string, Dictionary<string, string>>
            {
                ["ar"] = new Dictionary<string, string>
                {
                    ["Login"] = "تسجيل الدخول",
                    ["Register"] = "إنشاء حساب",
                    ["Username"] = "اسم المستخدم",
                    ["Password"] = "كلمة المرور",
                    ["ForgotPassword"] = "نسيت كلمة المرور؟",
                    ["FullName"] = "الاسم الكامل",
                    ["Email"] = "البريد الإلكتروني",
                    ["PhoneNumber"] = "رقم الهاتف",
                    ["ConfirmPassword"] = "تأكيد كلمة المرور",
                    ["AcceptTerms"] = "أوافق على شروط وأحكام الاستخدام",
                    ["AlreadyHaveAccount"] = "لديك حساب بالفعل؟",
                    ["DontHaveAccount"] = "ليس لديك حساب؟",
                    ["LoginWithPhone"] = "تسجيل الدخول برقم الهاتف",
                    ["LoginWithGoogle"] = "تسجيل الدخول باستخدام Google",
                    ["Or"] = "أو",
                    ["BackToLogin"] = "العودة إلى صفحة تسجيل الدخول",
                    ["SmartLawyer"] = "المحامي الذكي",
                    ["EnterLoginInfo"] = "أدخل بيانات الدخول للمتابعة",
                    ["Reset"] = "إعادة تعيين",
                    ["ResetPassword"] = "إعادة تعيين كلمة المرور",
                    ["EnterEmail"] = "أدخل البريد الإلكتروني",
                    ["VerificationCode"] = "رمز التحقق",
                    ["NewPassword"] = "كلمة المرور الجديدة",
                    ["SendCode"] = "إرسال الرمز",
                    ["EnterRegistrationInfo"] = "أدخل المعلومات المطلوبة لإنشاء حسابك",
                    ["LoginSuccess"] = "تم تسجيل الدخول بنجاح",
                    ["LoginError"] = "فشل تسجيل الدخول، يرجى التحقق من بيانات الدخول والمحاولة مرة أخرى"
                },
                ["en"] = new Dictionary<string, string>
                {
                    ["Login"] = "Login",
                    ["Register"] = "Register",
                    ["Username"] = "Username",
                    ["Password"] = "Password",
                    ["ForgotPassword"] = "Forgot Password?",
                    ["FullName"] = "Full Name",
                    ["Email"] = "Email",
                    ["PhoneNumber"] = "Phone Number",
                    ["ConfirmPassword"] = "Confirm Password",
                    ["AcceptTerms"] = "I accept the terms and conditions",
                    ["AlreadyHaveAccount"] = "Already have an account?",
                    ["DontHaveAccount"] = "Don't have an account?",
                    ["LoginWithPhone"] = "Login with Phone Number",
                    ["LoginWithGoogle"] = "Login with Google",
                    ["Or"] = "OR",
                    ["BackToLogin"] = "Back to Login",
                    ["SmartLawyer"] = "Smart Lawyer",
                    ["EnterLoginInfo"] = "Enter your login details to continue",
                    ["Reset"] = "Reset",
                    ["ResetPassword"] = "Reset Password",
                    ["EnterEmail"] = "Enter your email",
                    ["VerificationCode"] = "Verification Code",
                    ["NewPassword"] = "New Password",
                    ["SendCode"] = "Send Code",
                    ["EnterRegistrationInfo"] = "Enter the required information to create your account"
                }
            };
        }

        public async Task SetLanguageAsync(string language)
        {
            if (!_localizedStrings.ContainsKey(language))
            {
                throw new ArgumentException($"Language {language} is not supported");
            }

            var previousLanguage = _currentLanguage;
            _currentLanguage = language;

            // Set the current culture
            CultureInfo culture = new CultureInfo(language);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Save the language preference
            await _preferencesService.SaveValueAsync(LanguageKey, language);

            // Trigger language changed event if different
            if (previousLanguage != language)
            {
                LanguageChanged?.Invoke(this, new LanguageChangedEventArgs
                {
                    Language = language,
                    FlowDirection = GetFlowDirection()
                });
            }
        }

        public async Task<string> GetCurrentLanguageAsync()
        {
            if (string.IsNullOrEmpty(_currentLanguage))
            {
                _currentLanguage = await _preferencesService.GetValueAsync(LanguageKey);

                // Default to Arabic if not set
                if (string.IsNullOrEmpty(_currentLanguage))
                {
                    _currentLanguage = "ar";
                    await _preferencesService.SaveValueAsync(LanguageKey, _currentLanguage);
                }

                // Set the current culture
                CultureInfo culture = new CultureInfo(_currentLanguage);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            return _currentLanguage;
        }

        public string GetLocalizedString(string key)
        {
            if (string.IsNullOrEmpty(_currentLanguage))
            {
                // Default to "ar" if not initialized yet
                _currentLanguage = "ar";
            }

            if (_localizedStrings.TryGetValue(_currentLanguage, out var strings))
            {
                if (strings.TryGetValue(key, out var value))
                {
                    return value;
                }
            }

            return key; // Return the key itself if not found
        }

        public FlowDirection GetFlowDirection()
        {
            return _currentLanguage == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
    }
}