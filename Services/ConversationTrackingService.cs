using AutoMapper;
using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Muhami.DTOs;
using System.Text.Json;

namespace Services
{
    /// <summary>
    /// Interface for conversation tracking service
    /// </summary>
    public interface IConversationTrackingService
    {
        /// <summary>
        /// Log a conversation between a user and the AI
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roomId">Chat room ID</param>
        /// <param name="userQuery">User's message</param>
        /// <param name="aiResponse">AI's response</param>
        /// <param name="metadata">Additional metadata about the conversation</param>
        /// <returns>The conversation ID</returns>
        Task<string> LogConversationAsync(
            string userId,
            string roomId,
            string userQuery,
            string aiResponse,
            Dictionary<string, object> metadata = null);

        /// <summary>
        /// Track keywords associated with a conversation
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <param name="keywords">List of keywords</param>
        Task TrackKeywordsAsync(string conversationId, List<string> keywords);

        /// <summary>
        /// Track PDF references used in a conversation
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <param name="pdfReferences">Dictionary of PDF filenames and relevance scores</param>
        Task TrackPdfReferencesAsync(string conversationId, Dictionary<string, int> pdfReferences);

        /// <summary>
        /// Get conversation history for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="limit">Maximum number of conversations to retrieve</param>
        /// <param name="offset">Offset for pagination</param>
        /// <returns>List of conversations</returns>
        Task<List<ConversationTrackingDTO>> GetUserConversationsAsync(string userId, int limit = 20, int offset = 0);

        /// <summary>
        /// Get conversation history for a chat room
        /// </summary>
        /// <param name="roomId">Chat room ID</param>
        /// <param name="limit">Maximum number of conversations to retrieve</param>
        /// <param name="offset">Offset for pagination</param>
        /// <returns>List of conversations</returns>
        Task<List<ConversationTrackingDTO>> GetRoomConversationsAsync(string roomId, int limit = 20, int offset = 0);

        /// <summary>
        /// Get analytics data for all conversations
        /// </summary>
        /// <param name="fromDate">Start date for analytics</param>
        /// <param name="toDate">End date for analytics</param>
        /// <returns>Analytics data</returns>
        Task<ConversationAnalyticsDTO> GetAnalyticsAsync(DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Get analytics data for a specific user's conversations
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="fromDate">Start date for analytics</param>
        /// <param name="toDate">End date for analytics</param>
        /// <returns>Analytics data</returns>
        Task<ConversationAnalyticsDTO> GetUserAnalyticsAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null);
    }

    /// <summary>
    /// Implementation of the conversation tracking service
    /// </summary>
    public class ConversationTrackingService : IConversationTrackingService
    {
        private readonly MuhamiContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ConversationTrackingService> _logger;
        private readonly IConfiguration _configuration;

        public ConversationTrackingService(
            MuhamiContext context,
            IMapper mapper,
            ILogger<ConversationTrackingService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Log a conversation between a user and the AI
        /// </summary>
        public async Task<string> LogConversationAsync(
            string userId,
            string roomId,
            string userQuery,
            string aiResponse,
            Dictionary<string, object> metadata = null)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    _logger.LogWarning("Invalid user ID format: {UserId}", userId);
                    userIdLong = 0; // Use a default value for system or anonymous
                }

                // Extract topic from metadata if available
                string topic = "General";
                string language = "ar";

                if (metadata != null)
                {
                    if (metadata.TryGetValue("topic", out var topicObj) && topicObj != null)
                    {
                        topic = topicObj.ToString();
                    }

                    if (metadata.TryGetValue("language", out var languageObj) && languageObj != null)
                    {
                        language = languageObj.ToString();
                    }
                }

                // Create new conversation tracking entry
                var conversationId = Guid.NewGuid().ToString();
                var conversation = new ConversationTracking
                {
                    ConversationId = conversationId,
                    UserId = userIdLong,
                    RoomId = long.Parse(roomId),
                    UserQuery = userQuery,
                    AiResponse = aiResponse,
                    Topic = topic,
                    Language = language,
                    MetadataJson = metadata != null ? JsonSerializer.Serialize(metadata) : null,
                    Timestamp = DateTime.Now,
                    CreatedByUserId = userIdLong,
                    CreateDate = DateTime.Now
                };

                _context.ConversationTrackings.Add(conversation);
                await _context.SaveChangesAsync();

                // If metadata contains PDF files, track them
                if (metadata != null && metadata.TryGetValue("pdfFiles", out var pdfFilesObj) && pdfFilesObj is List<string> pdfFiles && pdfFiles.Count > 0)
                {
                    // Create a dictionary with default relevance scores
                    var pdfReferences = pdfFiles.ToDictionary(file => file, file => 100);
                    await TrackPdfReferencesAsync(conversationId, pdfReferences);
                }

                return conversationId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging conversation for user {UserId} in room {RoomId}", userId, roomId);
                return string.Empty;
            }
        }

        /// <summary>
        /// Track keywords associated with a conversation
        /// </summary>
        public async Task TrackKeywordsAsync(string conversationId, List<string> keywords)
        {
            if (string.IsNullOrEmpty(conversationId) || keywords == null || keywords.Count == 0)
            {
                return;
            }

            try
            {
                // Find the conversation
                var conversation = await _context.ConversationTrackings
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId && c.IsDeleted != true);

                if (conversation == null)
                {
                    _logger.LogWarning("Conversation not found: {ConversationId}", conversationId);
                    return;
                }

                // Group keywords to count occurrences
                var keywordGroups = keywords
                    .Select(k => k.Trim().ToLower())
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .GroupBy(k => k)
                    .Select(g => new { Keyword = g.Key, Count = g.Count() });

                foreach (var keywordGroup in keywordGroups)
                {
                    // Check if the keyword already exists for this conversation
                    var existingKeyword = await _context.ConversationKeywords
                        .FirstOrDefaultAsync(k => k.ConversationId == conversation.Id &&
                                              k.Keyword.ToLower() == keywordGroup.Keyword.ToLower());

                    if (existingKeyword != null)
                    {
                        // Update the count
                        existingKeyword.Count += keywordGroup.Count;
                    }
                    else
                    {
                        // Add a new keyword
                        _context.ConversationKeywords.Add(new ConversationKeyword
                        {
                            ConversationId = conversation.Id,
                            Keyword = keywordGroup.Keyword,
                            Count = keywordGroup.Count,
                            CreatedByUserId = conversation.CreatedByUserId,
                            CreateDate = DateTime.Now
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking keywords for conversation {ConversationId}", conversationId);
            }
        }

        /// <summary>
        /// Track PDF references used in a conversation
        /// </summary>
        public async Task TrackPdfReferencesAsync(string conversationId, Dictionary<string, int> pdfReferences)
        {
            if (string.IsNullOrEmpty(conversationId) || pdfReferences == null || pdfReferences.Count == 0)
            {
                return;
            }

            try
            {
                // Find the conversation
                var conversation = await _context.ConversationTrackings
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId && c.IsDeleted != true);

                if (conversation == null)
                {
                    _logger.LogWarning("Conversation not found: {ConversationId}", conversationId);
                    return;
                }

                // Get all PDF files
                var fileNames = pdfReferences.Keys.ToList();
                var pdfFiles = await _context.DataSourceFiles
                    .Where(f => fileNames.Contains(f.FileName) && f.IsDeleted != true)
                    .ToListAsync();

                // Create PDF references
                foreach (var pdfFile in pdfFiles)
                {
                    if (pdfReferences.TryGetValue(pdfFile.FileName, out int relevanceScore))
                    {
                        _context.ConversationPdfReferences.Add(new ConversationPdfReference
                        {
                            ConversationId = conversation.Id,
                            FileId = pdfFile.Id,
                            FileName = pdfFile.FileName,
                            RelevanceScore = relevanceScore,
                            CreatedByUserId = conversation.CreatedByUserId,
                            CreateDate = DateTime.Now
                        });
                    }
                }

                // If some files weren't found in the database, log them by filename only
                var missingFiles = fileNames
                    .Except(pdfFiles.Select(f => f.FileName))
                    .ToList();

                foreach (var fileName in missingFiles)
                {
                    if (pdfReferences.TryGetValue(fileName, out int relevanceScore))
                    {
                        _context.ConversationPdfReferences.Add(new ConversationPdfReference
                        {
                            ConversationId = conversation.Id,
                            FileId = 0, // Invalid ID to indicate it's not in the database
                            FileName = fileName,
                            RelevanceScore = relevanceScore,
                            CreatedByUserId = conversation.CreatedByUserId,
                            CreateDate = DateTime.Now
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking PDF references for conversation {ConversationId}", conversationId);
            }
        }

        /// <summary>
        /// Get conversation history for a user
        /// </summary>
        public async Task<List<ConversationTrackingDTO>> GetUserConversationsAsync(string userId, int limit = 20, int offset = 0)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    _logger.LogWarning("Invalid user ID format: {UserId}", userId);
                    return new List<ConversationTrackingDTO>();
                }

                // Get the conversations
                var conversations = await _context.ConversationTrackings
                    .Where(c => c.UserId == userIdLong && c.IsDeleted != true)
                    .OrderByDescending(c => c.Timestamp)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();

                // Map to DTOs and load related data
                var conversationDtos = new List<ConversationTrackingDTO>();
                foreach (var conversation in conversations)
                {
                    var dto = new ConversationTrackingDTO
                    {
                        Id = conversation.Id.ToString(),
                        ConversationId = conversation.ConversationId,
                        UserId = conversation.UserId.ToString(),
                        RoomId = conversation.RoomId.ToString(),
                        UserQuery = conversation.UserQuery,
                        AiResponse = conversation.AiResponse,
                        Topic = conversation.Topic,
                        Language = conversation.Language,
                        Timestamp = conversation.Timestamp
                    };

                    // Load metadata
                    if (!string.IsNullOrEmpty(conversation.MetadataJson))
                    {
                        try
                        {
                            dto.Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(conversation.MetadataJson);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing metadata for conversation {ConversationId}", conversation.ConversationId);
                        }
                    }

                    // Load keywords
                    var keywords = await _context.ConversationKeywords
                        .Where(k => k.ConversationId == conversation.Id)
                        .ToListAsync();

                    dto.Keywords = keywords.Select(k => new ConversationKeywordDTO
                    {
                        Keyword = k.Keyword,
                        Count = k.Count
                    }).ToList();

                    // Load PDF references
                    var pdfReferences = await _context.ConversationPdfReferences
                        .Where(p => p.ConversationId == conversation.Id)
                        .ToListAsync();

                    dto.PdfReferences = pdfReferences.Select(p => new ConversationPdfReferenceDTO
                    {
                        FileId = p.FileId.ToString(),
                        FileName = p.FileName,
                        RelevanceScore = p.RelevanceScore
                    }).ToList();

                    conversationDtos.Add(dto);
                }

                return conversationDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations for user {UserId}", userId);
                return new List<ConversationTrackingDTO>();
            }
        }

        /// <summary>
        /// Get conversation history for a chat room
        /// </summary>
        public async Task<List<ConversationTrackingDTO>> GetRoomConversationsAsync(string roomId, int limit = 20, int offset = 0)
        {
            try
            {
                // Get the conversations
                var conversations = await _context.ConversationTrackings
                    .Where(c => c.RoomId == long.Parse(roomId) && c.IsDeleted != true)
                    .OrderByDescending(c => c.Timestamp)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();

                // Map to DTOs and load related data
                var conversationDtos = new List<ConversationTrackingDTO>();
                foreach (var conversation in conversations)
                {
                    var dto = new ConversationTrackingDTO
                    {
                        Id = conversation.Id.ToString(),
                        ConversationId = conversation.ConversationId,
                        UserId = conversation.UserId.ToString(),
                        RoomId = conversation.RoomId.ToString(),
                        UserQuery = conversation.UserQuery,
                        AiResponse = conversation.AiResponse,
                        Topic = conversation.Topic,
                        Language = conversation.Language,
                        Timestamp = conversation.Timestamp
                    };

                    // Load metadata
                    if (!string.IsNullOrEmpty(conversation.MetadataJson))
                    {
                        try
                        {
                            dto.Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(conversation.MetadataJson);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing metadata for conversation {ConversationId}", conversation.ConversationId);
                        }
                    }

                    // Load keywords
                    var keywords = await _context.ConversationKeywords
                        .Where(k => k.ConversationId == conversation.Id)
                        .ToListAsync();

                    dto.Keywords = keywords.Select(k => new ConversationKeywordDTO
                    {
                        Keyword = k.Keyword,
                        Count = k.Count
                    }).ToList();

                    // Load PDF references
                    var pdfReferences = await _context.ConversationPdfReferences
                        .Where(p => p.ConversationId == conversation.Id)
                        .ToListAsync();

                    dto.PdfReferences = pdfReferences.Select(p => new ConversationPdfReferenceDTO
                    {
                        FileId = p.FileId.ToString(),
                        FileName = p.FileName,
                        RelevanceScore = p.RelevanceScore
                    }).ToList();

                    conversationDtos.Add(dto);
                }

                return conversationDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations for room {RoomId}", roomId);
                return new List<ConversationTrackingDTO>();
            }
        }

        /// <summary>
        /// Get analytics data for all conversations
        /// </summary>
        public async Task<ConversationAnalyticsDTO> GetAnalyticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                // Apply date filters if provided
                var query = _context.ConversationTrackings.AsQueryable();
                if (fromDate.HasValue)
                {
                    query = query.Where(c => c.Timestamp >= fromDate.Value);
                }
                if (toDate.HasValue)
                {
                    query = query.Where(c => c.Timestamp <= toDate.Value);
                }
                query = query.Where(c => c.IsDeleted != true);

                // Calculate basic metrics
                var conversationCount = await query.CountAsync();

                // Get top topics
                var topTopics = await query
                    .GroupBy(c => c.Topic)
                    .Select(g => new { Topic = g.Key, Count = g.Count() })
                    .OrderByDescending(t => t.Count)
                    .Take(10)
                    .ToListAsync();

                var topicStats = topTopics.Select(t => new TopicStat
                {
                    Topic = t.Topic,
                    Count = t.Count,
                    Percentage = conversationCount > 0 ? (double)t.Count / conversationCount * 100 : 0
                }).ToList();

                // Get top keywords
                var topKeywords = await _context.ConversationKeywords
                    .Where(k => k.Conversation.IsDeleted != true)
                    .GroupBy(k => k.Keyword)
                    .Select(g => new { Keyword = g.Key, Count = g.Sum(k => k.Count) })
                    .OrderByDescending(k => k.Count)
                    .Take(20)
                    .ToListAsync();

                var keywordStats = topKeywords.Select(k => new KeywordStat
                {
                    Keyword = k.Keyword,
                    Count = k.Count
                }).ToList();

                // Get top referenced PDFs
                var topPdfs = await _context.ConversationPdfReferences
                    .Where(p => p.Conversation.IsDeleted != true)
                    .GroupBy(p => new { p.FileId, p.FileName })
                    .Select(g => new { g.Key.FileId, g.Key.FileName, Count = g.Count() })
                    .OrderByDescending(p => p.Count)
                    .Take(10)
                    .ToListAsync();

                var pdfStats = topPdfs.Select(p => new PdfReferenceStat
                {
                    FileId = p.FileId.ToString(),
                    FileName = p.FileName,
                    ReferenceCount = p.Count
                }).ToList();

                // Calculate average messages per conversation
                var totalConversations = await query.CountAsync();
                var averageMessages = totalConversations > 0
                    ? await query.Select(c => c.Id).CountAsync() / (double)totalConversations
                    : 0;

                // Create and return the analytics DTO
                return new ConversationAnalyticsDTO
                {
                    ConversationCount = conversationCount,
                    TopTopics = topicStats,
                    TopKeywords = keywordStats,
                    TopReferencedPdfs = pdfStats,
                    AverageMessagesPerConversation = averageMessages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating analytics");
                return new ConversationAnalyticsDTO
                {
                    ConversationCount = 0,
                    TopTopics = new List<TopicStat>(),
                    TopKeywords = new List<KeywordStat>(),
                    TopReferencedPdfs = new List<PdfReferenceStat>(),
                    AverageMessagesPerConversation = 0
                };
            }
        }

        /// <summary>
        /// Get analytics data for a specific user's conversations
        /// </summary>
        public async Task<ConversationAnalyticsDTO> GetUserAnalyticsAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    _logger.LogWarning("Invalid user ID format: {UserId}", userId);
                    return new ConversationAnalyticsDTO
                    {
                        ConversationCount = 0,
                        TopTopics = new List<TopicStat>(),
                        TopKeywords = new List<KeywordStat>(),
                        TopReferencedPdfs = new List<PdfReferenceStat>(),
                        AverageMessagesPerConversation = 0
                    };
                }

                // Apply user and date filters
                var query = _context.ConversationTrackings
                    .Where(c => c.UserId == userIdLong && c.IsDeleted != true);

                if (fromDate.HasValue)
                {
                    query = query.Where(c => c.Timestamp >= fromDate.Value);
                }
                if (toDate.HasValue)
                {
                    query = query.Where(c => c.Timestamp <= toDate.Value);
                }

                // Calculate basic metrics
                var conversationCount = await query.CountAsync();

                // Get top topics for this user
                var topTopics = await query
                    .GroupBy(c => c.Topic)
                    .Select(g => new { Topic = g.Key, Count = g.Count() })
                    .OrderByDescending(t => t.Count)
                    .Take(10)
                    .ToListAsync();

                var topicStats = topTopics.Select(t => new TopicStat
                {
                    Topic = t.Topic,
                    Count = t.Count,
                    Percentage = conversationCount > 0 ? (double)t.Count / conversationCount * 100 : 0
                }).ToList();

                // Get top keywords for this user
                var topKeywords = await _context.ConversationKeywords
                    .Where(k => k.Conversation.UserId == userIdLong && k.Conversation.IsDeleted != true)
                    .GroupBy(k => k.Keyword)
                    .Select(g => new { Keyword = g.Key, Count = g.Sum(k => k.Count) })
                    .OrderByDescending(k => k.Count)
                    .Take(20)
                    .ToListAsync();

                var keywordStats = topKeywords.Select(k => new KeywordStat
                {
                    Keyword = k.Keyword,
                    Count = k.Count
                }).ToList();

                // Get top referenced PDFs for this user
                var topPdfs = await _context.ConversationPdfReferences
                    .Where(p => p.Conversation.UserId == userIdLong && p.Conversation.IsDeleted != true)
                    .GroupBy(p => new { p.FileId, p.FileName })
                    .Select(g => new { g.Key.FileId, g.Key.FileName, Count = g.Count() })
                    .OrderByDescending(p => p.Count)
                    .Take(10)
                    .ToListAsync();

                var pdfStats = topPdfs.Select(p => new PdfReferenceStat
                {
                    FileId = p.FileId.ToString(),
                    FileName = p.FileName,
                    ReferenceCount = p.Count
                }).ToList();

                // Calculate average messages per conversation for this user
                var totalConversations = await query.CountAsync();
                var averageMessages = totalConversations > 0
                    ? await query.Select(c => c.Id).CountAsync() / (double)totalConversations
                    : 0;

                // Create and return the analytics DTO
                return new ConversationAnalyticsDTO
                {
                    ConversationCount = conversationCount,
                    TopTopics = topicStats,
                    TopKeywords = keywordStats,
                    TopReferencedPdfs = pdfStats,
                    AverageMessagesPerConversation = averageMessages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating analytics for user {UserId}", userId);
                return new ConversationAnalyticsDTO
                {
                    ConversationCount = 0,
                    TopTopics = new List<TopicStat>(),
                    TopKeywords = new List<KeywordStat>(),
                    TopReferencedPdfs = new List<PdfReferenceStat>(),
                    AverageMessagesPerConversation = 0
                };
            }
        }
    }
}