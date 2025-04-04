using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Types;

namespace Services
{
    /// <summary>
    /// واجهة خدمة الرسائل النصية القصيرة
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// التحقق من صحة رمز التأكيد
        /// </summary>
        /// <param name="phoneNumber">رقم الهاتف</param>
        /// <param name="confirmationCode">رمز التأكيد</param>
        /// <returns>صحة الرمز</returns>
        Task<bool> VerifyConfirmationCode(long phoneNumber, string confirmationCode);

        /// <summary>
        /// إرسال رسالة نصية قصيرة برمز تأكيد
        /// </summary>
        /// <param name="phoneNumber">رقم الهاتف</param>
        /// <returns>رمز التأكيد المُرسل</returns>
        Task<string> SendOTP(long phoneNumber);

        /// <summary>
        /// إرسال رسالة نصية قصيرة مخصصة
        /// </summary>
        /// <param name="phoneNumber">رقم الهاتف</param>
        /// <param name="message">نص الرسالة</param>
        /// <returns>نتيجة الإرسال</returns>
        Task<bool> SendCustomSmsAsync(long phoneNumber, string message);
    }

    /// <summary>
    /// خدمة الرسائل النصية القصيرة باستخدام Twilio
    /// </summary>
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly MuhamiContext _context;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;
        private readonly string _verifyServiceSid;

        /// <summary>
        /// إنشاء نسخة جديدة من خدمة الرسائل النصية
        /// </summary>
        public SmsService(ILogger<SmsService> logger, IConfiguration configuration, MuhamiContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;

            // الحصول على بيانات Twilio من التكوين
            _accountSid = _configuration["Twilio:AccountSid"];
            _authToken = _configuration["Twilio:AuthToken"];
            _fromNumber = _configuration["Twilio:FromNumber"];
            _verifyServiceSid = _configuration["Twilio:VerificationServiceSid"];

            // تهيئة Twilio إذا كانت كل البيانات المطلوبة متوفرة
            if (!string.IsNullOrEmpty(_accountSid) && !string.IsNullOrEmpty(_authToken))
            {
                TwilioClient.Init(_accountSid, _authToken);
            }
            else
            {
                _logger.LogWarning("Twilio configuration is missing or incomplete. SMS functionality will not work properly.");
            }
        }

        /// <summary>
        /// إرسال رسالة نصية قصيرة برمز تأكيد باستخدام خدمة Twilio Verify
        /// </summary>
        public async Task<string> SendOTP(long phoneNumber)
        {
            try
            {
                _logger.LogInformation("Sending OTP to phone number {PhoneNumber}", phoneNumber);

                // تنسيق رقم الهاتف بالنموذج الدولي
                string formattedPhoneNumber = FormatPhoneNumber(phoneNumber.ToString());

                // إرسال رمز التحقق باستخدام خدمة Twilio Verify
                var verification = await VerificationResource.CreateAsync(
                    to: formattedPhoneNumber,
                    channel: "sms",
                    pathServiceSid: _verifyServiceSid
                );

                if (verification.Status == "pending")
                {
                    _logger.LogInformation("Verification sent to {PhoneNumber}, status: {Status}", phoneNumber, verification.Status);
                    
                    // لا نقوم بإنشاء أو تخزين رمز OTP بعد الآن لأن Twilio Verify تتعامل مع الأمر
                    // ولكن يمكننا تسجيل محاولة التحقق في قاعدة البيانات الخاصة بنا
                    var otp = new SmsOtpCode
                    {
                        PhoneNumber = phoneNumber,
                        Code = "twilio-verify", // لا نملك الرمز الفعلي لأن Twilio تتعامل معه
                        ExpirationTime = DateTime.UtcNow.AddMinutes(10), // صلاحية الرمز 10 دقائق (قابل للتعديل)
                        IsUsed = false,
                        CreatedByUserId = 1, // يمكن تغييره حسب حالة الاستخدام
                        CreateDate = DateTime.UtcNow
                    };

                    // حفظ سجل محاولة التحقق في قاعدة البيانات
                    _context.SmsOtpCodes.Add(otp);
                    await _context.SaveChangesAsync();

                    return "Code sent successfully"; // لا نستطيع إرجاع الرمز الفعلي
                }
                else
                {
                    _logger.LogWarning("Failed to send verification to {PhoneNumber}, status: {Status}", phoneNumber, verification.Status);
                    return "Failed to send verification code";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to phone number {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        /// <summary>
        /// إرسال رسالة نصية مخصصة
        /// </summary>
        public async Task<bool> SendCustomSmsAsync(long phoneNumber, string message)
        {
            try
            {
                // تنسيق رقم الهاتف
                string formattedPhoneNumber = FormatPhoneNumber(phoneNumber.ToString());

                // إرسال الرسالة المخصصة باستخدام Twilio
                var messageSent = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_fromNumber),
                    to: new PhoneNumber(formattedPhoneNumber)
                );

                // التحقق من حالة الإرسال
                bool success = messageSent.Status == MessageResource.StatusEnum.Queued || 
                              messageSent.Status == MessageResource.StatusEnum.Sent || 
                              messageSent.Status == MessageResource.StatusEnum.Delivered;
                
                if (success)
                {
                    _logger.LogInformation("SMS sent successfully to {PhoneNumber}, Twilio SID: {MessageSid}", phoneNumber, messageSent.Sid);
                }
                else
                {
                    _logger.LogWarning("SMS to {PhoneNumber} has status: {Status}, Twilio SID: {MessageSid}", 
                        phoneNumber, messageSent.Status, messageSent.Sid);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending custom SMS to phone number {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        /// <summary>
        /// التحقق من صحة رمز التأكيد باستخدام خدمة Twilio Verify
        /// </summary>
        public async Task<bool> VerifyConfirmationCode(long phoneNumber, string confirmationCode)
        {
            try
            {
                if (string.IsNullOrEmpty(confirmationCode))
                {
                    _logger.LogWarning("Confirmation code is null or empty for phone number {PhoneNumber}", phoneNumber);
                    return false;
                }

                // تنسيق رقم الهاتف بالنموذج الدولي
                string formattedPhoneNumber = FormatPhoneNumber(phoneNumber.ToString());

                // التحقق من صحة الرمز باستخدام خدمة Twilio Verify
                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: formattedPhoneNumber,
                    code: confirmationCode,
                    pathServiceSid: _verifyServiceSid
                );

                bool isValid = verificationCheck.Status == "approved";

                if (isValid)
                {
                    // تحديث حالة الرمز في قاعدة البيانات الخاصة بنا
                    var otpRecord = await _context.SmsOtpCodes
                        .Where(o => o.PhoneNumber == phoneNumber && !o.IsUsed && !o.IsDeleted)
                        .OrderByDescending(o => o.CreateDate)
                        .FirstOrDefaultAsync();

                    if (otpRecord != null)
                    {
                        otpRecord.IsUsed = true;
                        otpRecord.ModifiedDate = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }

                    _logger.LogInformation("Successfully verified OTP code for phone number {PhoneNumber}", phoneNumber);
                }
                else
                {
                    _logger.LogWarning("Invalid OTP code provided for phone number {PhoneNumber}, status: {Status}", 
                        phoneNumber, verificationCheck.Status);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying confirmation code for phone number {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        /// <summary>
        /// تنسيق رقم الهاتف بالصيغة الدولية
        /// </summary>
        private string FormatPhoneNumber(string phoneNumber)
        {
            // إزالة أي مسافات أو أحرف خاصة
            phoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // التأكد من أن الرقم يبدأ بـ + إذا لم يكن كذلك
            if (!phoneNumber.StartsWith("+"))
            {
                // إذا كان الرقم لا يتضمن رمز الدولة، إضافة رمز عمان (+968)
                if (phoneNumber.Length == 8)
                {
                    phoneNumber = "+968" + phoneNumber;
                }
                else
                {
                    phoneNumber = "+" + phoneNumber;
                }
            }

            return phoneNumber;
        }
    }
}

