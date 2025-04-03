using System.Text.RegularExpressions;
using System.Web;

namespace API.Configuration
{
    public static class SecurityConfig
    {
        public static bool IsValidFileExtension(this IFormFile file, IConfiguration configuration)
        {
            var allowedExtensions = configuration.GetSection("Security:AllowedFileExtensions").Get<string[]>() ?? 
                new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        public static bool IsValidFileSize(this IFormFile file, IConfiguration configuration)
        {
            var maxFileSizeMB = configuration.GetValue<int>("Security:MaxUploadSizeMB", 10);
            var maxFileSizeBytes = maxFileSizeMB * 1024 * 1024;
            return file.Length <= maxFileSizeBytes;
        }

        public static string SanitizeFileName(this string fileName)
        {
            // Remove any invalid characters from the filename
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = $"[{invalidChars}]";
            var sanitizedFileName = Regex.Replace(fileName, invalidReStr, "_");
            
            // Ensure the filename doesn't contain path traversal sequences
            sanitizedFileName = Path.GetFileName(sanitizedFileName);
            
            return sanitizedFileName;
        }

        public static string SanitizeInput(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // HTML encode the input to prevent XSS attacks
            input = HttpUtility.HtmlEncode(input);
            
            // Remove potentially dangerous sequences
            input = input.Replace("javascript:", "")
                         .Replace("data:", "")
                         .Replace("<script", "")
                         .Replace("</script", "");
            
            return input;
        }

        public static string GetSecureFilePath(string baseDirectory, string fileName)
        {
            // Sanitize the filename and create a unique name
            var sanitizedFileName = fileName.SanitizeFileName();
            var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
            
            // Create the full path, ensuring it remains within the designated directory
            var path = Path.Combine(baseDirectory, uniqueFileName);
            
            // Verify the path is still within the base directory (prevents directory traversal)
            var fullPath = Path.GetFullPath(path);
            var fullBaseDir = Path.GetFullPath(baseDirectory);
            
            if (!fullPath.StartsWith(fullBaseDir))
            {
                throw new UnauthorizedAccessException("Invalid file path detected. Possible directory traversal attempt.");
            }
            
            return path;
        }

        public static IServiceCollection AddSecurityHeaders(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.Name = "XSRF-TOKEN";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            return services;
        }

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                // Add Content Security Policy
                context.Response.Headers.Append("Content-Security-Policy", 
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
                    "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self' https://fonts.gstatic.com; " +
                    "connect-src 'self' https://api.smartlawyer.om; " +
                    "frame-src 'self'; " +
                    "object-src 'none'; " +
                    "base-uri 'self';");
                
                // Add other security headers
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
                
                await next();
            });

            return app;
        }
    }
} 