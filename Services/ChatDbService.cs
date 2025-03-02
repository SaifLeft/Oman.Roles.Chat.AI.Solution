using API.Services;
using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using System.Collections.Concurrent;

namespace Services
{
    /// <summary>
    /// واجهة خدمة التعامل مع قاعدة بيانات المحادثات
    /// </summary>
    public interface IChatDbService
    {
        /// <summary>
        /// إنشاء غرفة دردشة جديدة
        /// </summary>
        /// <param name="title">عنوان الغرفة</param>
        /// <param name="description">وصف الغرفة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <returns>معرف الغرفة الجديدة</returns>
        Task<long> CreateChatRoomAsync(string title, string description, long userId);

        /// <summary>
        /// الحصول على غرفة دردشة بمعرفها
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <returns>معلومات الغرفة</returns>
        Task<ChatRoom> GetChatRoomByIdAsync(long roomId);

        /// <summary>
        /// الحصول على غرف الدردشة للمستخدم
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <returns>قائمة بغرف الدردشة</returns>
        Task<List<ChatRoom>> GetChatRoomsByUserIdAsync(long userId);

        /// <summary>
        /// إضافة رسالة دردشة جديدة
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <param name="senderId">معرف المرسل</param>
        /// <param name="role">دور المرسل (مستخدم أو نظام)</param>
        /// <param name="content">محتوى الرسالة</param>
        /// <returns>معرف الرسالة الجديدة</returns>
        Task<long> AddChatMessageAsync(long roomId, string senderId, string role, string content);

        /// <summary>
        /// الحصول على رسائل غرفة دردشة
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <param name="limit">عدد الرسائل المطلوبة (الأحدث أولاً)</param>
        /// <returns>قائمة بالرسائل</returns>
        Task<List<ChatMessage>> GetChatRoomMessagesAsync(long roomId, int limit = 50);

        /// <summary>
        /// تحديث تاريخ آخر نشاط في غرفة الدردشة
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <returns>مهمة غير متزامنة</returns>
        Task UpdateChatRoomLastActivityAsync(long roomId);

        /// <summary>
        /// حذف غرفة دردشة (حذف منطقي)
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <returns>نجاح العملية</returns>
        Task<bool> DeleteChatRoomAsync(long roomId, long userId);

        /// <summary>
        /// تحويل كائن ChatRoom من قاعدة البيانات إلى نموذج
        /// </summary>
        /// <param name="dbChatRoom">كائن غرفة الدردشة من قاعدة البيانات</param>
        /// <param name="includeMessages">تضمين الرسائل</param>
        /// <returns>نموذج غرفة الدردشة</returns>
        ChatRoomDTO ConvertToChatRoomModel(Data.Structure.ChatRoom dbChatRoom, bool includeMessages = true);

        /// <summary>
        /// تحويل كائن ChatMessage من قاعدة البيانات إلى نموذج
        /// </summary>
        /// <param name="dbChatMessage">كائن رسالة الدردشة من قاعدة البيانات</param>
        /// <returns>نموذج رسالة الدردشة</returns>
        ChatMessageDTO ConvertToChatMessageModel(Data.Structure.ChatMessage dbChatMessage);
    }

    /// <summary>
    /// تنفيذ خدمة التعامل مع قاعدة بيانات المحادثات
    /// </summary>
    public class ChatDbService : IChatDbService
    {
        private readonly MuhamiContext _context;
        private readonly ILogger<ChatDbService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;

        // ذاكرة تخزين مؤقت للغرف والرسائل (للتحسين)
        private readonly ConcurrentDictionary<long, Models.ChatRoomDTO> _chatRoomsCache = new();
        private readonly ConcurrentDictionary<long, List<Models.ChatMessageDTO>> _chatMessagesCache = new();

        /// <summary>
        /// إنشاء مثيل جديد من خدمة التعامل مع قاعدة بيانات المحادثات
        /// </summary>
        public ChatDbService(
            MuhamiContext context,
            ILogger<ChatDbService> logger,
            ILocalizationService localizationService,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;
        }

        /// <summary>
        /// إنشاء غرفة دردشة جديدة
        /// </summary>
        public async Task<long> CreateChatRoomAsync(string title, string description, long userId)
        {
            try
            {
                _logger.LogInformation("إنشاء غرفة دردشة جديدة: {Title} بواسطة المستخدم: {UserId}", title, userId);

                var chatRoom = new Data.Structure.ChatRoom
                {
                    Title = title,
                    Description = description,
                    LastActivityAt = DateTime.UtcNow,
                    CreatedByUserId = userId,
                    CreateDate = DateTime.UtcNow
                };

                _context.ChatRooms.Add(chatRoom);
                await _context.SaveChangesAsync();

                // حذف الغرفة من الذاكرة المؤقتة إذا كانت موجودة
                _chatRoomsCache.TryRemove(chatRoom.Id, out _);

                return chatRoom.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء غرفة دردشة جديدة للمستخدم: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// الحصول على غرفة دردشة بمعرفها
        /// </summary>
        public async Task<ChatRoomDTO> GetChatRoomByIdAsync(long roomId)
        {
            try
            {
                _logger.LogInformation("الحصول على غرفة الدردشة: {RoomId}", roomId);

                var dbChatRoom = await _context.ChatRooms
                    .FirstOrDefaultAsync(cr => cr.Id == roomId && cr.IsDeleted != true);

                if (dbChatRoom == null)
                {
                    _logger.LogWarning("غرفة الدردشة غير موجودة: {RoomId}", roomId);
                    return null;
                }

                return ConvertToChatRoomModel(dbChatRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على غرفة الدردشة: {RoomId}", roomId);
                throw;
            }
        }

        /// <summary>
        /// الحصول على غرف الدردشة للمستخدم
        /// </summary>
        public async Task<List<ChatRoom>> GetChatRoomsByUserIdAsync(long userId)
        {
            try
            {
                _logger.LogInformation("الحصول على غرف الدردشة للمستخدم: {UserId}", userId);

                var dbChatRooms = await _context.ChatRooms
                    .Where(cr => cr.CreatedByUserId == userId && cr.IsDeleted != true)
                    .OrderByDescending(cr => cr.LastActivityAt)
                    .ToListAsync();

                var chatRooms = new List<ChatRoom>();
                foreach (var dbChatRoom in dbChatRooms)
                {
                    chatRooms.Add(ConvertToChatRoomModel(dbChatRoom, false)); // بدون تحميل الرسائل للأداء
                }

                return chatRooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على غرف الدردشة للمستخدم: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// إضافة رسالة دردشة جديدة
        /// </summary>
        public async Task<long> AddChatMessageAsync(long roomId, string senderId, string role, string content)
        {
            try
            {
                _logger.LogInformation("إضافة رسالة جديدة إلى الغرفة: {RoomId}", roomId);

                // التحقق من وجود الغرفة
                var chatRoom = await _context.ChatRooms
                    .FirstOrDefaultAsync(cr => cr.Id == roomId && cr.IsDeleted != true);

                if (chatRoom == null)
                {
                    _logger.LogWarning("غرفة الدردشة غير موجودة: {RoomId}", roomId);
                    throw new ArgumentException($"غرفة الدردشة غير موجودة: {roomId}");
                }

                // إنشاء رسالة جديدة
                var chatMessage = new Data.Structure.ChatMessage
                {
                    ChatRoomId = roomId,
                    SenderId = senderId,
                    Role = role,
                    Content = content,
                    Timestamp = DateTime.UtcNow,
                    CreatedByUserId = long.Parse(senderId),
                    CreateDate = DateTime.UtcNow
                };

                _context.ChatMessages.Add(chatMessage);

                // تحديث تاريخ آخر نشاط في الغرفة
                chatRoom.LastActivityAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // حذف رسائل الغرفة من الذاكرة المؤقتة لإعادة تحميلها
                _chatMessagesCache.TryRemove(roomId, out _);

                return chatMessage.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إضافة رسالة جديدة إلى الغرفة: {RoomId}", roomId);
                throw;
            }
        }

        /// <summary>
        /// الحصول على رسائل غرفة دردشة
        /// </summary>
        public async Task<List<ChatMessage>> GetChatRoomMessagesAsync(long roomId, int limit = 50)
        {
            try
            {
                _logger.LogInformation("الحصول على رسائل الغرفة: {RoomId}, الحد: {Limit}", roomId, limit);

                // محاولة الحصول على الرسائل من الذاكرة المؤقتة
                if (_chatMessagesCache.TryGetValue(roomId, out var cachedMessages))
                {
                    return cachedMessages.Take(limit).ToList();
                }

                // تحميل الرسائل من قاعدة البيانات
                var dbChatMessages = await _context.ChatMessages
                    .Where(cm => cm.ChatRoomId == roomId && cm.IsDeleted != true)
                    .OrderByDescending(cm => cm.Timestamp)
                    .Take(limit)
                    .ToListAsync();

                var chatMessages = new List<Models.ChatMessageDTO>();
                foreach (var dbChatMessage in dbChatMessages)
                {
                    chatMessages.Add(ConvertToChatMessageModel(dbChatMessage));
                }

                // تخزين الرسائل في الذاكرة المؤقتة
                _chatMessagesCache[roomId] = chatMessages;

                return chatMessages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على رسائل الغرفة: {RoomId}", roomId);
                throw;
            }
        }

        /// <summary>
        /// تحديث تاريخ آخر نشاط في غرفة الدردشة
        /// </summary>
        public async Task UpdateChatRoomLastActivityAsync(long roomId)
        {
            try
            {
                _logger.LogInformation("تحديث تاريخ آخر نشاط للغرفة: {RoomId}", roomId);

                var chatRoom = await _context.ChatRooms
                    .FirstOrDefaultAsync(cr => cr.Id == roomId && cr.IsDeleted != true);

                if (chatRoom == null)
                {
                    _logger.LogWarning("غرفة الدردشة غير موجودة: {RoomId}", roomId);
                    return;
                }

                chatRoom.LastActivityAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // حذف الغرفة من الذاكرة المؤقتة
                _chatRoomsCache.TryRemove(roomId, out _);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث تاريخ آخر نشاط للغرفة: {RoomId}", roomId);
                throw;
            }
        }

        /// <summary>
        /// حذف غرفة دردشة (حذف منطقي)
        /// </summary>
        public async Task<bool> DeleteChatRoomAsync(long roomId, long userId)
        {
            try
            {
                _logger.LogInformation("حذف غرفة الدردشة: {RoomId} بواسطة المستخدم: {UserId}", roomId, userId);

                var chatRoom = await _context.ChatRooms
                    .FirstOrDefaultAsync(cr => cr.Id == roomId && cr.IsDeleted != true);

                if (chatRoom == null)
                {
                    _logger.LogWarning("غرفة الدردشة غير موجودة: {RoomId}", roomId);
                    return false;
                }

                if (chatRoom.CreatedByUserId != userId)
                {
                    _logger.LogWarning("غير مصرح للمستخدم: {UserId} بحذف الغرفة: {RoomId}", userId, roomId);
                    return false;
                }

                // حذف منطقي للغرفة
                chatRoom.IsDeleted = true;
                chatRoom.ModifiedByUserId = userId;
                chatRoom.ModifiedDate = DateTime.UtcNow;

                // حذف منطقي لجميع رسائل الغرفة
                var chatMessages = await _context.ChatMessages
                    .Where(cm => cm.ChatRoomId == roomId && cm.IsDeleted != true)
                    .ToListAsync();

                foreach (var message in chatMessages)
                {
                    message.IsDeleted = true;
                    message.ModifiedByUserId = userId;
                    message.ModifiedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // حذف الغرفة والرسائل من الذاكرة المؤقتة
                _chatRoomsCache.TryRemove(roomId, out _);
                _chatMessagesCache.TryRemove(roomId, out _);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف غرفة الدردشة: {RoomId}", roomId);
                throw;
            }
        }

        /// <summary>
        /// تحويل كائن ChatRoom من قاعدة البيانات إلى نموذج
        /// </summary>
        public Models.ChatRoomDTO ConvertToChatRoomModel(ChatRoom dbChatRoom, bool includeMessages = true)
        {
            var chatRoom = new Models.ChatRoomDTO
            {
                Id = dbChatRoom.Id.ToString(),
                Title = dbChatRoom.Title,
                Description = dbChatRoom.Description ?? string.Empty,
                CreatedBy = dbChatRoom.CreatedByUserId.ToString(),
                CreatedAt = dbChatRoom.CreateDate,
                Messages = new List<Models.ChatMessageDTO>(),
                PdfFiles = new List<string>(), // سيتم تنفيذ ربط ملفات PDF في مستوى أعلى
                Rules = string.Empty // سيتم تنفيذ ربط القواعد في مستوى أعلى
            };

            if (includeMessages)
            {
                // تحميل الرسائل إذا كانت مضمنة
                var dbMessages = _context.ChatMessages
                    .Where(cm => cm.ChatRoomId == dbChatRoom.Id && cm.IsDeleted != true)
                    .OrderBy(cm => cm.Timestamp)
                    .ToList();

                foreach (var dbMessage in dbMessages)
                {
                    chatRoom.Messages.Add(ConvertToChatMessageModel(dbMessage));
                }
            }

            // تخزين في الذاكرة المؤقتة
            _chatRoomsCache[dbChatRoom.Id] = chatRoom;

            return chatRoom;
        }

        /// <summary>
        /// تحويل كائن ChatMessage من قاعدة البيانات إلى نموذج
        /// </summary>
        public ChatMessageDTO ConvertToChatMessageModel(ChatMessage dbChatMessage)
        {
            return new ChatMessageDTO
            {
                Id = dbChatMessage.Id,
                Role = dbChatMessage.Role,
                Content = dbChatMessage.Content,
                Timestamp = dbChatMessage.Timestamp
            };
        }
    }
}