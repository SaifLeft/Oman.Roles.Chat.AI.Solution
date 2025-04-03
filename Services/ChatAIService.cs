using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Models.Common;

namespace Services
{
    /// <summary>
    /// واجهة خدمة الدردشة الذكية للمحامي الذكي
    /// </summary>
    public interface IChatAIService
    {
        /// <summary>
        /// إنشاء غرفة دردشة جديدة
        /// </summary>
        /// <param name="request">طلب إنشاء غرفة دردشة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>معلومات غرفة الدردشة المنشأة</returns>
        Task<BaseResponse<ChatRoomDTO>> CreateChatRoomAsync(CreateChatRoomRequest request, string userId, string language);

        /// <summary>
        /// إرسال رسالة واستلام الرد من النظام الذكي
        /// </summary>
        /// <param name="request">طلب إرسال الرسالة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>معلومات الرد من النظام</returns>
        Task<BaseResponse<ChatResponseModel>> SendMessageAsync(SendMessageRequest request, string userId, string language);

        /// <summary>
        /// الحصول على غرفة دردشة بمعرفها
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>معلومات غرفة الدردشة</returns>
        Task<BaseResponse<ChatRoomDTO>> GetChatRoomAsync(string roomId, string userId, string language);

        /// <summary>
        /// الحصول على تاريخ المحادثات لغرفة دردشة
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>قائمة بالرسائل المتبادلة</returns>
        Task<BaseResponse<List<ChatMessageDTO>>> GetChatHistoryAsync(string roomId, string userId, string language);

        /// <summary>
        /// الحصول على جميع غرف الدردشة الخاصة بالمستخدم
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>قائمة بغرف الدردشة</returns>
        Task<BaseResponse<List<ChatRoomDTO>>> GetUserChatRoomsAsync(string userId, string language);

        /// <summary>
        /// حذف غرفة دردشة
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>نتيجة العملية</returns>
        Task<BaseResponse<bool>> DeleteChatRoomAsync(string roomId, string userId, string language);
    }

    /// <summary>
    /// تنفيذ خدمة الدردشة الذكية
    /// </summary>
    public class ChatAIService : IChatAIService
    {
        private readonly IDeepSeekService _deepSeekService;
        private readonly IPdfExtractionService _pdfExtractionService;
        private readonly IChatRulesService _chatRulesService;
        private readonly ILegalContextService _legalContextService;
        private readonly IChatDbService _chatDbService;
        private readonly IConversationTrackingService _conversationTrackingService;
        private readonly ILogger<ChatAIService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;

        private readonly string _pdfBasePath;

        /// <summary>
        /// إنشاء مثيل جديد من خدمة الدردشة الذكية
        /// </summary>
        public ChatAIService(
            IDeepSeekService deepSeekService,
            IPdfExtractionService pdfExtractionService,
            IChatRulesService chatRulesService,
            ILegalContextService legalContextService,
            IChatDbService chatDbService,
            IConversationTrackingService conversationTrackingService,
            ILogger<ChatAIService> logger,
            ILocalizationService localizationService,
            IConfiguration configuration)
        {
            _deepSeekService = deepSeekService;
            _pdfExtractionService = pdfExtractionService;
            _chatRulesService = chatRulesService;
            _legalContextService = legalContextService;
            _chatDbService = chatDbService;
            _conversationTrackingService = conversationTrackingService;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;

            // تحديد مسار البيانات
            _pdfBasePath = _configuration["ChatSettings:PdfBasePath"] ??
                           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfFiles");
        }

        /// <summary>
        /// إنشاء غرفة دردشة جديدة
        /// </summary>
        public async Task<BaseResponse<ChatRoomDTO>> CreateChatRoomAsync(CreateChatRoomRequest request, string userId, string language)
        {
            try
            {
                _logger.LogInformation("إنشاء غرفة دردشة جديدة بواسطة المستخدم: {UserId}", userId);

                // التحقق من المدخلات الأساسية
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    var errorMessage = _localizationService.GetMessage("ChatRoomTitleRequired", "Errors", language);
                    return BaseResponse<ChatRoomDTO>.FailureResponse(errorMessage, 400);
                }

                // التحقق من وجود ملفات PDF المحددة
                if (request.PdfFiles?.Count > 0)
                {
                    foreach (var pdfFile in request.PdfFiles)
                    {
                        if (!File.Exists(Path.Combine(_pdfBasePath, pdfFile)))
                        {
                            var errorMessage = _localizationService.GetMessage("PdfFileNotFound", "Errors", language);
                            return BaseResponse<ChatRoomDTO>.FailureResponse(string.Format(errorMessage, pdfFile), 400);
                        }
                    }
                }

                // الحصول على قواعد الدردشة
                string chatRules = !string.IsNullOrEmpty(request.Rules)
                    ? request.Rules
                    : _chatRulesService.GetDefaultRules(language);

                // إنشاء غرفة دردشة في قاعدة البيانات
                long chatRoomId = await _chatDbService.CreateChatRoomAsync(
                    request.Title,
                    request.Description ?? string.Empty,
                    long.Parse(userId)
                );

                // إضافة رسالة ترحيبية
                var welcomeMessage = _localizationService.GetMessage("ChatRoomWelcomeMessage", "Messages", language);
                await _chatDbService.AddChatMessageAsync(
                    chatRoomId,
                    "0", // معرف النظام
                    "system",
                    string.Format(welcomeMessage, request.Title)
                );

                // الحصول على بيانات الغرفة بعد الإنشاء
                var dbChatRoom = await _chatDbService.GetChatRoomByIdAsync(chatRoomId);
                if (dbChatRoom == null)
                {
                    var errorMessage = _localizationService.GetMessage("ChatRoomCreationError", "Errors", language);
                    return BaseResponse<ChatRoomDTO>.FailureResponse(errorMessage, 500);
                }

                // إضافة المعلومات الإضافية للغرفة
                dbChatRoom.PdfFiles = request.PdfFiles ?? new List<string>();
                dbChatRoom.Rules = chatRules;

                var successMessage = _localizationService.GetMessage("ChatRoomCreated", "Messages", language);
                return BaseResponse<ChatRoomDTO>.SuccessResponse(dbChatRoom, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إنشاء غرفة الدردشة");
                var errorMessage = _localizationService.GetMessage("ChatRoomCreationError", "Errors", language);
                return BaseResponse<ChatRoomDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// إرسال رسالة والحصول على رد من النظام الذكي
        /// </summary>
        public async Task<BaseResponse<ChatResponseModel>> SendMessageAsync(SendMessageRequest request, string userId, string language)
        {
            try
            {
                _logger.LogInformation("معالجة رسالة جديدة في الغرفة: {RoomId} من المستخدم: {UserId}", request.RoomId, userId);

                // التحقق من وجود غرفة الدردشة
                var chatRoom = await _chatDbService.GetChatRoomByIdAsync(long.Parse(request.RoomId));
                if (chatRoom == null)
                {
                    var errorMessage = _localizationService.GetMessage("ChatRoomNotFound", "Errors", language);
                    return BaseResponse<ChatResponseModel>.FailureResponse(errorMessage, 404);
                }

                // التحقق من أن المستخدم يملك الغرفة
                if (chatRoom.CreatedBy != userId)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedChatRoomAccess", "Errors", language);
                    return BaseResponse<ChatResponseModel>.FailureResponse(errorMessage, 403);
                }

                // التحقق من محتوى الرسالة
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    var errorMessage = _localizationService.GetMessage("MessageContentRequired", "Errors", language);
                    return BaseResponse<ChatResponseModel>.FailureResponse(errorMessage, 400);
                }

                // التحقق من شرعية الاستعلام
                var validationResult = await _legalContextService.ValidateQueryAsync(request.Content, language);
                if (!validationResult.IsValid)
                {
                    return BaseResponse<ChatResponseModel>.FailureResponse(validationResult.Message, 400);
                }

                // إضافة رسالة المستخدم إلى قاعدة البيانات
                long userMessageId = await _chatDbService.AddChatMessageAsync(
                    long.Parse(request.RoomId),
                    userId,
                    "user",
                    request.Content
                );

                // تحديث تاريخ آخر نشاط للغرفة
                await _chatDbService.UpdateChatRoomLastActivityAsync(long.Parse(request.RoomId));

                // استخراج نصوص الملفات المرتبطة بالغرفة
                var pdfContents = await GetPdfContentsAsync(chatRoom.PdfFiles);

                // البحث عن ملفات PDF ذات صلة إذا لم يتم تحديدها مسبقاً
                if (chatRoom.PdfFiles.Count == 0)
                {
                    var availablePdfFiles = Directory.GetFiles(_pdfBasePath, "*.pdf")
                        .Select(Path.GetFileName)
                        .ToList();

                    var relevantFiles = await _legalContextService.FindRelevantPdfFilesAsync(
                        request.Content,
                        availablePdfFiles,
                        language
                    );

                    // إضافة الملفات ذات الصلة
                    if (relevantFiles.Count > 0)
                    {
                        chatRoom.PdfFiles.AddRange(relevantFiles.Take(3)); // نقتصر على 3 ملفات كحد أقصى
                        pdfContents = await GetPdfContentsAsync(chatRoom.PdfFiles);
                    }
                }

                // إثراء السياق القانوني
                var enrichedContext = await _legalContextService.EnrichLegalContextAsync(
                    request.Content,
                    pdfContents,
                    language
                );

                // الحصول على رد من نموذج الذكاء الاصطناعي
                _logger.LogInformation("جاري الحصول على الرد من نموذج الذكاء الاصطناعي");
                var aiResponse = await _deepSeekService.ProcessPdfDataAsync(enrichedContext, language);

                // إضافة رد النظام إلى قاعدة البيانات
                long systemMessageId = await _chatDbService.AddChatMessageAsync(
                    long.Parse(request.RoomId),
                    "0", // معرف النظام
                    "system",
                    aiResponse
                );

                // الحصول على الرسائل بعد الإضافة
                var userMessage = await GetChatMessageById(long.Parse(request.RoomId), userMessageId);
                var systemMessage = await GetChatMessageById(long.Parse(request.RoomId), systemMessageId);

                // تسجيل المحادثة للتحليل
                var topic = await _legalContextService.DetectLegalTopicAsync(request.Content, language);
                var keywords = await _legalContextService.ExtractLegalKeywordsAsync(request.Content, language);

                var metadata = new Dictionary<string, object>
                {
                    ["topic"] = topic,
                    ["pdfFiles"] = chatRoom.PdfFiles,
                    ["language"] = language
                };

                string conversationId = await _conversationTrackingService.LogConversationAsync(
                    userId,
                    request.RoomId,
                    request.Content,
                    aiResponse,
                    metadata
                );

                // تتبع الكلمات المفتاحية
                if (keywords.Count > 0)
                {
                    await _conversationTrackingService.TrackKeywordsAsync(conversationId, keywords);
                }

                // إنشاء نموذج الاستجابة
                var response = new ChatResponseModel
                {
                    RoomId = request.RoomId,
                    UserMessage = userMessage,
                    SystemResponse = systemMessage
                };

                return BaseResponse<ChatResponseModel>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء معالجة الرسالة");
                var errorMessage = _localizationService.GetMessage("MessageProcessingError", "Errors", language);
                return BaseResponse<ChatResponseModel>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على غرفة دردشة بمعرفها
        /// </summary>
        public async Task<BaseResponse<ChatRoomDTO>> GetChatRoomAsync(string roomId, string userId, string language)
        {
            try
            {
                _logger.LogInformation("طلب معلومات غرفة الدردشة: {RoomId} من المستخدم: {UserId}", roomId, userId);

                // التحقق من وجود غرفة الدردشة
                var chatRoom = await _chatDbService.GetChatRoomByIdAsync(long.Parse(roomId));
                if (chatRoom == null)
                {
                    var errorMessage = _localizationService.GetMessage("ChatRoomNotFound", "Errors", language);
                    return BaseResponse<ChatRoomDTO>.FailureResponse(errorMessage, 404);
                }

                // التحقق من أن المستخدم يملك الغرفة
                if (chatRoom.CreatedBy != userId)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedChatRoomAccess", "Errors", language);
                    return BaseResponse<ChatRoomDTO>.FailureResponse(errorMessage, 403);
                }

                return BaseResponse<ChatRoomDTO>.SuccessResponse(chatRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء استرجاع غرفة الدردشة");
                var errorMessage = _localizationService.GetMessage("ChatRoomRetrievalError", "Errors", language);
                return BaseResponse<ChatRoomDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على تاريخ المحادثات لغرفة دردشة
        /// </summary>
        public async Task<BaseResponse<List<ChatMessageDTO>>> GetChatHistoryAsync(string roomId, string userId, string language)
        {
            try
            {
                _logger.LogInformation("طلب تاريخ المحادثات للغرفة: {RoomId} من المستخدم: {UserId}", roomId, userId);

                // التحقق من وجود غرفة الدردشة
                var chatRoom = await _chatDbService.GetChatRoomByIdAsync(long.Parse(roomId));
                if (chatRoom == null)
                {
                    var errorMessage = _localizationService.GetMessage("ChatRoomNotFound", "Errors", language);
                    return BaseResponse<List<ChatMessageDTO>>.FailureResponse(errorMessage, 404);
                }

                // التحقق من أن المستخدم يملك الغرفة
                if (chatRoom.CreatedBy != userId)
                {
                    var errorMessage = _localizationService.GetMessage("UnauthorizedChatRoomAccess", "Errors", language);
                    return BaseResponse<List<ChatMessageDTO>>.FailureResponse(errorMessage, 403);
                }

                // الحصول على رسائل الغرفة
                var messages = await _chatDbService.GetChatRoomMessagesAsync(long.Parse(roomId), 100);

                return BaseResponse<List<ChatMessageDTO>>.SuccessResponse(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء استرجاع تاريخ المحادثات");
                var errorMessage = _localizationService.GetMessage("ChatHistoryRetrievalError", "Errors", language);
                return BaseResponse<List<ChatMessageDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على جميع غرف الدردشة الخاصة بالمستخدم
        /// </summary>
        public async Task<BaseResponse<List<ChatRoomDTO>>> GetUserChatRoomsAsync(string userId, string language)
        {
            try
            {
                _logger.LogInformation("طلب قائمة غرف الدردشة للمستخدم: {UserId}", userId);

                // الحصول على غرف الدردشة من قاعدة البيانات
                var chatRooms = await _chatDbService.GetChatRoomsByUserIdAsync(long.Parse(userId));

                return BaseResponse<List<ChatRoomDTO>>.SuccessResponse(chatRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء استرجاع قائمة غرف الدردشة للمستخدم");
                var errorMessage = _localizationService.GetMessage("UserChatRoomsRetrievalError", "Errors", language);
                return BaseResponse<List<ChatRoomDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// حذف غرفة دردشة
        /// </summary>
        public async Task<BaseResponse<bool>> DeleteChatRoomAsync(string roomId, string userId, string language)
        {
            try
            {
                _logger.LogInformation("طلب حذف غرفة الدردشة: {RoomId} من المستخدم: {UserId}", roomId, userId);

                // حذف الغرفة من قاعدة البيانات
                bool success = await _chatDbService.DeleteChatRoomAsync(
                    long.Parse(roomId),
                    long.Parse(userId)
                );

                if (success)
                {
                    var successMessage = _localizationService.GetMessage("ChatRoomDeleted", "Messages", language);
                    return BaseResponse<bool>.SuccessResponse(true, successMessage);
                }
                else
                {
                    var errorMessage = _localizationService.GetMessage("ChatRoomDeletionError", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 400);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف غرفة الدردشة");
                var errorMessage = _localizationService.GetMessage("ChatRoomDeletionError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        #region Helper Methods

        /// <summary>
        /// استخراج محتوى ملفات PDF
        /// </summary>
        private async Task<Dictionary<string, string>> GetPdfContentsAsync(List<string> pdfFiles)
        {
            _logger.LogInformation("استخراج محتوى {Count} ملفات PDF", pdfFiles.Count);
            return await _pdfExtractionService.ExtractTextFromMultiplePdfsAsync(pdfFiles);
        }

        /// <summary>
        /// الحصول على رسالة محددة من قاعدة البيانات
        /// </summary>
        private async Task<ChatMessageDTO> GetChatMessageById(long roomId, long messageId)
        {
            // الحصول على جميع الرسائل
            var messages = await _chatDbService.GetChatRoomMessagesAsync(roomId);

            // البحث عن الرسالة المطلوبة
            return messages.FirstOrDefault(m => m.Id == messageId);
        }

        #endregion
    }
}