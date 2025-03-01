using API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Services
{
    public interface IChatService
    {
        /// <summary>
        /// إنشاء غرفة دردشة جديدة
        /// </summary>
        /// <param name="request">بيانات الغرفة</param>
        /// <param name="userId">معرف المستخدم المنشئ</param>
        /// <param name="language">اللغة</param>
        /// <returns>غرفة الدردشة المنشأة</returns>
        Task<BaseResponse<ChatRoom>> CreateChatRoomAsync(CreateChatRoomRequest request, string userId, string language);

        /// <summary>
        /// الحصول على غرفة دردشة بواسطة المعرف
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <param name="language">اللغة</param>
        /// <returns>غرفة الدردشة</returns>
        Task<BaseResponse<ChatRoom>> GetChatRoomAsync(string roomId, string language);

        /// <summary>
        /// الحصول على قائمة غرف الدردشة للمستخدم
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة</param>
        /// <returns>قائمة غرف الدردشة</returns>
        Task<BaseResponse<List<ChatRoom>>> GetUserChatRoomsAsync(string userId, string language);

        /// <summary>
        /// إرسال رسالة في غرفة دردشة
        /// </summary>
        /// <param name="request">بيانات الرسالة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة</param>
        /// <returns>الرد على الرسالة</returns>
        Task<BaseResponse<ChatResponseModel>> SendMessageAsync(SendMessageRequest request, string userId, string language);

        /// <summary>
        /// حذف غرفة دردشة
        /// </summary>
        /// <param name="roomId">معرف الغرفة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة</param>
        /// <returns>نتيجة الحذف</returns>
        Task<BaseResponse<bool>> DeleteChatRoomAsync(string roomId, string userId, string language);
    }

    public class ChatService : IChatService
    {
        private readonly IDeepSeekService _deepSeekService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly string _pdfBasePath;
        private readonly string _dataPath;
        private readonly object _fileLock = new object();
        // قواعد افتراضية للدردشة مع Deep Seek
        private readonly string _defaultRules = @"
1. الالتزام بالإجابة على الأسئلة المتعلقة بالملفات PDF المحددة فقط.
2. عدم إعطاء معلومات خارج نطاق الملفات المحددة.
3. رفض الإجابة على أي سؤال غير متعلق بمحتوى الملفات.
4. الالتزام بقواعد الاحترام والأدب في الردود.
5. توضيح مصدر المعلومة من الملف عند الإجابة.";

        // قاموس لتخزين جلسات الدردشة في الذاكرة (في بيئة حقيقية سيتم استخدام قاعدة بيانات)
        private readonly ConcurrentDictionary<string, ChatRoom> _chatRooms = new();

        public ChatService(
            IDeepSeekService deepSeekService,
            IConfiguration configuration,
            ILogger<ChatService> logger,
            ILocalizationService localizationService)
        {
            _deepSeekService = deepSeekService;
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;

            // تحديد مسار ملفات PDF
            _pdfBasePath = _configuration["ChatSettings:PdfBasePath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfFiles");

            // تحديد مسار حفظ بيانات غرف الدردشة
            _dataPath = _configuration["ChatSettings:DataPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChatData");

            // التأكد من وجود المسارات
            Directory.CreateDirectory(_pdfBasePath);
            Directory.CreateDirectory(_dataPath);

            // تحميل غرف الدردشة المخزنة سابقًا
            LoadChatRooms();
        }

        /// <summary>
        /// إنشاء غرفة دردشة جديدة
        /// </summary>
        public async Task<BaseResponse<ChatRoom>> CreateChatRoomAsync(CreateChatRoomRequest request, string userId, string language)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                var errorMessage = _localizationService.GetMessage("ChatRoomTitleRequired", "Errors", language);
                return BaseResponse<ChatRoom>.FailureResponse(errorMessage, 400);
            }

            // التحقق من وجود ملفات PDF المحددة
            foreach (var pdfFile in request.PdfFiles)
            {
                var pdfPath = Path.Combine(_pdfBasePath, pdfFile);
                if (!File.Exists(pdfPath))
                {
                    var errorMessage = _localizationService.GetMessage("PdfFileNotFound", "Errors", language);
                    return BaseResponse<ChatRoom>.FailureResponse(string.Format(errorMessage, pdfFile), 400);
                }
            }

            // إنشاء غرفة دردشة جديدة
            var chatRoom = new ChatRoom
            {
                Title = request.Title,
                Description = request.Description,
                CreatedBy = userId,
                PdfFiles = request.PdfFiles,
                Rules = request.Rules ?? _defaultRules
            };

            // إضافة رسالة ترحيب
            var welcomeMessage = _localizationService.GetMessage("ChatRoomWelcomeMessage", "Messages", language);
            chatRoom.Messages.Add(new ChatMessage
            {
                Role = "system",
                Content = string.Format(welcomeMessage, request.Title)
            });

            // تخزين الغرفة
            _chatRooms[chatRoom.Id] = chatRoom;
            SaveChatRoom(chatRoom);

            var successMessage = _localizationService.GetMessage("ChatRoomCreated", "Messages", language);
            return BaseResponse<ChatRoom>.SuccessResponse(chatRoom, successMessage);
        }

        /// <summary>
        /// الحصول على غرفة دردشة بواسطة المعرف
        /// </summary>
        public async Task<BaseResponse<ChatRoom>> GetChatRoomAsync(string roomId, string language)
        {
            if (!_chatRooms.TryGetValue(roomId, out var chatRoom))
            {
                var errorMessage = _localizationService.GetMessage("ChatRoomNotFound", "Errors", language);
                return BaseResponse<ChatRoom>.FailureResponse(errorMessage, 404);
            }

            return BaseResponse<ChatRoom>.SuccessResponse(chatRoom);
        }

        /// <summary>
        /// الحصول على قائمة غرف الدردشة للمستخدم
        /// </summary>
        public async Task<BaseResponse<List<ChatRoom>>> GetUserChatRoomsAsync(string userId, string language)
        {
            var userRooms = _chatRooms.Values
                .Where(r => r.CreatedBy == userId)
                .ToList();

            return BaseResponse<List<ChatRoom>>.SuccessResponse(userRooms);
        }

        /// <summary>
        /// إرسال رسالة في غرفة دردشة
        /// </summary>
        public async Task<BaseResponse<ChatResponseModel>> SendMessageAsync(SendMessageRequest request, string userId, string language)
        {
            if (!_chatRooms.TryGetValue(request.RoomId, out var chatRoom))
            {
                var errorMessage = _localizationService.GetMessage("ChatRoomNotFound", "Errors", language);
                return BaseResponse<ChatResponseModel>.FailureResponse(errorMessage, 404);
            }

            if (string.IsNullOrEmpty(request.Content))
            {
                var errorMessage = _localizationService.GetMessage("MessageContentRequired", "Errors", language);
                return BaseResponse<ChatResponseModel>.FailureResponse(errorMessage, 400);
            }

            // إنشاء رسالة المستخدم
            var userMessage = new ChatMessage
            {
                Role = "user",
                Content = request.Content
            };

            // إضافة الرسالة إلى الغرفة
            chatRoom.Messages.Add(userMessage);

            try
            {
                // إعداد المحتوى للإرسال إلى Deep Seek
                var deepSeekPrompt = PrepareDeepSeekPrompt(chatRoom, request.Content);

                // إرسال الطلب إلى Deep Seek
                var deepSeekResponse = await _deepSeekService.ProcessPdfDataAsync(deepSeekPrompt, language);

                // إنشاء رسالة النظام
                var systemMessage = new ChatMessage
                {
                    Role = "system",
                    Content = deepSeekResponse
                };

                // إضافة الرسالة إلى الغرفة
                chatRoom.Messages.Add(systemMessage);

                // حفظ التغييرات
                SaveChatRoom(chatRoom);

                // إنشاء نموذج الاستجابة
                var response = new ChatResponseModel
                {
                    RoomId = chatRoom.Id,
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
        /// حذف غرفة دردشة
        /// </summary>
        public async Task<BaseResponse<bool>> DeleteChatRoomAsync(string roomId, string userId, string language)
        {
            if (!_chatRooms.TryGetValue(roomId, out var chatRoom))
            {
                var errorMessage = _localizationService.GetMessage("ChatRoomNotFound", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 404);
            }

            if (chatRoom.CreatedBy != userId)
            {
                var errorMessage = _localizationService.GetMessage("UnauthorizedOperation", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 403);
            }

            // حذف الغرفة
            if (_chatRooms.TryRemove(roomId, out _))
            {
                // حذف ملف الغرفة
                var chatRoomPath = Path.Combine(_dataPath, $"{roomId}.json");
                if (File.Exists(chatRoomPath))
                {
                    File.Delete(chatRoomPath);
                }

                var successMessage = _localizationService.GetMessage("ChatRoomDeleted", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }

            var failureMessage = _localizationService.GetMessage("ChatRoomDeleteFailed", "Errors", language);
            return BaseResponse<bool>.FailureResponse(failureMessage, 500);
        }

        /// <summary>
        /// تحميل غرف الدردشة من الملفات
        /// </summary>
        private void LoadChatRooms()
        {
            try
            {
                var files = Directory.GetFiles(_dataPath, "*.json");
                foreach (var file in files)
                {
                    var json = File.ReadAllText(file);
                    var chatRoom = JsonSerializer.Deserialize<ChatRoom>(json);
                    if (chatRoom != null)
                    {
                        _chatRooms[chatRoom.Id] = chatRoom;
                    }
                }
                _logger.LogInformation("تم تحميل {count} غرفة دردشة", _chatRooms.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل غرف الدردشة");
            }
        }

        /// <summary>
        /// حفظ غرفة دردشة في ملف
        /// </summary>
        private void SaveChatRoom(ChatRoom chatRoom)
        {
            try
            {
                lock (_fileLock)
                {
                    var chatRoomPath = Path.Combine(_dataPath, $"{chatRoom.Id}.json");
                    var json = JsonSerializer.Serialize(chatRoom, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(chatRoomPath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حفظ غرفة الدردشة {roomId}", chatRoom.Id);
            }
        }

        /// <summary>
        /// إعداد المحتوى لإرسال إلى Deep Seek
        /// </summary>
        private string PrepareDeepSeekPrompt(ChatRoom chatRoom, string userQuery)
        {
            var pdfContents = new StringBuilder();

            // قراءة محتويات ملفات PDF
            foreach (var pdfFile in chatRoom.PdfFiles)
            {
                var pdfPath = Path.Combine(_pdfBasePath, pdfFile);
                if (File.Exists(pdfPath))
                {
                    // في بيئة حقيقية، ستستخدم مكتبة لاستخراج النص من ملفات PDF
                    // هنا نفترض أن لدينا نسخة نصية من الملف بنفس الاسم ولكن بلاحقة .txt
                    var txtFilePath = Path.ChangeExtension(pdfPath, ".txt");
                    if (File.Exists(txtFilePath))
                    {
                        var content = File.ReadAllText(txtFilePath);
                        pdfContents.AppendLine($"=== محتوى ملف: {pdfFile} ===");
                        pdfContents.AppendLine(content);
                        pdfContents.AppendLine();
                    }
                }
            }

            // بناء سلسلة من المحادثات السابقة (آخر 10 رسائل فقط للحفاظ على الحجم)
            var conversationHistory = new StringBuilder();
            var recentMessages = chatRoom.Messages.Skip(Math.Max(0, chatRoom.Messages.Count - 10)).ToList();
            foreach (var message in recentMessages)
            {
                conversationHistory.AppendLine($"{message.Role}: {message.Content}");
            }

            // بناء الاستعلام النهائي
            var prompt = new StringBuilder();
            prompt.AppendLine("أنت مساعد ذكي يساعد في تحليل وشرح المعلومات من ملفات PDF المحددة.");
            prompt.AppendLine();
            prompt.AppendLine("القواعد التي يجب عليك الالتزام بها:");
            prompt.AppendLine(chatRoom.Rules);
            prompt.AppendLine();
            prompt.AppendLine("محتوى الملفات التي يمكنك الرجوع إليها:");
            prompt.AppendLine(pdfContents.ToString());
            prompt.AppendLine();
            prompt.AppendLine("المحادثة السابقة:");
            prompt.AppendLine(conversationHistory.ToString());
            prompt.AppendLine();
            prompt.AppendLine($"سؤال المستخدم: {userQuery}");
            prompt.AppendLine();
            prompt.AppendLine("أجب على سؤال المستخدم بناءً على المعلومات الموجودة في الملفات المذكورة أعلاه فقط. إذا كان السؤال خارج نطاق هذه الملفات، قم بالاعتذار بلطف واشرح أنك مقيد بالإجابة على الأسئلة المتعلقة بهذه الملفات فقط.");

            return prompt.ToString();
        }
    }
}