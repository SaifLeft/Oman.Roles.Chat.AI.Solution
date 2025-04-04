using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace API.Extensions
{
    /// <summary>
    /// Extension methods for HTTP-related operations
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Serializes the specified object to JSON and writes it to the response
        /// </summary>
        public static void WriteJson<T>(this HttpResponse response, T obj, string contentType = "application/json")
        {
            response.ContentType = contentType;
            var json = JsonSerializer.Serialize(obj);
            response.WriteAsync(json);
        }

        /// <summary>
        /// Gets the user IP address from the HTTP context
        /// </summary>
        public static string GetUserIpAddress(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        /// <summary>
        /// Gets the user agent from the HTTP context
        /// </summary>
        public static string GetUserAgent(this HttpContext context)
        {
            return context.Request.Headers["User-Agent"].ToString();
        }
    }
} 