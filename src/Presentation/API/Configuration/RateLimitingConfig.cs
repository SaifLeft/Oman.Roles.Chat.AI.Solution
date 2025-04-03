using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace API.Configuration
{
    public static class RateLimitingConfig
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            // Get rate limiting configuration from appsettings.json
            var generalRateLimit = configuration.GetValue<int>("RateLimiting:GeneralRateLimit", 100);
            var authRateLimit = configuration.GetValue<int>("RateLimiting:AuthRateLimit", 10);
            var windowSizeInMinutes = configuration.GetValue<int>("RateLimiting:WindowSizeInMinutes", 1);

            // Configure rate limiting policies
            services.AddRateLimiter(options =>
            {
                // Add a general rate limiter that applies to most endpoints
                options.AddFixedWindowLimiter("general", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(windowSizeInMinutes);
                    opt.PermitLimit = generalRateLimit;
                    opt.QueueLimit = 0; // Don't queue requests when limit is reached
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                // Add a stricter rate limiter for authentication endpoints
                options.AddFixedWindowLimiter("auth", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(windowSizeInMinutes);
                    opt.PermitLimit = authRateLimit;
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                // Configure rate limit exceeded response
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new
                    {
                        succeeded = false,
                        message = "Too many requests. Please try again later.",
                        statusCode = 429,
                        errors = new[] { "Rate limit exceeded" },
                        data = (object)null
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(response, token);
                };
            });

            return services;
        }

        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
        {
            app.UseRateLimiter();

            return app;
        }
    }
} 