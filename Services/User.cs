using API.Services;
using Data.Structure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Services
{
    public interface IUserService
    {
        /// <summary>
        /// تسجيل مستخدم جديد
        /// </summary>
        Task<BaseResponse<UserInfo>> RegisterUserAsync(RegisterUserRequestDTO request, string language);

        /// <summary>
        /// تسجيل الدخول باستخدام اسم المستخدم وكلمة المرور
        /// </summary>
        Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequestDTO request, string language);

        /// <summary>
        /// تسجيل الدخول باستخدام رقم الهاتف وكلمة المرور
        /// </summary>
        Task<BaseResponse<LoginResponse>> LoginWithPhoneAsync(LoginWithPhoneRequestDTO request, string language);

        /// <summary>
        /// الحصول على معلومات المستخدم
        /// </summary>
        Task<BaseResponse<UserInfo>> GetUserProfileAsync(long userId, string language);

        /// <summary>
        /// تحديث معلومات المستخدم
        /// </summary>
        Task<BaseResponse<UserInfo>> UpdateUserProfileAsync(long userId, UpdateUserProfileRequestDTO request, string language);

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        Task<BaseResponse<bool>> ChangePasswordAsync(long userId, ChangePasswordRequestDTO request, string language);

        /// <summary>
        /// إعادة تعيين كلمة المرور (نسيان كلمة المرور)
        /// </summary>
        Task<BaseResponse<bool>> ResetPasswordAsync(ResetPasswordRequestDTO request, string language);

        /// <summary>
        /// تفعيل حساب المستخدم
        /// </summary>
        Task<BaseResponse<bool>> ActivateUserAsync(long userId, string language);

        /// <summary>
        /// تعطيل حساب المستخدم
        /// </summary>
        Task<BaseResponse<bool>> DeactivateUserAsync(long userId, string language);

        /// <summary>
        /// إضافة سجل نشاط للمستخدم
        /// </summary>
        Task<BaseResponse<bool>> AddUserActivityLogAsync(long userId, string activityType, string description, string ipAddress, string userAgent, string language);
    }

    public class UserService : IUserService
    {
        private readonly MuhamiContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IJwtService _jwtService;

        public UserService(
            MuhamiContext context,
            IConfiguration configuration,
            ILogger<UserService> logger,
            ILocalizationService localizationService,
            IJwtService jwtService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// تسجيل مستخدم جديد
        /// </summary>
        public async Task<BaseResponse<UserInfo>> RegisterUserAsync(RegisterUserRequestDTO request, string language)
        {
            try
            {
                // التحقق من المدخلات
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.FullName))
                {
                    var missingFieldsMessage = _localizationService.GetMessage("MissingRequiredFields", "Errors", language);
                    return BaseResponse<UserInfo>.FailureResponse(missingFieldsMessage, 400);
                }

                // التحقق من صحة البريد الإلكتروني
                if (!IsValidEmail(request.Email))
                {
                    var invalidEmailMessage = _localizationService.GetMessage("InvalidEmail", "Errors", language);
                    return BaseResponse<UserInfo>.FailureResponse(invalidEmailMessage, 400);
                }

                // التحقق من صحة رقم الهاتف (إذا تم تقديمه)
                if (request.PhoneNumber.HasValue && !IsValidPhoneNumber(request.PhoneNumber.Value.ToString()))
                {
                    var invalidPhoneMessage = _localizationService.GetMessage("InvalidPhoneNumber", "Errors", language);
                    return BaseResponse<UserInfo>.FailureResponse(invalidPhoneMessage, 400);
                }

                // التحقق من قوة كلمة المرور
                if (!IsPasswordStrong(request.Password))
                {
                    var weakPasswordMessage = _localizationService.GetMessage("WeakPassword", "Errors", language);
                    return BaseResponse<UserInfo>.FailureResponse(weakPasswordMessage, 400);
                }

                // التحقق من عدم وجود اسم المستخدم بالفعل
                if (await _context.Users.AnyAsync(u => u.Username == request.Username && u.IsDeleted != true))
                {
                    var usernameExistsMessage = _localizationService.GetMessage("UsernameExists", "Errors", language);
                    return BaseResponse<UserInfo>.FailureResponse(usernameExistsMessage, 400);
                }

                // التحقق من عدم وجود البريد الإلكتروني بالفعل
                if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.IsDeleted != true))
                {
                    var emailExistsMessage = _localizationService.GetMessage("EmailExists", "Errors", language);
                    return BaseResponse<UserInfo>.FailureResponse(emailExistsMessage, 400);
                }

                // التحقق من عدم وجود رقم الهاتف بالفعل (إذا تم تقديمه)
                if (request.PhoneNumber.HasValue &&
                    await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.IsDeleted != true))
                {
                    var phoneExistsMessage = _localizationService.GetMessage("PhoneNumberExists", "Errors", language);
                    return BaseResponse<UserInfo>.FailureResponse(phoneExistsMessage, 400);
                }

                // إنشاء مستخدم جديد
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = HashPassword(request.Password),
                    UserRole = "User", // الدور الافتراضي هو "مستخدم"
                    IsActive = true,
                    CreatedByUserId = 1, // يمكن تعديله لاستخدام معرف النظام
                    CreateDate = DateTime.UtcNow
                };

                // إضافة المستخدم الجديد إلى قاعدة البيانات
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(user.Id, "Register", "تسجيل مستخدم جديد", request.IpAddress, request.UserAgent);

                // تحويل البيانات إلى النموذج المطلوب للمستخدم
                var userInfo = new UserInfo
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = new List<string> { user.UserRole }
                };

                var successMessage = _localizationService.GetMessage("UserRegisteredSuccess", "Messages", language);
                return BaseResponse<UserInfo>.SuccessResponse(userInfo, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تسجيل مستخدم جديد");
                var errorMessage = _localizationService.GetMessage("UserRegistrationError", "Errors", language);
                return BaseResponse<UserInfo>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تسجيل الدخول باستخدام اسم المستخدم وكلمة المرور
        /// </summary>
        public async Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequestDTO request, string language)
        {
            try
            {
                // التحقق من المدخلات
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    var missingFieldsMessage = _localizationService.GetMessage("MissingCredentials", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(missingFieldsMessage, 400);
                }

                // البحث عن المستخدم باستخدام اسم المستخدم
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsDeleted != true);

                if (user == null)
                {
                    var invalidCredentialsMessage = _localizationService.GetMessage("InvalidCredentials", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(invalidCredentialsMessage, 401);
                }

                // التحقق من أن الحساب نشط
                if (!user.IsActive)
                {
                    var accountDisabledMessage = _localizationService.GetMessage("AccountDisabled", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(accountDisabledMessage, 401);
                }

                // التحقق من صحة كلمة المرور
                if (!VerifyPassword(request.Password, user.PasswordHash))
                {
                    var invalidCredentialsMessage = _localizationService.GetMessage("InvalidCredentials", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(invalidCredentialsMessage, 401);
                }

                // تحديث وقت آخر تسجيل دخول
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(user.Id, "Login", "تسجيل دخول", request.IpAddress, request.UserAgent);

                // إنشاء معلومات المستخدم للرمز
                var userInfo = new UserInfo
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = new List<string> { user.UserRole }
                };

                // إنشاء رمز JWT
                var loginResponse = _jwtService.GenerateJwtToken(userInfo);

                // تخزين رمز التحديث في قاعدة البيانات
                var refreshTokenEntity = new RefreshToken
                {
                    Token = loginResponse.RefreshToken,
                    ExpiresAt = loginResponse.ExpiresAt.AddDays(7), // 7 أيام بعد انتهاء رمز JWT
                    UserId = user.Id,
                    CreatedByUserId = user.Id,
                    CreateDate = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("LoginSuccess", "Messages", language);
                return BaseResponse<LoginResponse>.SuccessResponse(loginResponse, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تسجيل الدخول");
                var errorMessage = _localizationService.GetMessage("LoginError", "Errors", language);
                return BaseResponse<LoginResponse>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تسجيل الدخول باستخدام رقم الهاتف وكلمة المرور
        /// </summary>
        public async Task<BaseResponse<LoginResponse>> LoginWithPhoneAsync(LoginWithPhoneRequestDTO request, string language)
        {
            try
            {
                // التحقق من المدخلات
                if (!request.PhoneNumber.HasValue || string.IsNullOrWhiteSpace(request.Password))
                {
                    var missingFieldsMessage = _localizationService.GetMessage("MissingCredentials", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(missingFieldsMessage, 400);
                }

                // البحث عن المستخدم باستخدام رقم الهاتف
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber && u.IsDeleted != true);

                if (user == null)
                {
                    var invalidCredentialsMessage = _localizationService.GetMessage("InvalidCredentials", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(invalidCredentialsMessage, 401);
                }

                // التحقق من أن الحساب نشط
                if (!user.IsActive)
                {
                    var accountDisabledMessage = _localizationService.GetMessage("AccountDisabled", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(accountDisabledMessage, 401);
                }

                // التحقق من صحة كلمة المرور
                if (!VerifyPassword(request.Password, user.PasswordHash))
                {
                    var invalidCredentialsMessage = _localizationService.GetMessage("InvalidCredentials", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(invalidCredentialsMessage, 401);
                }

                // تحديث وقت آخر تسجيل دخول
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(user.Id, "LoginWithPhone", "تسجيل دخول برقم الهاتف", request.IpAddress, request.UserAgent);

                // إنشاء معلومات المستخدم للرمز
                var userInfo = new UserInfo
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = new List<string> { user.UserRole }
                };

                // إنشاء رمز JWT
                var loginResponse = _jwtService.GenerateJwtToken(userInfo);

                // تخزين رمز التحديث في قاعدة البيانات
                var refreshTokenEntity = new RefreshToken
                {
                    Token = loginResponse.RefreshToken,
                    ExpiresAt = loginResponse.ExpiresAt.AddDays(7), // 7 أيام بعد انتهاء رمز JWT
                    UserId = user.Id,
                    CreatedByUserId = user.Id,
                    CreateDate = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("LoginSuccess", "Messages", language);
                return BaseResponse<LoginResponse>.SuccessResponse(loginResponse, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تسجيل الدخول برقم الهاتف");
                var errorMessage = _localizationService.GetMessage("LoginError", "Errors", language);
                return BaseResponse<LoginResponse>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على معلومات المستخدم
        /// </summary>
        public async Task<BaseResponse<UserInfo>> GetUserProfileAsync(long userId, string language)
        {
            try
            {
                // البحث عن المستخدم
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted != true);

                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<UserInfo>.FailureResponse(userNotFoundMessage, 404);
                }

                // تحويل البيانات إلى النموذج المطلوب للمستخدم
                var userInfo = new UserInfo
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = new List<string> { user.UserRole }
                };

                return BaseResponse<UserInfo>.SuccessResponse(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على معلومات المستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("UserProfileRetrievalError", "Errors", language);
                return BaseResponse<UserInfo>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث معلومات المستخدم
        /// </summary>
        public async Task<BaseResponse<UserInfo>> UpdateUserProfileAsync(long userId, UpdateUserProfileRequestDTO request, string language)
        {
            try
            {
                // البحث عن المستخدم
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted != true);

                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<UserInfo>.FailureResponse(userNotFoundMessage, 404);
                }

                // التحقق من صحة البريد الإلكتروني الجديد (إذا تم تقديمه)
                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
                {
                    if (!IsValidEmail(request.Email))
                    {
                        var invalidEmailMessage = _localizationService.GetMessage("InvalidEmail", "Errors", language);
                        return BaseResponse<UserInfo>.FailureResponse(invalidEmailMessage, 400);
                    }

                    // التحقق من عدم وجود البريد الإلكتروني بالفعل لمستخدم آخر
                    if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != userId && u.IsDeleted != true))
                    {
                        var emailExistsMessage = _localizationService.GetMessage("EmailExists", "Errors", language);
                        return BaseResponse<UserInfo>.FailureResponse(emailExistsMessage, 400);
                    }

                    user.Email = request.Email;
                }

                // التحقق من صحة رقم الهاتف الجديد (إذا تم تقديمه)
                if (request.PhoneNumber.HasValue && request.PhoneNumber != user.PhoneNumber)
                {
                    if (!IsValidPhoneNumber(request.PhoneNumber.Value.ToString()))
                    {
                        var invalidPhoneMessage = _localizationService.GetMessage("InvalidPhoneNumber", "Errors", language);
                        return BaseResponse<UserInfo>.FailureResponse(invalidPhoneMessage, 400);
                    }

                    // التحقق من عدم وجود رقم الهاتف بالفعل لمستخدم آخر
                    if (await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId && u.IsDeleted != true))
                    {
                        var phoneExistsMessage = _localizationService.GetMessage("PhoneNumberExists", "Errors", language);
                        return BaseResponse<UserInfo>.FailureResponse(phoneExistsMessage, 400);
                    }

                    user.PhoneNumber = request.PhoneNumber;
                }

                // تحديث الاسم الكامل (إذا تم تقديمه)
                if (!string.IsNullOrWhiteSpace(request.FullName))
                {
                    user.FullName = request.FullName;
                }

                // تحديث معلومات التعديل
                user.ModifiedByUserId = userId;
                user.ModifiedDate = DateTime.UtcNow;

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(userId, "UpdateProfile", "تحديث الملف الشخصي", request.IpAddress, request.UserAgent);

                // تحويل البيانات إلى النموذج المطلوب للمستخدم
                var userInfo = new UserInfo
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = new List<string> { user.UserRole }
                };

                var successMessage = _localizationService.GetMessage("UserProfileUpdated", "Messages", language);
                return BaseResponse<UserInfo>.SuccessResponse(userInfo, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث معلومات المستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("UserProfileUpdateError", "Errors", language);
                return BaseResponse<UserInfo>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        public async Task<BaseResponse<bool>> ChangePasswordAsync(long userId, ChangePasswordRequestDTO request, string language)
        {
            try
            {
                // التحقق من المدخلات
                if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                    string.IsNullOrWhiteSpace(request.NewPassword) ||
                    string.IsNullOrWhiteSpace(request.ConfirmPassword))
                {
                    var missingFieldsMessage = _localizationService.GetMessage("MissingPasswordFields", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(missingFieldsMessage, 400);
                }

                // التحقق من تطابق كلمة المرور الجديدة مع التأكيد
                if (request.NewPassword != request.ConfirmPassword)
                {
                    var passwordMismatchMessage = _localizationService.GetMessage("PasswordMismatch", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(passwordMismatchMessage, 400);
                }

                // التحقق من قوة كلمة المرور الجديدة
                if (!IsPasswordStrong(request.NewPassword))
                {
                    var weakPasswordMessage = _localizationService.GetMessage("WeakPassword", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(weakPasswordMessage, 400);
                }

                // البحث عن المستخدم
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted != true);

                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(userNotFoundMessage, 404);
                }

                // التحقق من صحة كلمة المرور الحالية
                if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
                {
                    var invalidPasswordMessage = _localizationService.GetMessage("InvalidCurrentPassword", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(invalidPasswordMessage, 400);
                }

                // تحديث كلمة المرور
                user.PasswordHash = HashPassword(request.NewPassword);
                user.ModifiedByUserId = userId;
                user.ModifiedDate = DateTime.UtcNow;

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(userId, "ChangePassword", "تغيير كلمة المرور", request.IpAddress, request.UserAgent);

                // إلغاء جميع رموز التحديث للمستخدم
                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.IsDeleted != true)
                    .ToListAsync();

                foreach (var token in refreshTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    token.ModifiedByUserId = userId;
                    token.ModifiedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("PasswordChanged", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تغيير كلمة المرور للمستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("PasswordChangeError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إعادة تعيين كلمة المرور (نسيان كلمة المرور)
        /// </summary>
        public async Task<BaseResponse<bool>> ResetPasswordAsync(ResetPasswordRequestDTO request, string language)
        {
            try
            {
                // التحقق من المدخلات
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.ResetCode) ||
                    string.IsNullOrWhiteSpace(request.NewPassword) ||
                    string.IsNullOrWhiteSpace(request.ConfirmPassword))
                {
                    var missingFieldsMessage = _localizationService.GetMessage("MissingResetPasswordFields", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(missingFieldsMessage, 400);
                }

                // التحقق من تطابق كلمة المرور الجديدة مع التأكيد
                if (request.NewPassword != request.ConfirmPassword)
                {
                    var passwordMismatchMessage = _localizationService.GetMessage("PasswordMismatch", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(passwordMismatchMessage, 400);
                }

                // التحقق من قوة كلمة المرور الجديدة
                if (!IsPasswordStrong(request.NewPassword))
                {
                    var weakPasswordMessage = _localizationService.GetMessage("WeakPassword", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(weakPasswordMessage, 400);
                }

                // البحث عن المستخدم باستخدام البريد الإلكتروني
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsDeleted != true);

                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(userNotFoundMessage, 404);
                }

                // في بيئة حقيقية، ستتحقق من رمز إعادة التعيين المخزن في قاعدة البيانات أو المرسل بالبريد الإلكتروني
                // هنا نفترض أن رمز إعادة التعيين "123456" صالح للتجربة فقط
                if (request.ResetCode != "123456")
                {
                    var invalidResetCodeMessage = _localizationService.GetMessage("InvalidResetCode", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(invalidResetCodeMessage, 400);
                }

                // تحديث كلمة المرور
                user.PasswordHash = HashPassword(request.NewPassword);
                user.ModifiedByUserId = user.Id;
                user.ModifiedDate = DateTime.UtcNow;

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(user.Id, "ResetPassword", "إعادة تعيين كلمة المرور", request.IpAddress, request.UserAgent);

                // إلغاء جميع رموز التحديث للمستخدم
                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null && rt.IsDeleted != true)
                    .ToListAsync();

                foreach (var token in refreshTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    token.ModifiedByUserId = user.Id;
                    token.ModifiedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("PasswordReset", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إعادة تعيين كلمة المرور");
                var errorMessage = _localizationService.GetMessage("PasswordResetError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تفعيل حساب المستخدم
        /// </summary>
        public async Task<BaseResponse<bool>> ActivateUserAsync(long userId, string language)
        {
            try
            {
                // البحث عن المستخدم
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted != true);

                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(userNotFoundMessage, 404);
                }

                // تغيير حالة التفعيل
                user.IsActive = true;
                user.ModifiedByUserId = userId;
                user.ModifiedDate = DateTime.UtcNow;

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("UserActivated", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تفعيل حساب المستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("UserActivationError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تعطيل حساب المستخدم
        /// </summary>
        public async Task<BaseResponse<bool>> DeactivateUserAsync(long userId, string language)
        {
            try
            {
                // البحث عن المستخدم
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted != true);

                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(userNotFoundMessage, 404);
                }

                // تغيير حالة التفعيل
                user.IsActive = false;
                user.ModifiedByUserId = userId;
                user.ModifiedDate = DateTime.UtcNow;

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // إلغاء جميع رموز التحديث للمستخدم
                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.IsDeleted != true)
                    .ToListAsync();

                foreach (var token in refreshTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    token.ModifiedByUserId = userId;
                    token.ModifiedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("UserDeactivated", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تعطيل حساب المستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("UserDeactivationError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إضافة سجل نشاط للمستخدم
        /// </summary>
        public async Task<BaseResponse<bool>> AddUserActivityLogAsync(long userId, string activityType, string description, string ipAddress, string userAgent, string language)
        {
            try
            {
                await AddUserActivityLogInternalAsync(userId, activityType, description, ipAddress, userAgent);
                return BaseResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إضافة سجل نشاط للمستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("ActivityLogError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        #region Helper Methods

        /// <summary>
        /// إضافة سجل نشاط للمستخدم (استخدام داخلي)
        /// </summary>
        private async Task AddUserActivityLogInternalAsync(long userId, string activityType, string description, string? ipAddress = null, string? userAgent = null)
        {
            var activityLog = new UserActivityLog
            {
                UserId = userId,
                ActivityType = activityType,
                Description = description,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };

            _context.UserActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// تشفير كلمة المرور
        /// </summary>
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        /// <summary>
        /// التحقق من صحة كلمة المرور
        /// </summary>
        private bool VerifyPassword(string password, string passwordHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == passwordHash;
        }

        /// <summary>
        /// التحقق من صحة البريد الإلكتروني
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// التحقق من صحة رقم الهاتف
        /// </summary>
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            try
            {
                // يتم قبول الأرقام فقط بطول 8 أرقام على الأقل
                var regex = new Regex(@"^\d{8,}$");
                return regex.IsMatch(phoneNumber);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// التحقق من قوة كلمة المرور
        /// </summary>
        private bool IsPasswordStrong(string password)
        {
            // كلمة المرور يجب أن تكون 8 أحرف على الأقل وتحتوي على حرف كبير، حرف صغير، رقم وحرف خاص
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
            return regex.IsMatch(password);
        }

        #endregion
    }
}
