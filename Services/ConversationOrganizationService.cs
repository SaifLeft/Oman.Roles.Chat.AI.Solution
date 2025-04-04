using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs;
using Services.Common;

namespace Services
{
    /// <summary>
    /// واجهة خدمة تنظيم المحادثات
    /// </summary>
    public interface IConversationOrganizationService
    {
        /// <summary>
        /// إنشاء مجلد جديد للمحادثات
        /// </summary>
        Task<BaseResponse<ChatRoomFolderDTO>> CreateFolderAsync(long userId, string folderName, long? parentFolderId, string language);

        /// <summary>
        /// الحصول على قائمة المجلدات للمستخدم
        /// </summary>
        Task<BaseResponse<List<ChatRoomFolderDTO>>> GetUserFoldersAsync(long userId, string language);

        /// <summary>
        /// نقل محادثة إلى مجلد
        /// </summary>
        Task<BaseResponse<bool>> MoveConversationToFolderAsync(long conversationId, int? folderId, long userId, string language);

        /// <summary>
        /// تحديث عنوان المحادثة
        /// </summary>
        Task<BaseResponse<bool>> UpdateConversationTitleAsync(long conversationId, string title, long userId, string language);

        /// <summary>
        /// تحديث وسوم المحادثة
        /// </summary>
        Task<BaseResponse<bool>> UpdateConversationTagsAsync(long conversationId, List<string> tags, long userId, string language);

        /// <summary>
        /// إضافة/إزالة المحادثة من المفضلة
        /// </summary>
        Task<BaseResponse<bool>> ToggleFavoriteAsync(long conversationId, long userId, string language);

        /// <summary>
        /// تحديث حالة المحادثة
        /// </summary>
        Task<BaseResponse<bool>> UpdateConversationStatusAsync(long conversationId, string status, long userId, string language);

        /// <summary>
        /// البحث المتقدم في المحادثات
        /// </summary>
        Task<BaseResponse<PaginatedResponse<List<OrganizedConversationDTO>>>> SearchConversationsAsync(ConversationSearchQuery query, long userId, string language);
    }

    /// <summary>
    /// تنفيذ خدمة تنظيم المحادثات
    /// </summary>
    public class ConversationOrganizationService : IConversationOrganizationService
    {
        private readonly MuhamiContext _context;
        private readonly ILogger<ConversationOrganizationService> _logger;
        private readonly ILocalizationService _localizationService;

        public ConversationOrganizationService(
            MuhamiContext context,
            ILogger<ConversationOrganizationService> logger,
            ILocalizationService localizationService)
        {
            _context = context;
            _logger = logger;
            _localizationService = localizationService;
        }

        /// <summary>
        /// إنشاء مجلد جديد للمحادثات
        /// </summary>
        public async Task<BaseResponse<ChatRoomFolderDTO>> CreateFolderAsync(long userId, string folderName, long? parentFolderId, string language)
        {
            try
            {
                // التحقق من وجود المستخدم
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    var errorMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<ChatRoomFolderDTO>.FailureResponse(errorMessage, 404);
                }

                // التحقق من المجلد الأب إذا تم تحديده
                if (parentFolderId.HasValue)
                {
                    var parentFolder = await _context.ChatRoomFolders
                        .FirstOrDefaultAsync(f => f.Id == parentFolderId.Value && f.UserId == userId && !f.IsDeleted);

                    if (parentFolder == null)
                    {
                        var errorMessage = _localizationService.GetMessage("ParentFolderNotFound", "Errors", language);
                        return BaseResponse<ChatRoomFolderDTO>.FailureResponse(errorMessage, 404);
                    }
                }

                // التحقق من عدم وجود مجلد بنفس الاسم في نفس المستوى
                bool folderExists;
                if (parentFolderId.HasValue)
                {
                    folderExists = await _context.ChatRoomFolders
                        .AnyAsync(f => f.UserId == userId && f.ParentFolderId == parentFolderId && f.Name == folderName && !f.IsDeleted);
                }
                else
                {
                    folderExists = await _context.ChatRoomFolders
                        .AnyAsync(f => f.UserId == userId && f.ParentFolderId == null && f.Name == folderName && !f.IsDeleted);
                }

                if (folderExists)
                {
                    var errorMessage = _localizationService.GetMessage("FolderNameExists", "Errors", language);
                    return BaseResponse<ChatRoomFolderDTO>.FailureResponse(errorMessage, 400);
                }

                // إنشاء المجلد الجديد
                var newFolder = new ChatRoomFolder
                {
                    Name = folderName,
                    UserId = userId,
                    ParentFolderId = parentFolderId,
                    CreateDate = DateTime.UtcNow
                };

                _context.ChatRoomFolders.Add(newFolder);
                await _context.SaveChangesAsync();

                // إعداد DTO للاستجابة
                var folderDto = new ChatRoomFolderDTO
                {
                    Id = newFolder.Id,
                    Name = newFolder.Name,
                    ParentFolderId = newFolder.ParentFolderId,
                    ParentFolderName = parentFolderId.HasValue
                        ? (await _context.ChatRoomFolders.FirstOrDefaultAsync(f => f.Id == parentFolderId.Value))?.Name
                        : null,
                    SubFolders = new List<ChatRoomFolderDTO>(),
                    ConversationCount = 0
                };

                var successMessage = _localizationService.GetMessage("FolderCreated", "Messages", language);
                return BaseResponse<ChatRoomFolderDTO>.SuccessResponse(folderDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إنشاء مجلد جديد للمحادثات");
                var errorMessage = _localizationService.GetMessage("FolderCreationError", "Errors", language);
                return BaseResponse<ChatRoomFolderDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على قائمة المجلدات للمستخدم
        /// </summary>
        public async Task<BaseResponse<List<ChatRoomFolderDTO>>> GetUserFoldersAsync(long userId, string language)
        {
            try
            {
                // التحقق من وجود المستخدم
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    var errorMessage = _localizationService.GetMessage("UserNotFound", "Errors", language);
                    return BaseResponse<List<ChatRoomFolderDTO>>.FailureResponse(errorMessage, 404);
                }

                // الحصول على جميع المجلدات للمستخدم
                var folders = await _context.ChatRoomFolders
                    .Where(f => f.UserId == userId && !f.IsDeleted)
                    .OrderBy(f => f.Name)
                    .ToListAsync();

                // إنشاء نموذج المجلدات بشكل هرمي
                var folderDtos = BuildFolderHierarchy(folders);

                // إضافة عدد المحادثات في كل مجلد
                await AddConversationCountsToFolders(folderDtos);

                var successMessage = _localizationService.GetMessage("FoldersRetrieved", "Messages", language);
                return BaseResponse<List<ChatRoomFolderDTO>>.SuccessResponse(folderDtos, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء الحصول على قائمة المجلدات للمستخدم");
                var errorMessage = _localizationService.GetMessage("FoldersRetrievalError", "Errors", language);
                return BaseResponse<List<ChatRoomFolderDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// نقل محادثة إلى مجلد
        /// </summary>
        public async Task<BaseResponse<bool>> MoveConversationToFolderAsync(long conversationId, int? folderId, long userId, string language)
        {
            try
            {
                // التحقق من وجود المحادثة وملكيتها للمستخدم
                var conversation = await _context.ChatRooms
                    .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId && !c.IsDeleted);

                if (conversation == null)
                {
                    var errorMessage = _localizationService.GetMessage("ConversationNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 404);
                }

                // التحقق من وجود المجلد إذا تم تحديده
                if (folderId.HasValue)
                {
                    var folder = await _context.ChatRoomFolders
                        .FirstOrDefaultAsync(f => f.Id == folderId.Value && f.UserId == userId && !f.IsDeleted);

                    if (folder == null)
                    {
                        var errorMessage = _localizationService.GetMessage("FolderNotFound", "Errors", language);
                        return BaseResponse<bool>.FailureResponse(errorMessage, 404);
                    }

                    // تحديث مسار المجلد
                    conversation.FolderPath = await BuildFolderPathAsync(folderId.Value);
                }
                else
                {
                    // إزالة من المجلد (نقل إلى الجذر)
                    conversation.FolderPath = null;
                }

                // تحديث آخر نشاط
                conversation.LastActivityAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("ConversationMoved", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء نقل محادثة إلى مجلد");
                var errorMessage = _localizationService.GetMessage("ConversationMoveError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث عنوان المحادثة
        /// </summary>
        public async Task<BaseResponse<bool>> UpdateConversationTitleAsync(long conversationId, string title, long userId, string language)
        {
            try
            {
                // التحقق من وجود المحادثة وملكيتها للمستخدم
                var conversation = await _context.ChatRooms
                    .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId && !c.IsDeleted);

                if (conversation == null)
                {
                    var errorMessage = _localizationService.GetMessage("ConversationNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 404);
                }

                // تحديث العنوان
                conversation.Title = title;
                conversation.LastActivityAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("ConversationTitleUpdated", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث عنوان المحادثة");
                var errorMessage = _localizationService.GetMessage("ConversationTitleUpdateError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث وسوم المحادثة
        /// </summary>
        public async Task<BaseResponse<bool>> UpdateConversationTagsAsync(long conversationId, List<string> tags, long userId, string language)
        {
            try
            {
                // التحقق من وجود المحادثة وملكيتها للمستخدم
                var conversation = await _context.ChatRooms
                    .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId && !c.IsDeleted);

                if (conversation == null)
                {
                    var errorMessage = _localizationService.GetMessage("ConversationNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 404);
                }

                // تحويل قائمة الوسوم إلى نص مفصول بفواصل
                conversation.Tags = string.Join(",", tags);
                conversation.LastActivityAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("ConversationTagsUpdated", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث وسوم المحادثة");
                var errorMessage = _localizationService.GetMessage("ConversationTagsUpdateError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إضافة/إزالة المحادثة من المفضلة
        /// </summary>
        public async Task<BaseResponse<bool>> ToggleFavoriteAsync(long conversationId, long userId, string language)
        {
            try
            {
                // التحقق من وجود المحادثة وملكيتها للمستخدم
                var conversation = await _context.ChatRooms
                    .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId && !c.IsDeleted);

                if (conversation == null)
                {
                    var errorMessage = _localizationService.GetMessage("ConversationNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 404);
                }

                // تبديل حالة المفضلة
                conversation.IsFavorite = !conversation.IsFavorite;
                conversation.LastActivityAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                string messageKey = conversation.IsFavorite ? "ConversationAddedToFavorites" : "ConversationRemovedFromFavorites";
                var successMessage = _localizationService.GetMessage(messageKey, "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تبديل حالة المفضلة للمحادثة");
                var errorMessage = _localizationService.GetMessage("ConversationFavoriteToggleError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث حالة المحادثة
        /// </summary>
        public async Task<BaseResponse<bool>> UpdateConversationStatusAsync(long conversationId, string status, long userId, string language)
        {
            try
            {
                // التحقق من وجود المحادثة وملكيتها للمستخدم
                var conversation = await _context.ChatRooms
                    .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId && !c.IsDeleted);

                if (conversation == null)
                {
                    var errorMessage = _localizationService.GetMessage("ConversationNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 404);
                }

                // التحقق من صحة الحالة
                var validStatuses = new[] { "Active", "Archived", "Completed", "Pending" };
                if (!validStatuses.Contains(status))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidConversationStatus", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 400);
                }

                // تحديث الحالة
                conversation.Status = status;
                conversation.LastActivityAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("ConversationStatusUpdated", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث حالة المحادثة");
                var errorMessage = _localizationService.GetMessage("ConversationStatusUpdateError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// البحث المتقدم في المحادثات
        /// </summary>
        public async Task<BaseResponse<PaginatedResponse<List<OrganizedConversationDTO>>>> SearchConversationsAsync(ConversationSearchQuery query, long userId, string language)
        {
            try
            {
                // بناء استعلام أساسي
                var conversationsQuery = _context.ChatRooms
                    .Where(c => c.UserId == userId && !c.IsDeleted)
                    .AsQueryable();

                // تطبيق معايير البحث

                // البحث النصي
                if (!string.IsNullOrWhiteSpace(query.SearchText))
                {
                    conversationsQuery = conversationsQuery.Where(c =>
                        (c.Title != null && c.Title.Contains(query.SearchText)) ||
                        (c.Tags != null && c.Tags.Contains(query.SearchText))
                    );
                }

                // البحث في المفضلة
                if (query.IsFavorite.HasValue)
                {
                    conversationsQuery = conversationsQuery.Where(c => c.IsFavorite == query.IsFavorite.Value);
                }

                // البحث حسب الحالة
                if (!string.IsNullOrWhiteSpace(query.Status))
                {
                    conversationsQuery = conversationsQuery.Where(c => c.Status == query.Status);
                }

                // البحث حسب المجلد
                if (query.FolderId.HasValue)
                {
                    // بناء مسار المجلد للبحث
                    var folderPath = await BuildFolderPathAsync(query.FolderId.Value);
                    conversationsQuery = conversationsQuery.Where(c => c.FolderPath == folderPath);
                }

                // البحث حسب التاريخ
                if (query.FromDate.HasValue)
                {
                    conversationsQuery = conversationsQuery.Where(c => c.CreateDate >= query.FromDate.Value);
                }

                if (query.ToDate.HasValue)
                {
                    var endDate = query.ToDate.Value.AddDays(1).AddSeconds(-1); // نهاية اليوم
                    conversationsQuery = conversationsQuery.Where(c => c.CreateDate <= endDate);
                }

                // البحث حسب الوسوم
                if (query.Tags != null && query.Tags.Any())
                {
                    foreach (var tag in query.Tags)
                    {
                        var tagToSearch = tag.Trim();
                        conversationsQuery = conversationsQuery.Where(c => c.Tags != null && c.Tags.Contains(tagToSearch));
                    }
                }

                // تحديد الترتيب
                IOrderedQueryable<ChatRoom> orderedQuery;



                // Update the switch statement to use the enum
                switch (query.OrderBy)
                {
                    case OrderByOptions.CreateDate:
                        orderedQuery = query.Descending
                            ? conversationsQuery.OrderByDescending(c => c.CreateDate)
                            : conversationsQuery.OrderBy(c => c.CreateDate);
                        break;
                    case OrderByOptions.Title:
                        orderedQuery = query.Descending
                            ? conversationsQuery.OrderByDescending(c => c.Title)
                            : conversationsQuery.OrderBy(c => c.Title);
                        break;
                    case OrderByOptions.LastActivity:
                    default:
                        orderedQuery = query.Descending
                            ? conversationsQuery.OrderByDescending(c => c.LastActivityAt ?? c.CreateDate)
                            : conversationsQuery.OrderBy(c => c.LastActivityAt ?? c.CreateDate);
                        break;
                }

                // تنفيذ الاستعلام مع التصفح
                long totalCount = await orderedQuery.CountAsync();
                long skip = (query.Page - 1) * query.PageSize;

                var conversations = await orderedQuery
                    .Skip(int.Parse(skip.ToString()))
                    .Take(int.Parse(query.PageSize.ToString()))
                    .ToListAsync();

                // تحويل النتائج إلى DTOs
                var conversationDtos = new List<OrganizedConversationDTO>();

                foreach (var conversation in conversations)
                {
                    // استخراج قائمة الوسوم
                    var tags = string.IsNullOrEmpty(conversation.Tags)
                        ? new List<string>()
                        : conversation.Tags.Split(',').Select(t => t.Trim()).ToList();

                    // الحصول على فئات المحادثة
                    var categories = await GetConversationCategoriesAsync(conversation.Id);

                    var dto = new OrganizedConversationDTO
                    {
                        Id = conversation.Id,
                        Title = conversation.Title ?? $"محادثة {conversation.Id}",
                        IsFavorite = conversation.IsFavorite,
                        Status = conversation.Status,
                        FolderPath = conversation.FolderPath,
                        CreateDate = conversation.CreateDate,
                        LastActivity = conversation.LastActivityAt,
                        Tags = tags,
                        Categories = categories
                    };

                    conversationDtos.Add(dto);
                }

                // إنشاء قائمة متصفحة
                var paginatedResult = new PaginatedResponse<List<OrganizedConversationDTO>>
                {
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (long)Math.Ceiling((double)totalCount / query.PageSize),
                    Data = conversationDtos


                };

                var successMessage = _localizationService.GetMessage("ConversationsSearched", "Messages", language);
                return BaseResponse<PaginatedResponse<List<OrganizedConversationDTO>>>.SuccessResponse(paginatedResult, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء البحث في المحادثات");
                var errorMessage = _localizationService.GetMessage("ConversationsSearchError", "Errors", language);
                return BaseResponse<PaginatedResponse<List<OrganizedConversationDTO>>>.FailureResponse(errorMessage, 500);
            }
        }

        #region Helper Methods

        /// <summary>
        /// بناء التسلسل الهرمي للمجلدات
        /// </summary>
        private List<ChatRoomFolderDTO> BuildFolderHierarchy(List<ChatRoomFolder> folders)
        {
            // قاموس للمجلدات لسهولة الوصول
            var folderDict = new Dictionary<long, ChatRoomFolderDTO>();

            // إنشاء DTOs للمجلدات
            foreach (var folder in folders)
            {
                var folderDto = new ChatRoomFolderDTO
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    ParentFolderId = folder.ParentFolderId,
                    SubFolders = new List<ChatRoomFolderDTO>()
                };

                folderDict[folder.Id] = folderDto;
            }

            // قائمة المجلدات الجذرية
            var rootFolders = new List<ChatRoomFolderDTO>();

            // بناء التسلسل الهرمي
            foreach (var folder in folders)
            {
                if (folder.ParentFolderId.HasValue && folderDict.ContainsKey(folder.ParentFolderId.Value))
                {
                    var parentFolder = folderDict[folder.ParentFolderId.Value];
                    parentFolder.SubFolders.Add(folderDict[folder.Id]);
                }
                else
                {
                    rootFolders.Add(folderDict[folder.Id]);
                }
            }

            return rootFolders;
        }

        /// <summary>
        /// إضافة عدد المحادثات لكل مجلد
        /// </summary>
        private async Task AddConversationCountsToFolders(List<ChatRoomFolderDTO> folders)
        {
            foreach (var folder in folders)
            {
                // بناء مسار المجلد
                var folderPath = await BuildFolderPathAsync(folder.Id);

                // عدد المحادثات في هذا المجلد
                folder.ConversationCount = await _context.ChatRooms
                    .CountAsync(c => c.FolderPath == folderPath && !c.IsDeleted);

                // استدعاء متكرر للمجلدات الفرعية
                if (folder.SubFolders.Any())
                {
                    await AddConversationCountsToFolders(folder.SubFolders);
                }
            }
        }

        /// <summary>
        /// بناء مسار المجلد المستخدم في حقل FolderPath
        /// </summary>
        private async Task<string> BuildFolderPathAsync(long folderId)
        {
            var pathParts = new List<string>();
            var currentFolderId = folderId;

            while (currentFolderId > 0)
            {
                var folder = await _context.ChatRoomFolders.FirstOrDefaultAsync(f => f.Id == currentFolderId);
                if (folder == null)
                {
                    break;
                }

                pathParts.Insert(0, folder.Name);
                currentFolderId = folder.ParentFolderId ?? 0;
            }

            return string.Join("/", pathParts);
        }

        /// <summary>
        /// الحصول على فئات المحادثة
        /// </summary>
        private async Task<List<string>> GetConversationCategoriesAsync(long conversationId)
        {
            // الحصول على قائمة الرسائل في المحادثة
            var messageIds = await _context.ChatMessages
                .Where(m => m.ChatRoomId == conversationId && m.Role == nameof(UserRole.USER) && !m.IsDeleted)
                .Select(m => m.Id)
                .ToListAsync();

            if (!messageIds.Any())
            {
                return new List<string>();
            }

            // الحصول على فئات الرسائل
            var categories = await _context.MessageCategories
                .Where(c => messageIds.Contains(c.MessageId) && !c.IsDeleted)
                .Join(_context.LegalCategories,
                    mc => mc.LegalCategoryId,
                    lc => lc.Id,
                    (mc, lc) => lc.Name)
                .Distinct()
                .ToListAsync();

            return categories;
        }

        #endregion
    }


}