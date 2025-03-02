using System.Text.Json.Serialization;

namespace Muhami.DTOs
{
    public class ConversationTrackingDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("conversationId")]
        public string ConversationId { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("roomId")]
        public string RoomId { get; set; }

        [JsonPropertyName("userQuery")]
        public string UserQuery { get; set; }

        [JsonPropertyName("aiResponse")]
        public string AiResponse { get; set; }

        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("keywords")]
        public List<ConversationKeywordDTO> Keywords { get; set; } = new List<ConversationKeywordDTO>();

        [JsonPropertyName("pdfReferences")]
        public List<ConversationPdfReferenceDTO> PdfReferences { get; set; } = new List<ConversationPdfReferenceDTO>();

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    public class ConversationKeywordDTO
    {
        [JsonPropertyName("keyword")]
        public string Keyword { get; set; }

        [JsonPropertyName("count")]
        public long Count { get; set; }
    }

    public class ConversationPdfReferenceDTO
    {
        [JsonPropertyName("fileId")]
        public string FileId { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("relevanceScore")]
        public long RelevanceScore { get; set; }
    }

    public class ConversationAnalyticsDTO
    {
        [JsonPropertyName("topTopics")]
        public List<TopicStat> TopTopics { get; set; } = new List<TopicStat>();

        [JsonPropertyName("topKeywords")]
        public List<KeywordStat> TopKeywords { get; set; } = new List<KeywordStat>();

        [JsonPropertyName("conversationCount")]
        public int ConversationCount { get; set; }

        [JsonPropertyName("averageMessagesPerConversation")]
        public double AverageMessagesPerConversation { get; set; }

        [JsonPropertyName("topReferencedPdfs")]
        public List<PdfReferenceStat> TopReferencedPdfs { get; set; } = new List<PdfReferenceStat>();
    }

    public class TopicStat
    {
        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }
    }

    public class KeywordStat
    {
        [JsonPropertyName("keyword")]
        public string Keyword { get; set; }

        [JsonPropertyName("count")]
        public long Count { get; set; }
    }

    public class PdfReferenceStat
    {
        [JsonPropertyName("fileId")]
        public string FileId { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("referenceCount")]
        public long ReferenceCount { get; set; }
    }
}