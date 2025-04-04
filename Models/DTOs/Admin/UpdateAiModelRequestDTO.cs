namespace Models.DTOs.Admin
{


    /// <summary>
    /// Update AI model request
    /// </summary>
    public class UpdateAiModelRequestDTO
    {
        /// <summary>
        /// Model name
        /// </summary>
        public string ModelName { get; set; } = string.Empty;

        /// <summary>
        /// API endpoint
        /// </summary>
        public string? Endpoint { get; set; }

        /// <summary>
        /// API key
        /// </summary>
        public string? ApiKey { get; set; }
    }
}
