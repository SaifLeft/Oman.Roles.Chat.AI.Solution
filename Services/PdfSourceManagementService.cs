using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.Files;

namespace Services
{
    /// <summary>
    /// واجهة خدمة إدارة مصادر ملفات PDF
    /// </summary>
    public interface IPdfSourceManagementService
    {
        /// <summary>
        /// رفع ملف PDF جديد
        /// </summary>
        /// <param name="fileName">اسم الملف</param>
        /// <param name="title">العنوان الوصفي للملف</param>
        /// <param name="description">وصف الملف</param>
        /// <param name="fileContent">محتوى الملف</param>
        /// <param name="contentType">نوع المحتوى</param>
        /// <param name="keywords">الكلمات المفتاحية</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>معلومات الملف المضاف</returns>
        Task<BaseResponse<PdfDocumentDTO>> UploadPdfFileAsync(string fileName, string title, string description, byte[] fileContent, string contentType, List<string> keywords, long userId, string language);

        /// <summary>
        /// الحصول على قائمة ملفات PDF المتاحة
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>قائمة بملفات PDF</returns>
        Task<BaseResponse<List<PdfDocumentDTO>>> GetAvailablePdfFilesAsync(long userId, string language);

        /// <summary>
        /// الحصول على معلومات ملف PDF
        /// </summary>
        /// <param name="fileId">معرف الملف</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>معلومات الملف</returns>
        Task<BaseResponse<PdfDocumentDTO>> GetPdfFileInfoAsync(long fileId, string language);

        /// <summary>
        /// تحديث معلومات ملف PDF
        /// </summary>
        /// <param name="fileId">معرف الملف</param>
        /// <param name="title">العنوان الجديد</param>
        /// <param name="description">الوصف الجديد</param>
        /// <param name="keywords">الكلمات المفتاحية الجديدة</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>معلومات الملف المحدثة</returns>
        Task<BaseResponse<PdfDocumentDTO>> UpdatePdfFileInfoAsync(long fileId, string title, string description, List<string> keywords, long userId, string language);

        /// <summary>
        /// حذف ملف PDF
        /// </summary>
        /// <param name="fileId">معرف الملف</param>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>نتيجة العملية</returns>
        Task<BaseResponse<bool>> DeletePdfFileAsync(long fileId, long userId, string language);

        /// <summary>
        /// البحث عن ملفات PDF بالكلمات المفتاحية
        /// </summary>
        /// <param name="keywords">الكلمات المفتاحية</param>
        /// <param name="language">اللغة المستخدمة</param>
        /// <returns>قائمة بملفات PDF</returns>
        Task<BaseResponse<List<PdfDocumentDTO>>> SearchPdfFilesByKeywordsAsync(List<string> keywords, string language);
        Task<BaseResponse<List<PdfDocumentDTO>>> UploadKnowledgeBasePdfAsync(string fileName, string title, string description, byte[] fileContent, string contentType, List<string> keywords, long v, string language);
    }

    /// <summary>
    /// تنفيذ خدمة إدارة مصادر ملفات PDF
    /// </summary>
    public class PdfSourceManagementService : IPdfSourceManagementService
    {
        private readonly MuhamiContext _context;
        private readonly IPdfExtractionService _pdfExtractionService;
        private readonly ILegalContextService _legalContextService;
        private readonly ILogger<PdfSourceManagementService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;
        private readonly string _pdfBasePath;

        /// <summary>
        /// إنشاء مثيل جديد من خدمة إدارة مصادر ملفات PDF
        /// </summary>
        public PdfSourceManagementService(
            MuhamiContext context,
            IPdfExtractionService pdfExtractionService,
            ILegalContextService legalContextService,
            ILogger<PdfSourceManagementService> logger,
            ILocalizationService localizationService,
            IConfiguration configuration)
        {
            _context = context;
            _pdfExtractionService = pdfExtractionService;
            _legalContextService = legalContextService;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;

            // تحديد مسار ملفات PDF
            _pdfBasePath = _configuration["ChatSettings:PdfBasePath"] ??
                           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfFiles");

            // التأكد من وجود المسار
            Directory.CreateDirectory(_pdfBasePath);
        }

        /// <summary>
        /// رفع ملف PDF جديد
        /// </summary>
        public async Task<BaseResponse<PdfDocumentDTO>> UploadPdfFileAsync(
            string fileName,
            string title,
            string description,
            byte[] fileContent,
            string contentType,
            List<string> keywords,
            long userId,
            string language)
        {
            try
            {
                _logger.LogInformation("رفع ملف PDF جديد: {FileName} بواسطة المستخدم: {UserId}", fileName, userId);

                // التحقق من نوع الملف
                if (!contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidFileType", "Errors", language);
                    return BaseResponse<PdfDocumentDTO>.FailureResponse(errorMessage, 400);
                }

                // التحقق من حجم الملف (الحد الأقصى 20 ميجابايت)
                const int maxFileSizeBytes = 20 * 1024 * 1024;
                if (fileContent.Length > maxFileSizeBytes)
                {
                    var errorMessage = _localizationService.GetMessage("FileTooLarge", "Errors", language);
                    return BaseResponse<PdfDocumentDTO>.FailureResponse(errorMessage, 400);
                }

                // إنشاء اسم ملف فريد إذا لم يتم تقديمه
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = $"{Guid.NewGuid()}.pdf";
                }
                else if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.pdf";
                }

                // التحقق من عدم وجود ملف بنفس الاسم
                if (await _context.DataSourceFiles.AnyAsync(f => f.FileName == fileName && f.IsDeleted != true))
                {
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                }

                // حفظ الملف على القرص
                string filePath = Path.Combine(_pdfBasePath, fileName);
                await File.WriteAllBytesAsync(filePath, fileContent);

                // إنشاء كائن DataSourceFile
                var dataSourceFile = new DataSourceFile
                {
                    FileName = fileName,
                    Title = !string.IsNullOrEmpty(title) ? title : Path.GetFileNameWithoutExtension(fileName),
                    Description = description ?? string.Empty,
                    FilePath = filePath,
                    Size = fileContent.Length,
                    ContentType = contentType,
                    CreatedByUserId = userId,
                    CreateDate = DateTime.Now
                };

                // إضافة الكائن إلى قاعدة البيانات
                _context.DataSourceFiles.Add(dataSourceFile);
                await _context.SaveChangesAsync();

                // إضافة الكلمات المفتاحية
                if (keywords != null && keywords.Count > 0)
                {
                    foreach (var keyword in keywords)
                    {
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            var keywordEntity = new DataSourceFileKeyword
                            {
                                FileId = dataSourceFile.Id,
                                Keyword = keyword.Trim()
                            };
                            _context.DataSourceFileKeywords.Add(keywordEntity);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // استخراج كلمات مفتاحية إضافية من محتوى الملف
                try
                {
                    // استخراج النص من الملف
                    var extractedText = await _pdfExtractionService.ExtractTextFromPdfAsync(fileName);

                    // استخراج الكلمات المفتاحية
                    if (!string.IsNullOrEmpty(extractedText))
                    {
                        var extractedKeywords = await _legalContextService.ExtractLegalKeywordsAsync(extractedText, language);
                        if (extractedKeywords.Count > 0)
                        {
                            var existingKeywords = await _context.DataSourceFileKeywords
                                .Where(k => k.FileId == dataSourceFile.Id)
                                .Select(k => k.Keyword.ToLower())
                                .ToListAsync();

                            foreach (var keyword in extractedKeywords)
                            {
                                if (!string.IsNullOrWhiteSpace(keyword) &&
                                    !existingKeywords.Contains(keyword.ToLower()))
                                {
                                    var keywordEntity = new DataSourceFileKeyword
                                    {
                                        FileId = dataSourceFile.Id,
                                        Keyword = keyword.Trim()
                                    };
                                    _context.DataSourceFileKeywords.Add(keywordEntity);
                                }
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "فشل في استخراج الكلمات المفتاحية من ملف PDF: {FileName}", fileName);
                    // لا نريد فشل العملية بأكملها إذا فشل استخراج الكلمات المفتاحية
                }

                // تحويل الكائن إلى نموذج PdfFile للاستجابة
                var pdfFile = await ConvertToModelAsync(dataSourceFile);

                var successMessage = _localizationService.GetMessage("PdfFileUploaded", "Messages", language);
                return BaseResponse<PdfDocumentDTO>.SuccessResponse(pdfFile, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في رفع ملف PDF: {FileName}", fileName);
                var errorMessage = _localizationService.GetMessage("PdfFileUploadError", "Errors", language);
                return BaseResponse<PdfDocumentDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على قائمة ملفات PDF المتاحة
        /// </summary>
        public async Task<BaseResponse<List<PdfDocumentDTO>>> GetAvailablePdfFilesAsync(long userId, string language)
        {
            try
            {
                _logger.LogInformation("الحصول على قائمة ملفات PDF المتاحة للمستخدم: {UserId}", userId);

                var dataSourceFiles = await _context.DataSourceFiles
                    .Where(f => f.IsDeleted != true)
                    .OrderByDescending(f => f.CreateDate)
                    .ToListAsync();

                var pdfFiles = new List<PdfDocumentDTO>();
                foreach (var dataSourceFile in dataSourceFiles)
                {
                    var pdfFile = await ConvertToModelAsync(dataSourceFile);
                    pdfFiles.Add(pdfFile);
                }

                return BaseResponse<List<PdfDocumentDTO>>.SuccessResponse(pdfFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على قائمة ملفات PDF المتاحة");
                var errorMessage = _localizationService.GetMessage("PdfFilesRetrievalError", "Errors", language);
                return BaseResponse<List<PdfDocumentDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على معلومات ملف PDF
        /// </summary>
        public async Task<BaseResponse<PdfDocumentDTO>> GetPdfFileInfoAsync(long fileId, string language)
        {
            try
            {
                _logger.LogInformation("الحصول على معلومات ملف PDF: {FileId}", fileId);

                var dataSourceFile = await _context.DataSourceFiles
                    .FirstOrDefaultAsync(f => f.Id == fileId && f.IsDeleted != true);

                if (dataSourceFile == null)
                {
                    var errorMessage = _localizationService.GetMessage("PdfFileNotFound", "Errors", language);
                    return BaseResponse<PdfDocumentDTO>.FailureResponse(errorMessage, 404);
                }

                var pdfFile = await ConvertToModelAsync(dataSourceFile);

                return BaseResponse<PdfDocumentDTO>.SuccessResponse(pdfFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على معلومات ملف PDF: {FileId}", fileId);
                var errorMessage = _localizationService.GetMessage("PdfFileInfoRetrievalError", "Errors", language);
                return BaseResponse<PdfDocumentDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث معلومات ملف PDF
        /// </summary>
        public async Task<BaseResponse<PdfDocumentDTO>> UpdatePdfFileInfoAsync(
            long fileId,
            string title,
            string description,
            List<string> keywords,
            long userId,
            string language)
        {
            try
            {
                _logger.LogInformation("تحديث معلومات ملف PDF: {FileId} بواسطة المستخدم: {UserId}", fileId, userId);

                var dataSourceFile = await _context.DataSourceFiles
                    .FirstOrDefaultAsync(f => f.Id == fileId && f.IsDeleted != true);

                if (dataSourceFile == null)
                {
                    var errorMessage = _localizationService.GetMessage("PdfFileNotFound", "Errors", language);
                    return BaseResponse<PdfDocumentDTO>.FailureResponse(errorMessage, 404);
                }

                // تحديث المعلومات
                if (!string.IsNullOrWhiteSpace(title))
                {
                    dataSourceFile.Title = title;
                }

                if (description != null) // السماح بوصف فارغ
                {
                    dataSourceFile.Description = description;
                }

                dataSourceFile.ModifiedByUserId = userId;
                dataSourceFile.ModifiedDate = DateTime.Now;

                // تحديث الكلمات المفتاحية
                if (keywords != null)
                {
                    // حذف الكلمات المفتاحية الحالية
                    var existingKeywords = await _context.DataSourceFileKeywords
                        .Where(k => k.FileId == fileId)
                        .ToListAsync();

                    foreach (var keyword in existingKeywords)
                    {
                        _context.DataSourceFileKeywords.Remove(keyword);
                    }

                    // إضافة الكلمات المفتاحية الجديدة
                    foreach (var keyword in keywords)
                    {
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            var keywordEntity = new DataSourceFileKeyword
                            {
                                FileId = fileId,
                                Keyword = keyword.Trim()
                            };
                            _context.DataSourceFileKeywords.Add(keywordEntity);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                var pdfFile = await ConvertToModelAsync(dataSourceFile);

                var successMessage = _localizationService.GetMessage("PdfFileInfoUpdated", "Messages", language);
                return BaseResponse<PdfDocumentDTO>.SuccessResponse(pdfFile, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث معلومات ملف PDF: {FileId}", fileId);
                var errorMessage = _localizationService.GetMessage("PdfFileInfoUpdateError", "Errors", language);
                return BaseResponse<PdfDocumentDTO>.FailureResponse(errorMessage, 500);
            }
        }


        /// <summary>
        /// حذف ملف PDF
        /// </summary>
        public async Task<BaseResponse<bool>> DeletePdfFileAsync(long fileId, long userId, string language)
        {
            try
            {
                _logger.LogInformation("حذف ملف PDF: {FileId} بواسطة المستخدم: {UserId}", fileId, userId);

                var dataSourceFile = await _context.DataSourceFiles
                    .FirstOrDefaultAsync(f => f.Id == fileId && f.IsDeleted != true);

                if (dataSourceFile == null)
                {
                    var errorMessage = _localizationService.GetMessage("PdfFileNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(errorMessage, 404);
                }

                // تنفيذ حذف منطقي (Soft Delete) بدلاً من الحذف الفعلي
                dataSourceFile.IsDeleted = true;
                dataSourceFile.ModifiedByUserId = userId;
                dataSourceFile.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                // اختياري: حذف الملف الفعلي من المجلد
                // هذا يعتمد على سياسة التطبيق - قد ترغب في الاحتفاظ بالملفات حتى بعد الحذف المنطقي
                // إذا كنت ترغب في حذف الملف فعلياً من القرص، قم بإزالة التعليق عن الكود التالي
                /*
                try
                {
                    if (File.Exists(dataSourceFile.FilePath))
                    {
                        File.Delete(dataSourceFile.FilePath);
                    }
                }
                catch (Exception ex)
                {
                    // تسجيل الخطأ ولكن لا نفشل العملية بالكامل إذا فشل حذف الملف من القرص
                    _logger.LogWarning(ex, "فشل في حذف ملف PDF من القرص: {FilePath}", dataSourceFile.FilePath);
                }
                */

                var successMessage = _localizationService.GetMessage("PdfFileDeleted", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف ملف PDF: {FileId}", fileId);
                var errorMessage = _localizationService.GetMessage("PdfFileDeletionError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// البحث عن ملفات PDF بالكلمات المفتاحية
        /// </summary>
        public async Task<BaseResponse<List<PdfDocumentDTO>>> SearchPdfFilesByKeywordsAsync(List<string> keywords, string language)
        {
            try
            {
                _logger.LogInformation("البحث عن ملفات PDF بالكلمات المفتاحية: {Keywords}", string.Join(", ", keywords));

                if (keywords == null || keywords.Count == 0)
                {
                    var errorMessage = _localizationService.GetMessage("NoKeywordsProvided", "Errors", language);
                    return BaseResponse<List<PdfDocumentDTO>>.FailureResponse(errorMessage, 400);
                }

                // تنظيف وتنسيق الكلمات المفتاحية
                var normalizedKeywords = keywords
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Select(k => k.Trim().ToLower())
                    .ToList();

                if (normalizedKeywords.Count == 0)
                {
                    var errorMessage = _localizationService.GetMessage("NoKeywordsProvided", "Errors", language);
                    return BaseResponse<List<PdfDocumentDTO>>.FailureResponse(errorMessage, 400);
                }

                // الحصول على معرفات الملفات التي تحتوي على أي من الكلمات المفتاحية
                var fileIds = await _context.DataSourceFileKeywords
                    .Where(k => normalizedKeywords.Contains(k.Keyword.ToLower()))
                    .Select(k => k.FileId)
                    .Distinct()
                    .ToListAsync();

                if (fileIds.Count == 0)
                {
                    // لم يتم العثور على ملفات تطابق الكلمات المفتاحية
                    var errorMessage = _localizationService.GetMessage("NoFilesMatchKeywords", "Errors", language);
                    return BaseResponse<List<PdfDocumentDTO>>.FailureResponse(errorMessage, 404);
                }

                // الحصول على بيانات الملفات
                var dataSourceFiles = await _context.DataSourceFiles
                    .Where(f => fileIds.Contains(f.Id) && f.IsDeleted != true)
                    .OrderByDescending(f => f.CreateDate)
                    .ToListAsync();

                // تحويل الكائنات إلى النماذج المطلوبة للاستجابة
                var pdfFiles = new List<PdfDocumentDTO>();
                foreach (var dataSourceFile in dataSourceFiles)
                {
                    var pdfFile = await ConvertToModelAsync(dataSourceFile);
                    pdfFiles.Add(pdfFile);
                }

                var successMessage = _localizationService.GetMessage("FilesFoundWithKeywords", "Messages", language);
                return BaseResponse<List<PdfDocumentDTO>>.SuccessResponse(pdfFiles, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في البحث عن ملفات PDF بالكلمات المفتاحية");
                var errorMessage = _localizationService.GetMessage("PdfFileSearchError", "Errors", language);
                return BaseResponse<List<PdfDocumentDTO>>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحويل كائن DataSourceFile إلى نموذج PdfFile
        /// </summary>
        private async Task<PdfDocumentDTO> ConvertToModelAsync(DataSourceFile dataSourceFile)
        {
            // الحصول على الكلمات المفتاحية المرتبطة بالملف
            var keywords = await _context.DataSourceFileKeywords
                .Where(k => k.FileId == dataSourceFile.Id)
                .Select(k => k.Keyword)
                .ToListAsync();

            // إنشاء نموذج PdfFile
            var pdfFile = new PdfDocumentDTO
            {
                Id = dataSourceFile.Id.ToString(),
                FileName = dataSourceFile.FileName,
                Title = dataSourceFile.Title,
                Description = dataSourceFile.Description,
                FilePath = dataSourceFile.FilePath,
                Size = dataSourceFile.Size,
                SizeFormatted = FormatFileSize(dataSourceFile.Size),
                ContentType = dataSourceFile.ContentType,
                Keywords = keywords,
                CreatedByUserId = dataSourceFile.CreatedByUserId.ToString(),
                CreatedAt = dataSourceFile.CreateDate,
                ModifiedByUserId = dataSourceFile.ModifiedByUserId?.ToString(),
                ModifiedAt = dataSourceFile.ModifiedDate,
                DownloadUrl = $"/api/files/download/{dataSourceFile.Id}" // URL تنزيل الملف (قد يحتاج للتعديل حسب تكوين API)
            };

            return pdfFile;
        }

        /// <summary>
        /// تنسيق حجم الملف لعرضه بصيغة مناسبة للمستخدم
        /// </summary>
        private string FormatFileSize(long bytes)
        {
            string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            const double thresh = 1024;

            if (bytes < 0)
            {
                return "0 B";
            }

            int i = 0;
            double size = bytes;

            while (size >= thresh && i < sizeSuffixes.Length - 1)
            {
                size /= thresh;
                i++;
            }

            return string.Format("{0:0.##} {1}", size, sizeSuffixes[i]);
        }

        public async Task<BaseResponse<List<PdfDocumentDTO>>> UploadKnowledgeBasePdfAsync(string fileName, string title, string description, byte[] fileContent, string contentType, List<string> keywords, long v, string language)
        {
            try
            {
                _logger.LogInformation("رفع ملف PDF جديد: {FileName} بواسطة المستخدم: {UserId}", fileName, v);
                // التحقق من نوع الملف
                if (!contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var errorMessage = _localizationService.GetMessage("InvalidFileType", "Errors", language);
                    return BaseResponse<List<PdfDocumentDTO>>.FailureResponse(errorMessage, 400);
                }
                // التحقق من حجم الملف (الحد الأقصى 20 ميجابايت)
                const int maxFileSizeBytes = 20 * 1024 * 1024;
                if (fileContent.Length > maxFileSizeBytes)
                {
                    var errorMessage = _localizationService.GetMessage("FileTooLarge", "Errors", language);
                    return BaseResponse<List<PdfDocumentDTO>>.FailureResponse(errorMessage, 400);
                }
                // إنشاء اسم ملف فريد إذا لم يتم تقديمه
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = $"{Guid.NewGuid()}.pdf";
                }
                else if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.pdf";
                }
                // التحقق من عدم وجود ملف بنفس الاسم
                if (await _context.DataSourceFiles.AnyAsync(f => f.FileName == fileName && f.IsDeleted != true))
                {
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                }
                // حفظ الملف على القرص
                string filePath = Path.Combine(_pdfBasePath, fileName);
                await File.WriteAllBytesAsync(filePath, fileContent);
                // إنشاء كائن DataSourceFile
                var dataSourceFile = new DataSourceFile
                {
                    FileName = fileName,
                    Title = !string.IsNullOrEmpty(title) ? title : Path.GetFileNameWithoutExtension(fileName),
                    Description = description ?? string.Empty,
                    FilePath = filePath,
                    Size = fileContent.Length,
                    ContentType = contentType,
                    CreatedByUserId = v,
                    CreateDate = DateTime.Now,
                    IsKnowledgeBase = true,
                    Content = fileContent,
                    FileType = contentType,
                    IsPublic = true,
                    IsActive = true,
                    PageCount = 0, // يجب تحديثه لاحقًا بعد استخراج
                    UploadedBy = v
                };
                // إضافة الكائن إلى قاعدة البيانات
                _context.DataSourceFiles.Add(dataSourceFile);
                await _context.SaveChangesAsync();
                // إضافة الكلمات المفتاحية
                if (keywords != null && keywords.Count > 0)
                {
                    foreach (var keyword in keywords)
                    {
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            var keywordEntity = new DataSourceFileKeyword
                            {
                                FileId = dataSourceFile.Id,
                                Keyword = keyword.Trim()
                            };
                            _context.DataSourceFileKeywords.Add(keywordEntity);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                // استخراج كلمات مفتاحية إضافية من محتوى الملف
                try
                {
                    // استخراج النص من الملف
                    var extractedText = await _pdfExtractionService.ExtractTextFromPdfAsync(fileName);
                    // استخراج الكلمات المفتاحية
                    if (!string.IsNullOrEmpty(extractedText))
                    {
                        var extractedKeywords = await _legalContextService.ExtractLegalKeywordsAsync(extractedText, language);
                        if (extractedKeywords.Count > 0)
                        {
                            var existingKeywords = await _context.DataSourceFileKeywords
                                .Where(k => k.FileId == dataSourceFile.Id)
                                .Select(k => k.Keyword.ToLower())
                                .ToListAsync();
                            foreach (var keyword in extractedKeywords)
                            {
                                if (!string.IsNullOrWhiteSpace(keyword) &&
                                    !existingKeywords.Contains(keyword.ToLower()))
                                {
                                    var keywordEntity = new DataSourceFileKeyword
                                    {
                                        FileId = dataSourceFile.Id,
                                        Keyword = keyword.Trim()
                                    };
                                    _context.DataSourceFileKeywords.Add(keywordEntity);
                                }
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "فشل في استخراج الكلمات المفتاحية من ملف PDF: {FileName}", fileName);
                    // لا نريد فشل العملية بأكملها إذا فشل استخراج الكلمات المفتاحية
                }
                // تحويل الكائن إلى نموذج PdfFile للاستجابة
                var pdfFile = await ConvertToModelAsync(dataSourceFile);
                var successMessage = _localizationService.GetMessage("PdfFileUploaded", "Messages", language);
                return BaseResponse<List<PdfDocumentDTO>>.SuccessResponse(new List<PdfDocumentDTO> { pdfFile }, successMessage);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في رفع ملف PDF: {FileName}", fileName);
                var errorMessage = _localizationService.GetMessage("PdfFileUploadError", "Errors", language);
                return BaseResponse<List<PdfDocumentDTO>>.FailureResponse(errorMessage, 500);
            }

        }
    }
}