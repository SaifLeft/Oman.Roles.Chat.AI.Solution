using System.Text.Json.Serialization;

namespace Models.Common
{
    /// <summary>
    /// استجابة مع تقسيم الصفحات
    /// </summary>
    public class PaginatedResponse<T> : BaseResponse<T>
    {
        /// <summary>
        /// رقم الصفحة الحالية
        /// </summary>
        [JsonPropertyName("page")]
        public long Page { get; set; }

        /// <summary>
        /// حجم الصفحة
        /// </summary>
        [JsonPropertyName("pageSize")]
        public long PageSize { get; set; }

        /// <summary>
        /// إجمالي عدد العناصر
        /// </summary>
        [JsonPropertyName("totalCount")]
        public long TotalCount { get; set; }

        /// <summary>
        /// إجمالي عدد الصفحات
        /// </summary>
        [JsonPropertyName("totalPages")]
        public long TotalPages { get; set; }

        /// <summary>
        /// هل هناك صفحة سابقة
        /// </summary>
        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// هل هناك صفحة تالية
        /// </summary>
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage => Page < TotalPages;

        public static PaginatedResponse<T> SuccessResponse(T data, string message, long page, long pageSize, long totalCount)
        {
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return new PaginatedResponse<T>
            {
                Success = true,
                Message = message,
                StatusCode = 200,
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public static PaginatedResponse<T> FailureResponse(string message, int statusCode)
        {
            return new PaginatedResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Data = default,
                Page = 0,
                PageSize = 0,
                TotalCount = 0,
                TotalPages = 0
            };
        }
    }
}