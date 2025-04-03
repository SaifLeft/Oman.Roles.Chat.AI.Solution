using AutoMapper;
using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Models.Common;
using Models.DTOs.Authorization;
using Services.Common;
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
        Task<BaseResponse<UserDTO>> RegisterUserAsync(RegisterUserRequestDTO request, string language);

        /// <summary>
        /// تسجيل الدخول باستخدام رقم الهاتف
        /// </summary>
        Task<BaseResponse<LoginResponse>> RegisterWithPhoneAsync(UserPhoneRegistrationDTO registrationDto, string language);

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
        Task<BaseResponse<UserDTO>> GetUserProfileAsync(long userId, string language);

        /// <summary>
        /// تحديث معلومات المستخدم
        /// </summary>
        Task<BaseResponse<UserDTO>> UpdateUserProfileAsync(long userId, UpdateUserProfileRequestDTO request, string language);

        /// <summary>
        /// تحديث معلومات المستخدم (للمسؤول)
        /// </summary>
        Task<BaseResponse<UserDTO>> AdminUpdateUserAsync(long userId, AdminUpdateUserRequestDTO request, long adminId, string language);

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        Task<BaseResponse<bool>> ChangePasswordAsync(long userId, ChangePasswordRequestDTO request, string language);

        /// <summary>
        /// إعادة تعيين كلمة المرور
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
        /// الحصول على جميع المستخدمين
        /// </summary>
        Task<PaginatedResponse<List<UserDTO>>> GetAllUsersAsync(int page, int pageSize, string language);

        /// <summary>
        /// إضافة سجل نشاط للمستخدم
        /// </summary>
        Task<BaseResponse<bool>> AddUserActivityLogAsync(long userId, string activityType, string description, string ipAddress, string userAgent, string language);

        /// <summary>
        /// الحصول على أو إنشاء مستخدم جوجل
        /// </summary>
        Task<User> GetOrCreateGoogleUserAsync(GoogleTokenValidationResult validationResult, string language);
    }

    public class UserService : IUserService
    {
        private readonly MuhamiContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ISmsService _smsService;

        public UserService(
            MuhamiContext context,
            IConfiguration configuration,
            ILogger<UserService> logger,
            ILocalizationService localizationService,
            IJwtService jwtService,
            IMapper mapper,
            ISmsService smsService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;
            _jwtService = jwtService;
            _mapper = mapper;
            _smsService = smsService;
        }

        /// <summary>
        /// تسجيل مستخدم جديد
        /// </summary>
        public async Task<BaseResponse<UserDTO>> RegisterUserAsync(RegisterUserRequestDTO request, string language)
        {
            try
            {
                // التحقق من صحة البريد الإلكتروني
                if (!IsValidEmail(request.Email))
                {
                    var invalidEmailMessage = _localizationService.GetMessage("InvalidEmail", "Errors", language);
                    return BaseResponse<UserDTO>.FailureResponse(invalidEmailMessage, 400);
                }

                // التحقق من صحة رقم الهاتف (إذا تم تقديمه)
                if (request.PhoneNumber.HasValue && !IsValidPhoneNumber(request.PhoneNumber.Value.ToString()))
                {
                    var invalidPhoneMessage = _localizationService.GetMessage("InvalidPhoneNumber", "Errors", language);
                    return BaseResponse<UserDTO>.FailureResponse(invalidPhoneMessage, 400);
                }

                // التحقق من قوة كلمة المرور
                if (!IsPasswordStrong(request.Password))
                {
                    var weakPasswordMessage = _localizationService.GetMessage("WeakPassword", "Errors", language);
                    return BaseResponse<UserDTO>.FailureResponse(weakPasswordMessage, 400);
                }

                // التحقق من عدم وجود اسم المستخدم بالفعل
                if (await _context.Users.AnyAsync(u => u.Username == request.Username && u.IsDeleted != true))
                {
                    var usernameExistsMessage = _localizationService.GetMessage("UsernameExists", "Errors", language);
                    return BaseResponse<UserDTO>.FailureResponse(usernameExistsMessage, 400);
                }

                // التحقق من عدم وجود البريد الإلكتروني بالفعل
                if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.IsDeleted != true))
                {
                    var emailExistsMessage = _localizationService.GetMessage("EmailExists", "Errors", language);
                    return BaseResponse<UserDTO>.FailureResponse(emailExistsMessage, 400);
                }

                // التحقق من عدم وجود رقم الهاتف بالفعل (إذا تم تقديمه)
                if (request.PhoneNumber.HasValue &&
                    await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.IsDeleted != true))
                {
                    var phoneExistsMessage = _localizationService.GetMessage("PhoneNumberExists", "Errors", language);
                    return BaseResponse<UserDTO>.FailureResponse(phoneExistsMessage, 400);
                }

                // إنشاء مستخدم جديد
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = HashPassword(request.Password),
                    UserRole = nameof(UserRole.USER), // الدور الافتراضي هو "مستخدم"
                    IsActive = true,
                    CreatedByUserId = 1, // يمكن تعديله لاستخدام معرف النظام
                    CreateDate = DateTime.Now,
                };

                // إضافة المستخدم الجديد إلى قاعدة البيانات
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(user.Id, "Register", "تسجيل مستخدم جديد", request.IpAddress, request.UserAgent);

                // تحويل البيانات إلى النموذج المطلوب للمستخدم
                var userInfo = new UserDTO
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = new List<string> { user.UserRole }
                };

                var successMessage = _localizationService.GetMessage("UserRegisteredSuccess", "Messages", language);
                return BaseResponse<UserDTO>.SuccessResponse(userInfo, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تسجيل مستخدم جديد");
                var errorMessage = _localizationService.GetMessage("UserRegistrationError", "Errors", language);
                return BaseResponse<UserDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تسجيل مستخدم جديد عبر الهاتف
        /// </summary>
        public async Task<BaseResponse<LoginResponse>> RegisterWithPhoneAsync(UserPhoneRegistrationDTO registrationDto, string language)
        {
            try
            {
                // Validate confirmation code
                if (!await _smsService.VerifyConfirmationCode(registrationDto.PhoneNumber, registrationDto.ConfirmationCode))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidConfirmationCode", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(errorMessage, 400);
                }

                // Check if phone number already exists
                if (await _context.Users.AnyAsync(u => u.PhoneNumber == registrationDto.PhoneNumber && u.IsDeleted != true))
                {
                    var errorMessage = _localizationService.GetMessage("PhoneNumberExists", "Errors", language);
                    return BaseResponse<LoginResponse>.FailureResponse(errorMessage, 400);
                }

                // Create new user
                var user = new User
                {
                    Username = $"user_{registrationDto.PhoneNumber}", // Generate username from phone
                    PhoneNumber = registrationDto.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    PasswordHash = HashPassword(registrationDto.Password),
                    UserRole = nameof(UserRole.USER),
                    IsActive = true,
                    CreatedByUserId = 1, // System user
                    CreateDate = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Add activity log
                await AddUserActivityLogInternalAsync(user.Id, "RegisterWithPhone", "تسجيل مستخدم جديد باستخدام رقم الهاتف", null, null);

                // Generate JWT token
                var token = await _jwtService.GenerateJwtToken(user.Id.ToString());

                var successMessage = _localizationService.GetMessage("RegistrationSuccess", "Messages", language);
                return BaseResponse<LoginResponse>.SuccessResponse(token, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تسجيل مستخدم جديد باستخدام رقم الهاتف {PhoneNumber}", registrationDto.PhoneNumber);
                var errorMessage = _localizationService.GetMessage("UserRegistrationError", "Errors", language);
                return BaseResponse<LoginResponse>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على أو إنشاء مستخدم جوجل
        /// </summary>
        public async Task<User> GetOrCreateGoogleUserAsync(GoogleTokenValidationResult validationResult, string language)
        {
            try
            {
                // البحث عن المستخدم بمعرف Google أو البريد الإلكتروني
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        (u.GoogleId == validationResult.Subject || u.Email == validationResult.Email) &&
                        u.IsDeleted != true);

                if (existingUser != null)
                {
                    return existingUser;
                }

                // إنشاء مستخدم جديد
                var newUser = new User
                {
                    Username = $"google_{Guid.NewGuid().ToString().Substring(0, 8)}", // إنشاء اسم مستخدم فريد
                    Email = validationResult.Email,
                    FirstName = validationResult.Name,
                    GoogleId = validationResult.Subject,
                    IsEmailConfirmed = validationResult.EmailVerified,
                    IsActive = true,
                    UserRole = nameof(UserRole.USER),
                    CreatedByUserId = 1, // النظام
                    CreateDate = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // إضافة سجل نشاط
                await AddUserActivityLogInternalAsync(newUser.Id, "GoogleAuth", "تسجيل مستخدم جديد باستخدام Google", null, null);

                return newUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء معالجة مستخدم Google");
                throw;
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
                user.LastLoginAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(user.Id, "Login", "تسجيل دخول", request.IpAddress, request.UserAgent);

                // إنشاء رمز JWT
                var loginResponse = await _jwtService.GenerateJwtToken(user.Id.ToString());

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
                user.LastLoginAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(user.Id, "LoginWithPhone", "تسجيل دخول برقم الهاتف", request.IpAddress, request.UserAgent);

                // إنشاء رمز JWT
                var loginResponse = await _jwtService.GenerateJwtToken(user.Id.ToString());

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
        public async Task<BaseResponse<UserDTO>> GetUserProfileAsync(long userId, string language)
        {
            try
            {
                // البحث عن المستخدم
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted != true);

                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<UserDTO>.FailureResponse(userNotFoundMessage, 404);
                }

                // تحويل البيانات إلى النموذج المطلوب للمستخدم
                var userInfo = new UserDTO
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,

                    Roles = new List<string> { user.UserRole }
                };

                return BaseResponse<UserDTO>.SuccessResponse(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على معلومات المستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("UserProfileRetrievalError", "Errors", language);
                return BaseResponse<UserDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث معلومات المستخدم
        /// </summary>
        public async Task<BaseResponse<UserDTO>> UpdateUserProfileAsync(long userId, UpdateUserProfileRequestDTO request, string language)
        {
            try
            {
                // البحث عن المستخدم
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted != true);

                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<UserDTO>.FailureResponse(userNotFoundMessage, 404);
                }

                // التحقق من صحة البريد الإلكتروني الجديد (إذا تم تقديمه)
                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
                {
                    if (!IsValidEmail(request.Email))
                    {
                        var invalidEmailMessage = _localizationService.GetMessage("InvalidEmail", "Errors", language);
                        return BaseResponse<UserDTO>.FailureResponse(invalidEmailMessage, 400);
                    }

                    // التحقق من عدم وجود البريد الإلكتروني بالفعل لمستخدم آخر
                    if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != userId && u.IsDeleted != true))
                    {
                        var emailExistsMessage = _localizationService.GetMessage("EmailExists", "Errors", language);
                        return BaseResponse<UserDTO>.FailureResponse(emailExistsMessage, 400);
                    }

                    user.Email = request.Email;
                }

                // التحقق من صحة رقم الهاتف الجديد (إذا تم تقديمه)
                if (request.PhoneNumber.HasValue && request.PhoneNumber != user.PhoneNumber)
                {
                    if (!IsValidPhoneNumber(request.PhoneNumber.Value.ToString()))
                    {
                        var invalidPhoneMessage = _localizationService.GetMessage("InvalidPhoneNumber", "Errors", language);
                        return BaseResponse<UserDTO>.FailureResponse(invalidPhoneMessage, 400);
                    }

                    // التحقق من عدم وجود رقم الهاتف بالفعل لمستخدم آخر
                    if (await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId && u.IsDeleted != true))
                    {
                        var phoneExistsMessage = _localizationService.GetMessage("PhoneNumberExists", "Errors", language);
                        return BaseResponse<UserDTO>.FailureResponse(phoneExistsMessage, 400);
                    }

                    user.PhoneNumber = request.PhoneNumber;
                }

                // تحديث الاسم الكامل (إذا تم تقديمه)
                if (!string.IsNullOrWhiteSpace(request.FullName))
                {
                    user.FirstName = request.FullName;
                }

                // تحديث معلومات التعديل
                user.ModifiedByUserId = userId;
                user.ModifiedDate = DateTime.Now;

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(userId, "UpdateProfile", "تحديث الملف الشخصي", request.IpAddress, request.UserAgent);

                // تحويل البيانات إلى النموذج المطلوب للمستخدم
                var userInfo = new UserDTO
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = new List<string> { user.UserRole }
                };

                var successMessage = _localizationService.GetMessage("UserProfileUpdated", "Messages", language);
                return BaseResponse<UserDTO>.SuccessResponse(userInfo, successMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = _localizationService.GetMessage("UserProfileUpdateError", "Errors", language);
                _logger.LogError(ex, "حدث خطأ أثناء تحديث معلومات المستخدم {userId}", userId);
                return BaseResponse<UserDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث معلومات المستخدم (للمسؤول)
        /// </summary>
        public async Task<BaseResponse<UserDTO>> AdminUpdateUserAsync(long userId, AdminUpdateUserRequestDTO request, long adminId, string language)
        {
            try
            {
                // البحث عن المستخدم
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted != true);
                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<UserDTO>.FailureResponse(userNotFoundMessage, 404);
                }

                // التحقق من صحة البريد الإلكتروني الجديد (إذا تم تقديمه)
                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
                {
                    if (!IsValidEmail(request.Email))
                    {
                        var invalidEmailMessage = _localizationService.GetMessage("InvalidEmail", "Errors", language);
                        return BaseResponse<UserDTO>.FailureResponse(invalidEmailMessage, 400);
                    }

                    // التحقق من عدم وجود البريد الإلكتروني بالفعل لمستخدم آخر
                    if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != userId && u.IsDeleted != true))
                    {
                        var emailExistsMessage = _localizationService.GetMessage("EmailExists", "Errors", language);
                        return BaseResponse<UserDTO>.FailureResponse(emailExistsMessage, 400);
                    }

                    user.Email = request.Email;
                }

                // التحقق من صحة رقم الهاتف الجديد (إذا تم تقديمه)
                if (request.PhoneNumber.HasValue && request.PhoneNumber != user.PhoneNumber)
                {
                    if (!IsValidPhoneNumber(request.PhoneNumber.Value.ToString()))
                    {
                        var invalidPhoneMessage = _localizationService.GetMessage("InvalidPhoneNumber", "Errors", language);
                        return BaseResponse<UserDTO>.FailureResponse(invalidPhoneMessage, 400);
                    }

                    // التحقق من عدم وجود رقم الهاتف بالفعل لمستخدم آخر
                    if (await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId && u.IsDeleted != true))
                    {
                        var phoneExistsMessage = _localizationService.GetMessage("PhoneNumberExists", "Errors", language);
                        return BaseResponse<UserDTO>.FailureResponse(phoneExistsMessage, 400);
                    }

                    user.PhoneNumber = request.PhoneNumber;
                }

                // تحديث الاسم الكامل (إذا تم تقديمه)
                if (!string.IsNullOrWhiteSpace(request.FullName))
                {
                    user.FirstName = request.FullName;
                }

                // تحديث دور المستخدم (إذا تم تقديمه)
                if (!string.IsNullOrWhiteSpace(request.Roles.SingleOrDefault()))
                {
                    // التحقق من صحة الدور

                    if (!Enum.IsDefined(typeof(UserRole), request.Roles))
                    {
                        var invalidRoleMessage = _localizationService.GetMessage("InvalidRole", "Errors", language);
                        return BaseResponse<UserDTO>.FailureResponse(invalidRoleMessage, 400);
                    }

                    user.UserRole = request.Roles.SingleOrDefault() ?? nameof(UserRole.USER);
                }

                // تحديث حالة التفعيل (إذا تم تقديمها)
                if (request.IsActive.HasValue)
                {
                    user.IsActive = request.IsActive.Value;
                }

                // تحديث معلومات التعديل
                user.ModifiedByUserId = adminId;
                user.ModifiedDate = DateTime.Now;

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // إضافة سجل للنشاط
                await AddUserActivityLogInternalAsync(adminId, "AdminUpdateUser", $"تحديث بيانات المستخدم {userId} بواسطة المسؤول", null, null);

                // تحويل البيانات إلى النموذج المطلوب للمستخدم
                var userInfo = new UserDTO
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = new List<string> { user.UserRole }
                };

                var successMessage = _localizationService.GetMessage("UserUpdatedByAdmin", "Messages", language);
                return BaseResponse<UserDTO>.SuccessResponse(userInfo, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث معلومات المستخدم {userId} بواسطة المسؤول {adminId}", userId, adminId);
                var errorMessage = _localizationService.GetMessage("UserUpdateError", "Errors", language);
                return BaseResponse<UserDTO>.FailureResponse(errorMessage, 500);
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
                user.ModifiedDate = DateTime.Now;

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
                    token.RevokedAt = DateTime.Now;
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
                user.ModifiedDate = DateTime.Now;

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
                    token.RevokedAt = DateTime.Now;
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
                user.ModifiedDate = DateTime.Now;

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
                user.ModifiedDate = DateTime.Now;

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // إلغاء جميع رموز التحديث للمستخدم
                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.IsDeleted != true)
                    .ToListAsync();

                foreach (var token in refreshTokens)
                {
                    token.RevokedAt = DateTime.Now;
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
        /// الحصول على جميع المستخدمين
        /// </summary>
        public async Task<PaginatedResponse<List<UserDTO>>> GetAllUsersAsync(int page, int pageSize, string language)
        {
            try
            {
                // التحقق من صحة المدخلات
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // تحديد الحد الأقصى لعدد العناصر في الصفحة

                // الحصول على المستخدمين مع التصفية والترتيب والتقسيم
                var users = await _context.Users
                    .Where(u => u.IsDeleted != true)
                    .OrderByDescending(u => u.CreateDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // حساب إجمالي عدد المستخدمين
                long totalUsers = await _context.Users
                    .Where(u => u.IsDeleted != true)
                    .CountAsync();

                // تحويل البيانات إلى النموذج المطلوب للمستخدمين
                var usersInfo = _mapper.Map<List<UserDTO>>(users);

                // إرجاع النتيجة مع معلومات التقسيم
                return PaginatedResponse<List<UserDTO>>.SuccessResponse(usersInfo, page, pageSize, totalUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على قائمة المستخدمين");
                var errorMessage = _localizationService.GetMessage("UserListRetrievalError", "Errors", language);
                return PaginatedResponse<List<UserDTO>>.FailureResponse(errorMessage, 500);
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
                Timestamp = DateTime.Now
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
            // يتم قبول كلمات المرور بطول 8 أحرف على الأقل وتحتوي على حرف كبير وحرف صغير ورقم ورمز
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
            return regex.IsMatch(password);
        }

        /// <summary>
        /// إضافة سجل نشاط للمستخدم
        /// </summary>
        public async Task<BaseResponse<bool>> AddUserActivityLogAsync(long userId, string activityType, string description, string ipAddress, string userAgent, string language)
        {
            try
            {
                // التحقق من وجود المستخدم
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted != true);

                if (user == null)
                {
                    var userNotFoundMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(userNotFoundMessage, 404);
                }

                // إضافة سجل النشاط
                await AddUserActivityLogInternalAsync(userId, activityType, description, ipAddress, userAgent);

                var successMessage = _localizationService.GetMessage("ActivityLogAdded", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إضافة سجل نشاط للمستخدم {userId}", userId);
                var errorMessage = _localizationService.GetMessage("ActivityLogError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }


        #endregion
    }
}