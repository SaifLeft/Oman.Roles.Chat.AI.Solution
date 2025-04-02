namespace Models.DTOs.Authorization
{

    // نماذج طلبات المستخدم
    public class RegisterUserRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public long? PhoneNumber { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class LoginWithPhoneRequestDTO
    {
        public long? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class UpdateUserProfileRequestDTO
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public long? PhoneNumber { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class ChangePasswordRequestDTO
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class ResetPasswordRequestDTO
    {
        public string Email { get; set; } = string.Empty;
        public string ResetCode { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    //AdminUpdateUserRequest

    public class AdminUserDTO
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// اسم المستخدم
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// أدوار المستخدم
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// حالة التفعيل
        /// </summary>
        public bool IsActive { get; set; }
    }
}
