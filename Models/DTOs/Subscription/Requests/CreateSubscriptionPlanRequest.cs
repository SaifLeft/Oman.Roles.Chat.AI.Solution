using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription.Requests
{
    /// <summary>
    /// ‰„Ê–Ã ÿ·» ≈‰‘«¡ Œÿ… «‘ —«ﬂ ÃœÌœ…
    /// Model for creating a new subscription plan
    /// </summary>
    public class CreateSubscriptionPlanRequestDTO
    {
        /// <summary>
        /// «”„ Œÿ… «·«‘ —«ﬂ
        /// Subscription plan name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Ê’› Œÿ… «·«‘ —«ﬂ
        /// Subscription plan description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// «·”⁄— «·‘Â—Ì
        /// Monthly price
        /// </summary>
        [JsonPropertyName("priceMonthly")]
        public double PriceMonthly { get; set; }

        /// <summary>
        /// «·”⁄— «·”‰ÊÌ
        /// Yearly price
        /// </summary>
        [JsonPropertyName("priceYearly")]
        public double PriceYearly { get; set; }

        /// <summary>
        /// ⁄œœ €—› «·œ—œ‘… «·„”„ÊÕ »Â«
        /// Number of allowed chat rooms
        /// </summary>
        [JsonPropertyName("allowedChatRooms")]
        public long AllowedChatRooms { get; set; }

        /// <summary>
        /// ⁄œœ «·„·›«  «·„”„ÊÕ »Â«
        /// Number of allowed files
        /// </summary>
        [JsonPropertyName("allowedFiles")]
        public long AllowedFiles { get; set; }

        /// <summary>
        /// «·ÕÃ„ «·„”„ÊÕ »Â ··„·›«  »«·„ÌÃ«»«Ì 
        /// Allowed file size in MB
        /// </summary>
        [JsonPropertyName("allowedFileSizeMb")]
        public long AllowedFileSizeMb { get; set; }

        /// <summary>
        /// Â· «·Œÿ… ‰‘ÿ…
        /// Is the plan active
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Â· «·Œÿ…  Ã—Ì»Ì…
        /// Is the plan a trial plan
        /// </summary>
        [JsonPropertyName("isTrial")]
        public bool IsTrial { get; set; } = false;

        /// <summary>
        /// „„Ì“«  «·Œÿ…
        /// Plan features
        /// </summary>
        [JsonPropertyName("features")]
        public List<string> Features { get; set; } = new List<string>();

        /// <summary>
        /// ⁄·«„«  «·Œÿ…
        /// Plan tags
        /// </summary>
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// ⁄œœ √Ì«„ «·› —… «· Ã—Ì»Ì…
        /// Number of trial days
        /// </summary>
        [JsonPropertyName("trialDays")]
        public long? TrialDays { get; set; }
        public long MaxQueriesPerDay { get; set; }
        public long MaxQueriesPerMonth { get; set; }
    }
}