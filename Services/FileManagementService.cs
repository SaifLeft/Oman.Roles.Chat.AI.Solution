using Data.Structure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.Files;

namespace Services
{
    /// <summary>
    /// واجهة خدمة إدارة الملفات
    /// </summary>
    public interface IFileManagementService
    {
        /// <summary>
        /// رفع ملف PDF 
        /// </summary>
        Task<BaseResponse<FileInfoDTO>> UploadPdfFileAsync(IFormFile file, long userId, string language);

        /// <summary>
        /// رفع صورة
        /// </summary>
        Task<BaseResponse<FileInfoDTO>> UploadImageAsync(IFormFile file, long userId, string language);

        /// <summary>
        /// الحصول على ملف بواسطة المعرف
        /// </summary>
        Task<BaseResponse<FileInfoDTO>> GetFileByIdAsync(string fileId, long userId, string language);

        /// <summary>
        /// الحصول على محتوى ملف بتنسيق octet-stream
        /// </summary>
        Task<BaseResponse<byte[]>> GetFileContentAsync(string fileId, long userId, string language);

        /// <summary>
        /// حذف ملف
        /// </summary>
        Task<BaseResponse<bool>> DeleteFileAsync(string fileId, long userId, string language);

        /// <summary>
        /// التحقق من صلاحية الملف
        /// </summary>
        Task<BaseResponse<bool>> ValidateFileAsync(IFormFile file, string fileType, string language);

        /// <summary>
        /// الحصول على قائمة الملفات التي رفعها المستخدم
        /// </summary>
        Task<PaginatedResponse<List<FileInfoDTO>>> GetUserFilesAsync(long userId, int page, int pageSize, string language);
    }

    /// <summary>
    /// تنفيذ خدمة إدارة الملفات
    /// </summary>
    public class FileManagementService : IFileManagementService
    {
        private readonly MuhamiContext _context;
        private readonly ILogger<FileManagementService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IConfiguration _configuration;
        private readonly IPdfService _pdfService;
        private readonly string _fileBasePath;
        private readonly string _pdfBasePath;
        private readonly string _imagesBasePath;
        private readonly List<string> _allowedImageTypes;
        private readonly List<string> _allowedDocumentTypes;
        private readonly long _maxImageSize;
        private readonly long _maxDocumentSize;
        private readonly int _maxUserFiles;

        public FileManagementService(
            MuhamiContext context,
            ILogger<FileManagementService> logger,
            ILocalizationService localizationService,
            IConfiguration configuration,
            IPdfService pdfService)
        {
            _context = context;
            _logger = logger;
            _localizationService = localizationService;
            _configuration = configuration;
            _pdfService = pdfService;

            // تحديد مسارات تخزين الملفات من ملف الإعدادات
            _fileBasePath = _configuration["FileStorage:BasePath"] ??
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
            _pdfBasePath = Path.Combine(_fileBasePath, _configuration["FileStorage:PdfFolderName"] ?? "Documents");
            _imagesBasePath = Path.Combine(_fileBasePath, _configuration["FileStorage:ImagesFolderName"] ?? "Images");

            // تحديد أنواع الملفات المسموح بها
            _allowedImageTypes = _configuration.GetSection("FileStorage:AllowedImageTypes").Get<List<string>>() ??
                new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            _allowedDocumentTypes = _configuration.GetSection("FileStorage:AllowedDocumentTypes").Get<List<string>>() ??
                new List<string> { ".pdf" };

            // تحديد الحد الأقصى لحجم الملفات (بالميجابايت)
            if (!double.TryParse(_configuration["FileStorage:MaxImageSizeMB"], out double maxImageSizeMB))
            {
                maxImageSizeMB = 5; // القيمة الافتراضية 5 ميجابايت
            }
            _maxImageSize = (long)(maxImageSizeMB * 1024 * 1024);

            if (!double.TryParse(_configuration["FileStorage:MaxDocumentSizeMB"], out double maxDocumentSizeMB))
            {
                maxDocumentSizeMB = 20; // القيمة الافتراضية 20 ميجابايت
            }
            _maxDocumentSize = (long)(maxDocumentSizeMB * 1024 * 1024);

            // تحديد الحد الأقصى لعدد الملفات لكل مستخدم
            if (!int.TryParse(_configuration["FileStorage:MaxUserFiles"], out int maxUserFiles))
            {
                maxUserFiles = 50; // القيمة الافتراضية 50 ملف
            }
            _maxUserFiles = maxUserFiles;

            // تأكد من وجود المجلدات
            Directory.CreateDirectory(_pdfBasePath);
            Directory.CreateDirectory(_imagesBasePath);

            _logger.LogInformation("File storage initialized with basePath: {BasePath}, pdfPath: {PdfPath}, imagesPath: {ImagesPath}",
                _fileBasePath, _pdfBasePath, _imagesBasePath);
            _logger.LogInformation("File storage limits: MaxImageSize: {MaxImageSize}MB, MaxDocumentSize: {MaxDocumentSize}MB, MaxUserFiles: {MaxUserFiles}",
                maxImageSizeMB, maxDocumentSizeMB, maxUserFiles);
        }

        /// <summary>
        /// رفع ملف PDF
        /// </summary>
        public async Task<BaseResponse<FileInfoDTO>> UploadPdfFileAsync(IFormFile file, long userId, string language)
        {
            try
            {
                // التحقق من عدد الملفات الحالية للمستخدم
                var userFilesCount = await _context.DataSourceFiles
                    .Where(f => f.UploadedBy == userId && f.IsActive && !f.IsDeleted)
                    .CountAsync();

                if (userFilesCount >= _maxUserFiles)
                {
                    var errorMessage = _localizationService.GetMessage("PdfFilesLimitReached", "Errors", language);
                    var infoMessage = string.Format(_localizationService.GetMessage("PdfFilesLimitInfo", "Errors", language), userFilesCount, _maxUserFiles);
                    return BaseResponse<FileInfoDTO>.FailureResponse($"{errorMessage} {infoMessage}", 400);
                }

                // التحقق من صلاحية الملف
                var validationResult = await ValidateFileAsync(file, "pdf", language);
                if (!validationResult.Success)
                {
                    return BaseResponse<FileInfoDTO>.FailureResponse(validationResult.Message, validationResult.StatusCode);
                }

                // إنشاء اسم فريد للملف
                var fileId = Guid.NewGuid();
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = fileId.ToString() + extension;
                var filePath = Path.Combine(_pdfBasePath, fileName);

                // حفظ الملف
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // إنشاء ملف في قاعدة البيانات
                var dataSourceFile = new DataSourceFile
                {
                    FileName = file.FileName,
                    Title = Path.GetFileNameWithoutExtension(file.FileName),
                    FilePath = filePath,
                    FileType = "pdf",
                    ContentType = file.ContentType,
                    Size = file.Length,
                    UploadedBy = userId,
                    IsPublic = false,
                    IsActive = true,
                    CreatedByUserId = userId,
                    CreateDate = DateTime.UtcNow
                };

                _context.DataSourceFiles.Add(dataSourceFile);
                await _context.SaveChangesAsync();

                // إرجاع معلومات الملف
                var dataFileDto = new FileInfoDTO
                {
                    Id = dataSourceFile.Id.ToString(),
                    FileName = dataSourceFile.FileName,
                    FileType = dataSourceFile.FileType,
                    FileSize = dataSourceFile.Size,
                    UploadDate = dataSourceFile.CreateDate,
                    IsPublic = dataSourceFile.IsPublic
                };

                var successMessage = _localizationService.GetMessage("FileUploadedSuccessfully", "Messages", language);
                return BaseResponse<FileInfoDTO>.SuccessResponse(dataFileDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء رفع ملف PDF");
                var errorMessage = _localizationService.GetMessage("FileUploadError", "Errors", language);
                return BaseResponse<FileInfoDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// رفع صورة
        /// </summary>
        public async Task<BaseResponse<FileInfoDTO>> UploadImageAsync(IFormFile file, long userId, string language)
        {
            try
            {
                // التحقق من صلاحية الملف
                var validationResult = await ValidateFileAsync(file, "image", language);
                if (!validationResult.Success)
                {
                    return BaseResponse<FileInfoDTO>.FailureResponse(validationResult.Message, validationResult.StatusCode);
                }

                // إنشاء اسم فريد للملف
                var fileId = Guid.NewGuid();
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = fileId.ToString() + extension;
                var filePath = Path.Combine(_imagesBasePath, fileName);

                // حفظ الملف
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // إنشاء ملف في قاعدة البيانات
                var dataSourceFile = new DataSourceFile
                {
                    FileName = file.FileName,
                    Title = Path.GetFileNameWithoutExtension(file.FileName),
                    FilePath = filePath,
                    FileType = "image",
                    Size = file.Length,
                    UploadedBy = userId,
                    IsPublic = false,
                    IsActive = true,
                    CreatedByUserId = userId,
                    CreateDate = DateTime.UtcNow
                };

                _context.DataSourceFiles.Add(dataSourceFile);
                await _context.SaveChangesAsync();

                // إرجاع معلومات الملف
                var dataFileDto = new FileInfoDTO
                {
                    Id = dataSourceFile.Id.ToString(),
                    FileName = dataSourceFile.FileName,
                    FileType = dataSourceFile.FileType,
                    FileSize = dataSourceFile.Size,
                    UploadDate = dataSourceFile.CreateDate,
                    IsPublic = dataSourceFile.IsPublic
                };

                var successMessage = _localizationService.GetMessage("ImageUploadedSuccessfully", "Messages", language);
                return BaseResponse<FileInfoDTO>.SuccessResponse(dataFileDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء رفع صورة");
                var errorMessage = _localizationService.GetMessage("ImageUploadError", "Errors", language);
                return BaseResponse<FileInfoDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على ملف بواسطة المعرف
        /// </summary>
        public async Task<BaseResponse<FileInfoDTO>> GetFileByIdAsync(string fileId, long userId, string language)
        {
            try
            {
                // التحقق من صلاحية معرف الملف
                if (string.IsNullOrEmpty(fileId) || !long.TryParse(fileId, out long parsedFileId))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidFileId", "Errors", language);
                    return BaseResponse<FileInfoDTO>.FailureResponse(invalidIdMessage, 400);
                }

                // البحث عن الملف
                var file = await _context.DataSourceFiles
                    .FirstOrDefaultAsync(f => f.Id == parsedFileId && (f.UploadedBy == userId || f.IsPublic) && f.IsActive);

                if (file == null)
                {
                    var fileNotFoundMessage = _localizationService.GetMessage("FileNotFound", "Errors", language);
                    return BaseResponse<FileInfoDTO>.FailureResponse(fileNotFoundMessage, 404);
                }

                // إرجاع معلومات الملف
                var dataFileDto = new FileInfoDTO
                {
                    Id = file.Id.ToString(),
                    FileName = file.FileName,
                    FileType = file.FileType,
                    FileSize = file.Size,
                    PageCount = file.PageCount,
                    UploadDate = file.CreateDate,
                    IsPublic = file.IsPublic
                };

                var successMessage = _localizationService.GetMessage("FileRetrievedSuccessfully", "Messages", language);
                return BaseResponse<FileInfoDTO>.SuccessResponse(dataFileDto, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء استرجاع معلومات الملف");
                var errorMessage = _localizationService.GetMessage("FileRetrievalError", "Errors", language);
                return BaseResponse<FileInfoDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على محتوى ملف بتنسيق octet-stream
        /// </summary>
        public async Task<BaseResponse<byte[]>> GetFileContentAsync(string fileId, long userId, string language)
        {
            try
            {
                // التحقق من صلاحية معرف الملف
                if (string.IsNullOrEmpty(fileId) || !long.TryParse(fileId, out long parsedFileId))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidFileId", "Errors", language);
                    return BaseResponse<byte[]>.FailureResponse(invalidIdMessage, 400);
                }

                // البحث عن الملف
                var file = await _context.DataSourceFiles
                    .FirstOrDefaultAsync(f => f.Id == parsedFileId && (f.UploadedBy == userId || f.IsPublic) && f.IsActive);

                if (file == null)
                {
                    var fileNotFoundMessage = _localizationService.GetMessage("FileNotFound", "Errors", language);
                    return BaseResponse<byte[]>.FailureResponse(fileNotFoundMessage, 404);
                }

                // التحقق من وجود الملف على القرص
                if (!File.Exists(file.FilePath))
                {
                    var fileNotFoundMessage = _localizationService.GetMessage("FileNotFoundOnDisk", "Errors", language);
                    return BaseResponse<byte[]>.FailureResponse(fileNotFoundMessage, 404);
                }

                // قراءة محتوى الملف
                var fileContent = await File.ReadAllBytesAsync(file.FilePath);

                var successMessage = _localizationService.GetMessage("FileContentRetrievedSuccessfully", "Messages", language);
                return BaseResponse<byte[]>.SuccessResponse(fileContent, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء استرجاع محتوى الملف");
                var errorMessage = _localizationService.GetMessage("FileContentRetrievalError", "Errors", language);
                return BaseResponse<byte[]>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// حذف ملف
        /// </summary>
        public async Task<BaseResponse<bool>> DeleteFileAsync(string fileId, long userId, string language)
        {
            try
            {
                // التحقق من صلاحية معرف الملف
                if (string.IsNullOrEmpty(fileId) || !long.TryParse(fileId, out long parsedFileId))
                {
                    var invalidIdMessage = _localizationService.GetMessage("InvalidFileId", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(invalidIdMessage, 400);
                }

                // البحث عن الملف
                var file = await _context.DataSourceFiles
                    .FirstOrDefaultAsync(f => f.Id == parsedFileId && f.UploadedBy == userId && f.IsActive);

                if (file == null)
                {
                    var fileNotFoundMessage = _localizationService.GetMessage("FileNotFound", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(fileNotFoundMessage, 404);
                }

                // حذف منطقي للملف
                file.IsActive = false;
                file.IsDeleted = true;
                file.DeletedAt = DateTime.UtcNow;
                file.ModifiedByUserId = userId;
                file.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var successMessage = _localizationService.GetMessage("FileDeletedSuccessfully", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف الملف");
                var errorMessage = _localizationService.GetMessage("FileDeletionError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// التحقق من صلاحية الملف
        /// </summary>
        public async Task<BaseResponse<bool>> ValidateFileAsync(IFormFile file, string fileType, string language)
        {
            try
            {
                // التحقق من وجود الملف
                if (file == null || file.Length == 0)
                {
                    var emptyFileMessage = _localizationService.GetMessage("EmptyFile", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(emptyFileMessage, 400);
                }

                // التحقق من امتداد الملف
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (fileType.ToLower() == "pdf")
                {
                    // التحقق من أن الملف هو PDF
                    if (!_allowedDocumentTypes.Contains(extension))
                    {
                        var invalidTypeMessage = _localizationService.GetMessage("InvalidPdfFileType", "Errors", language);
                        return BaseResponse<bool>.FailureResponse(invalidTypeMessage, 400);
                    }

                    // التحقق من حجم الملف
                    if (file.Length > _maxDocumentSize)
                    {
                        var fileTooLargeMessage = _localizationService.GetMessage("FileTooLarge", "Errors", language);
                        var fileSizeInfo = string.Format(_localizationService.GetMessage("PdfFileSizeLimitInfo", "Errors", language),
                            Math.Round(file.Length / (1024.0 * 1024.0), 2),
                            Math.Round(_maxDocumentSize / (1024.0 * 1024.0), 2));
                        return BaseResponse<bool>.FailureResponse($"{fileTooLargeMessage} {fileSizeInfo}", 400);
                    }
                }
                else if (fileType.ToLower() == "image")
                {
                    // التحقق من أن الملف هو صورة
                    if (!_allowedImageTypes.Contains(extension))
                    {
                        var invalidTypeMessage = _localizationService.GetMessage("InvalidImageFileType", "Errors", language);
                        return BaseResponse<bool>.FailureResponse(invalidTypeMessage, 400);
                    }

                    // التحقق من حجم الملف
                    if (file.Length > _maxImageSize)
                    {
                        var fileTooLargeMessage = _localizationService.GetMessage("FileTooLarge", "Errors", language);
                        var fileSizeInfo = string.Format(_localizationService.GetMessage("PdfFileSizeLimitInfo", "Errors", language),
                            Math.Round(file.Length / (1024.0 * 1024.0), 2),
                            Math.Round(_maxImageSize / (1024.0 * 1024.0), 2));
                        return BaseResponse<bool>.FailureResponse($"{fileTooLargeMessage} {fileSizeInfo}", 400);
                    }
                }
                else
                {
                    var unsupportedTypeMessage = _localizationService.GetMessage("UnsupportedFileType", "Errors", language);
                    return BaseResponse<bool>.FailureResponse(unsupportedTypeMessage, 400);
                }

                var successMessage = _localizationService.GetMessage("FileValidationSuccess", "Messages", language);
                return BaseResponse<bool>.SuccessResponse(true, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء التحقق من صلاحية الملف");
                var errorMessage = _localizationService.GetMessage("FileValidationError", "Errors", language);
                return BaseResponse<bool>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على قائمة الملفات التي رفعها المستخدم
        /// </summary>
        public async Task<PaginatedResponse<List<FileInfoDTO>>> GetUserFilesAsync(long userId, int page, int pageSize, string language)
        {
            try
            {
                // التحقق من صحة معاملات الترقيم
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                // إنشاء استعلام لاسترجاع ملفات المستخدم
                var query = _context.DataSourceFiles
                    .Where(f => f.UploadedBy == userId && f.IsActive && !f.IsDeleted)
                    .OrderByDescending(f => f.CreateDate);

                // الحصول على العدد الإجمالي للملفات
                var totalCount = await query.CountAsync();

                // حساب عدد الصفحات
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                // استرجاع ملفات الصفحة المطلوبة
                var files = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // تحويل قائمة الملفات إلى DTOs
                var fileDtos = files.Select(f => new FileInfoDTO
                {
                    Id = f.Id.ToString(),
                    FileName = f.FileName,
                    FileType = f.FileType,
                    FileSize = f.Size,
                    PageCount = f.PageCount,
                    UploadDate = f.CreateDate,
                    IsPublic = f.IsPublic
                }).ToList();

                var successMessage = _localizationService.GetMessage("UserFilesRetrievedSuccessfully", "Messages", language);
                return new PaginatedResponse<List<FileInfoDTO>>
                {
                    Data = fileDtos,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Page = page,
                    PageSize = pageSize,
                    Success = true,
                    Message = successMessage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء استرجاع ملفات المستخدم");
                var errorMessage = _localizationService.GetMessage("UserFilesRetrievalError", "Errors", language);
                throw new Exception(errorMessage, ex);
            }
        }
    }
}