namespace Application.Common
{
    /// <summary>
    /// Base response model for API responses
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class BaseResponse<T>
    {
        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result of the operation
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Data returned by the operation
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Creates a successful response with data
        /// </summary>
        /// <param name="data">The data to return</param>
        /// <param name="message">Success message</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>A successful response</returns>
        public static BaseResponse<T> SuccessResponse(T data, string message = "Operation completed successfully", int statusCode = 200)
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
        /// Creates a failure response
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>A failure response</returns>
        public static BaseResponse<T> FailureResponse(string message, int statusCode)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Data = default
            };
        }
    }

    /// <summary>
    /// Static helper class for creating responses without specifying a type
    /// </summary>
    public static class BaseResponse
    {
        /// <summary>
        /// Creates a failure response without data
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>A failure response</returns>
        public static BaseResponse<object> FailureResponse(string message, int statusCode)
        {
            return new BaseResponse<object>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Data = default
            };
        }
    }
}