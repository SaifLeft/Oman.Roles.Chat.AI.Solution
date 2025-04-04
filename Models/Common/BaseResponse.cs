using System.Text.Json.Serialization;

namespace Models.Common
{
    /// <summary>
    /// يمثل الاستجابة الأساسية الموحدة لجميع الطلبات
    /// </summary>
    /// <typeparam name="T">نوع البيانات المُرجعة</typeparam>
    public class BaseResponse<T>
    {
        /// <summary>
        /// حالة الاستجابة (نجاح أو فشل)
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// رسالة الاستجابة
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// رمز الحالة HTTP
        /// </summary>
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; } = 200; // 200 for success, 400 for failure, etc.

        /// <summary>
        /// البيانات المُرجعة
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        /// <summary>
        /// قائمة بالأخطاء إن وجدت
        /// </summary>
        [JsonPropertyName("errors")]
        public List<string>? Errors { get; set; }

        /// <summary>
        /// إنشاء استجابة ناجحة
        /// </summary>
        public static BaseResponse<T> SuccessResponse(T data, string message = "تمت العملية بنجاح", int statusCode = 200)
        {
            return new BaseResponse<T>
            {
                Success = true,
                Message = message,
                StatusCode = statusCode,
                Data = data
            };
        }

        /// <summary>
        /// إنشاء استجابة ناجحة بدون بيانات
        /// </summary>
        public static BaseResponse<T> SuccessResponse(string message = "تمت العملية بنجاح", int statusCode = 200)
        {
            return new BaseResponse<T>
            {
                Success = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// إنشاء استجابة فاشلة
        /// </summary>
        public static BaseResponse<T> FailureResponse(string message = "فشلت العملية", int statusCode = 400, List<string>? errors = null)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// استجابة أساسية بدون بيانات مُرجعة
    /// </summary>
    public class BaseResponse : BaseResponse<object>
    {
        /// <summary>
        /// إنشاء استجابة ناجحة
        /// </summary>
        public static BaseResponse SuccessResponse(string message = "تمت العملية بنجاح", int statusCode = 200)
        {
            return new BaseResponse
            {
                Success = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// إنشاء استجابة فاشلة
        /// </summary>
        public static new BaseResponse FailureResponse(string message = "فشلت العملية", int statusCode = 400, List<string>? errors = null)
        {
            return new BaseResponse
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
}