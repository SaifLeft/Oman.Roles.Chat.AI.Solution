﻿using AutoMapper;
using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using System.Text.Json;
using Data.Structure.Entities;
using Models.DTOs.AIChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Common;

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
        Task<BaseResponse<string>> LogConversationAsync(
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
        Task<BaseResponse<bool>> TrackKeywordsAsync(string conversationId, List<string> keywords);

        /// <summary>
        /// Track PDF references used in a conversation
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <param name="pdfReferences">Dictionary of PDF filenames and relevance scores</param>
        Task<BaseResponse<bool>> TrackPdfReferencesAsync(string conversationId, Dictionary<string, int> pdfReferences);

        /// <summary>
        /// Get conversation history for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="limit">Maximum number of conversations to retrieve</param>
        /// <param name="offset">Offset for pagination</param>
        /// <returns>List of conversations</returns>
        Task<BaseResponse<List<ConversationTrackingDTO>>> GetUserConversationsAsync(string userId, int limit = 20, int offset = 0);

        /// <summary>
        /// Get conversation history for a chat room
        /// </summary>
        /// <param name="roomId">Chat room ID</param>
        /// <param name="limit">Maximum number of conversations to retrieve</param>
        /// <param name="offset">Offset for pagination</param>
        /// <returns>List of conversations</returns>
        Task<BaseResponse<List<ConversationTrackingDTO>>> GetRoomConversationsAsync(string roomId, int limit = 20, int offset = 0);

        /// <summary>
        /// Get analytics data for all conversations
        /// </summary>
        /// <param name="fromDate">Start date for analytics</param>
        /// <param name="toDate">End date for analytics</param>
        /// <returns>Analytics data</returns>
        Task<BaseResponse<ConversationAnalyticsDTO>> GetAnalyticsAsync(DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Get analytics data for a specific user's conversations
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="fromDate">Start date for analytics</param>
        /// <param name="toDate">End date for analytics</param>
        /// <returns>Analytics data</returns>
        Task<BaseResponse<ConversationAnalyticsDTO>> GetUserAnalyticsAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// تتبع محادثة المستخدم مع الذكاء الاصطناعي
        /// </summary>
        /// <param name="conversationId">معرف المحادثة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="userQuery">استعلام المستخدم</param>
        /// <param name="aiResponse">رد الذكاء الاصطناعي</param>
        /// <param name="topic">موضوع المحادثة</param>
        /// <param name="language">لغة المحادثة</param>
        /// <returns>هل تم التتبع بنجاح</returns>
        Task<BaseResponse<bool>> TrackConversationAsync(string conversationId, long userId, string userQuery, string aiResponse, string topic, string language);
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
        private readonly ILocalizationService _localizationService;

        public ConversationTrackingService(
            MuhamiContext context,
            IMapper mapper,
            ILogger<ConversationTrackingService> logger,
            IConfiguration configuration,
            ILocalizationService localizationService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Log a conversation between a user and the AI
        /// </summary>
        public async Task<BaseResponse<string>> LogConversationAsync(
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

                var successMessage = _localizationService.GetMessage("ConversationLoggedSuccessfully", "Messages", language);
                return BaseResponse<string>.SuccessResponse(conversationId, successMessage, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging conversation for user {UserId} in room {RoomId}", userId, roomId);
                var language = metadata != null && metadata.TryGetValue("language", out var languageObj) ? languageObj.ToString() : "ar";
                var errorMessage = _localizationService.GetMessage("ConversationLoggingError", "Errors", language);
                return BaseResponse<string>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Track keywords associated with a conversation
        /// </summary>
        public async Task<BaseResponse<bool>> TrackKeywordsAsync(string conversationId, List<string> keywords)
        {
            string language = "ar"; // Default language
            try
            {
                if (string.IsNullOrEmpty(conversationId) || keywords == null || keywords.Count == 0)
                {
                    var invalidDataMessage = _localizationService.GetMessage("InvalidConversationOrKeywords", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(invalidDataMessage, 400);
                }

                // Find the conversation
                var conversation = await _context.ConversationTrackings
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId && c.IsDeleted != true);

                if (conversation == null)
                {
                    _logger.LogWarning("Conversation not found: {ConversationId}", conversationId);
                    var notFoundMessage = _localizationService.GetMessage("ConversationNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(notFoundMessage, 404);
                }

                // Update language from conversation
                language = conversation.Language;

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
                var successMessage = _localizationService.GetMessage("KeywordsTrackedSuccessfully", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking keywords for conversation {ConversationId}", conversationId);
                var errorMessage = _localizationService.GetMessage("KeywordTrackingError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Track PDF references used in a conversation
        /// </summary>
        public async Task<BaseResponse<bool>> TrackPdfReferencesAsync(string conversationId, Dictionary<string, int> pdfReferences)
        {
            string language = "ar"; // Default language
            try
            {
                if (string.IsNullOrEmpty(conversationId) || pdfReferences == null || pdfReferences.Count == 0)
                {
                    var invalidDataMessage = _localizationService.GetMessage("InvalidConversationOrPdfReferences", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(invalidDataMessage, 400);
                }

                // Find the conversation
                var conversation = await _context.ConversationTrackings
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId && c.IsDeleted != true);

                if (conversation == null)
                {
                    _logger.LogWarning("Conversation not found: {ConversationId}", conversationId);
                    var notFoundMessage = _localizationService.GetMessage("ConversationNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(notFoundMessage, 404);
                }

                // Update language from conversation
                language = conversation.Language;

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
                var successMessage = _localizationService.GetMessage("PdfReferencesTrackedSuccessfully", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking PDF references for conversation {ConversationId}", conversationId);
                var errorMessage = _localizationService.GetMessage("PdfReferenceTrackingError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get conversation history for a user
        /// </summary>
        public async Task<BaseResponse<List<ConversationTrackingDTO>>> GetUserConversationsAsync(string userId, int limit = 20, int offset = 0)
        {
            string language = "ar"; // Default language
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    _logger.LogWarning("Invalid user ID format: {UserId}", userId);
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<List<ConversationTrackingDTO>>.FailureResponse(invalidIdMessage, 400);
                }

                // Get the conversations
                var conversations = await _context.ConversationTrackings
                    .Where(c => c.UserId == userIdLong && c.IsDeleted != true)
                    .OrderByDescending(c => c.Timestamp)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();

                // Try to get language from first conversation
                if (conversations.Any())
                {
                    language = conversations.First().Language;
                }

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

                var successMessage = _localizationService.GetMessage("UserConversationsRetrievedSuccessfully", "Messages", language);
                return BaseResponse<List<ConversationTrackingDTO>>.SuccessResponse(conversationDtos, successMessage, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations for user {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("UserConversationsRetrievalError", "Errors", language);
                return BaseResponse<List<ConversationTrackingDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get conversation history for a chat room
        /// </summary>
        public async Task<BaseResponse<List<ConversationTrackingDTO>>> GetRoomConversationsAsync(string roomId, int limit = 20, int offset = 0)
        {
            string language = "ar"; // Default language
            try
            {
                // Get the conversations
                var conversations = await _context.ConversationTrackings
                    .Where(c => c.RoomId == long.Parse(roomId) && c.IsDeleted != true)
                    .OrderByDescending(c => c.Timestamp)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();

                // Try to get language from first conversation
                if (conversations.Any())
                {
                    language = conversations.First().Language;
                }

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

                var successMessage = _localizationService.GetMessage("RoomConversationsRetrievedSuccessfully", "Messages", language);
                return BaseResponse<List<ConversationTrackingDTO>>.SuccessResponse(conversationDtos, successMessage, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations for room {RoomId}", roomId);
                var errorMessage = _localizationService.GetMessage("RoomConversationsRetrievalError", "Errors", language);
                return BaseResponse<List<ConversationTrackingDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get analytics data for all conversations
        /// </summary>
        public async Task<BaseResponse<ConversationAnalyticsDTO>> GetAnalyticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            string language = "ar"; // Default language
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
                var analytics = new ConversationAnalyticsDTO
                {
                    ConversationCount = conversationCount,
                    TopTopics = topicStats,
                    TopKeywords = keywordStats,
                    TopReferencedPdfs = pdfStats,
                    AverageMessagesPerConversation = averageMessages
                };

                var successMessage = _localizationService.GetMessage("AnalyticsRetrievedSuccessfully", "Messages", language);
                return BaseResponse<ConversationAnalyticsDTO>.SuccessResponse(analytics, successMessage, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating analytics");
                var errorMessage = _localizationService.GetMessage("AnalyticsRetrievalError", "Errors", language);
                return BaseResponse<ConversationAnalyticsDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// Get analytics data for a specific user's conversations
        /// </summary>
        public async Task<BaseResponse<ConversationAnalyticsDTO>> GetUserAnalyticsAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            string language = "ar"; // Default language
            try
            {
                if (!long.TryParse(userId, out long userIdLong))
                {
                    _logger.LogWarning("Invalid user ID format: {UserId}", userId);
                    var invalidIdMessage = _localizationService.GetMessage("InvalidUserId", "Errors", language);
                    return BaseResponse<ConversationAnalyticsDTO>.FailureResponse(invalidIdMessage, 400);
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

                // Try to get language from first conversation
                var firstConversation = await query.FirstOrDefaultAsync();
                if (firstConversation != null)
                {
                    language = firstConversation.Language;
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
                var analytics = new ConversationAnalyticsDTO
                {
                    ConversationCount = conversationCount,
                    TopTopics = topicStats,
                    TopKeywords = keywordStats,
                    TopReferencedPdfs = pdfStats,
                    AverageMessagesPerConversation = averageMessages
                };

                var successMessage = _localizationService.GetMessage("UserAnalyticsRetrievedSuccessfully", "Messages", language);
                return BaseResponse<ConversationAnalyticsDTO>.SuccessResponse(analytics, successMessage, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating analytics for user {UserId}", userId);
                var errorMessage = _localizationService.GetMessage("UserAnalyticsRetrievalError", "Errors", language);
                return BaseResponse<ConversationAnalyticsDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تتبع محادثة المستخدم مع الذكاء الاصطناعي
        /// </summary>
        /// <param name="conversationId">معرف المحادثة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="userQuery">استعلام المستخدم</param>
        /// <param name="aiResponse">رد الذكاء الاصطناعي</param>
        /// <param name="topic">موضوع المحادثة</param>
        /// <param name="language">لغة المحادثة</param>
        /// <returns>هل تم التتبع بنجاح</returns>
        public async Task<BaseResponse<bool>> TrackConversationAsync(string conversationId, long userId, string userQuery, string aiResponse, string topic, string language)
        {
            try
            {
                _logger.LogInformation($"Tracking conversation: {conversationId} for user: {userId}");

                var tracking = new ConversationTracking
                {
                    ConversationId = conversationId,
                    UserId = userId,
                    UserQuery = userQuery,
                    AiResponse = aiResponse,
                    Topic = topic,
                    Language = language,
                    CreatedByUserId = userId,
                    CreateDate = DateTime.Now,
                    Timestamp = DateTime.Now,
                    RoomId = 1 // Default room ID if not provided
                };

                await _context.ConversationTrackings.AddAsync(tracking);
                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("ConversationTrackedSuccessfully", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error tracking conversation: {ex.Message}");
                var errorMessage = _localizationService.GetMessage("ConversationTrackingError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }
    }
}