﻿using System.Text.Json.Serialization;

namespace Models.DTOs.Authorization
{
    /// <summary>
    /// معلومات المستخدم
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// اسم المستخدم
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الكامل
        /// </summary>
        [JsonPropertyName("fullName")]
        public string FirstName { get; set; } = string.Empty;


        //last name
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// أدوار المستخدم
        /// </summary>
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>
    /// نموذج طلب تسجيل الدخول
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>
        /// اسم المستخدم
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// كلمة المرور
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string UserAgent { get; set; }
    }

    /// <summary>
    /// نموذج استجابة تسجيل الدخول
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// رمز الوصول (JWT)
        /// </summary>
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// رمز التحديث
        /// </summary>
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ انتهاء الصلاحية
        /// </summary>
        [JsonPropertyName("expiresAt")]
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// نموذج طلب تحديث الرمز
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// رمز التحديث
        /// </summary>
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }


}