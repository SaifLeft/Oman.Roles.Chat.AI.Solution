using System.Text.Json.Serialization;

namespace Models.DTOs.Subscription.Requests
{
    /// <summary>
    /// ����� ��� ����� ��� ������ �����
    /// Model for creating a new subscription plan
    /// </summary>
    public class CreateSubscriptionPlanRequestDTO
    {
        /// <summary>
        /// ��� ��� ��������
        /// Subscription plan name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// ��� ��� ��������
        /// Subscription plan description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// ����� ������
        /// Monthly price
        /// </summary>
        [JsonPropertyName("priceMonthly")]
        public double PriceMonthly { get; set; }

        /// <summary>
        /// ����� ������
        /// Yearly price
        /// </summary>
        [JsonPropertyName("priceYearly")]
        public double PriceYearly { get; set; }

        /// <summary>
        /// ��� ��� ������� ������� ���
        /// Number of allowed chat rooms
        /// </summary>
        [JsonPropertyName("allowedChatRooms")]
        public long AllowedChatRooms { get; set; }

        /// <summary>
        /// ��� ������� ������� ���
        /// Number of allowed files
        /// </summary>
        [JsonPropertyName("allowedFiles")]
        public long AllowedFiles { get; set; }

        /// <summary>
        /// ����� ������� �� ������� �����������
        /// Allowed file size in MB
        /// </summary>
        [JsonPropertyName("allowedFileSizeMb")]
        public long AllowedFileSizeMb { get; set; }

        /// <summary>
        /// �� ����� ����
        /// Is the plan active
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// �� ����� �������
        /// Is the plan a trial plan
        /// </summary>
        [JsonPropertyName("isTrial")]
        public bool IsTrial { get; set; } = false;

        /// <summary>
        /// ������ �����
        /// Plan features
        /// </summary>
        [JsonPropertyName("features")]
        public List<string> Features { get; set; } = new List<string>();

        /// <summary>
        /// ������ �����
        /// Plan tags
        /// </summary>
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// ��� ���� ������ ���������
        /// Number of trial days
        /// </summary>
        [JsonPropertyName("trialDays")]
        public long? TrialDays { get; set; }
        public long MaxQueriesPerDay { get; set; }
        public long MaxQueriesPerMonth { get; set; }
    }
}