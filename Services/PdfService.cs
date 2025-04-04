using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.DTOs.Files;
using System.Text.Json;

namespace Services
{
    public interface IPdfService
    {
        Task<string> ExtractTextFromPdfAsync(string filePath);

        /// <summary>
        /// الحصول على قائمة ملفات PDF المتاحة
        /// </summary>
        Task<BaseResponse<PdfFilesResponse>> GetAvailablePdfFilesAsync(string language);

        /// <summary>
        /// الحصول على معلومات ملف PDF
        /// </summary>
        Task<BaseResponse<PdfDocumentDTO>> GetPdfFileInfoAsync(string fileName, string language);
        Task<long> GetPdfPageCountAsync(string filePath);

        /// <summary>
        /// تحديث معلومات ملف PDF
        /// </summary>
        Task<BaseResponse<PdfDocumentDTO>> UpdatePdfInfoAsync(string fileName, string title, string description, List<string> keywords, string userId, string language);
    }

    public class PdfService : IPdfService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PdfService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly string _pdfBasePath;
        private readonly string _pdfInfoPath;
        private readonly object _fileLock = new object();

        public PdfService(
            IConfiguration configuration,
            ILogger<PdfService> logger,
            ILocalizationService localizationService)
        {
            _configuration = configuration;
            _logger = logger;
            _localizationService = localizationService;

            // تحديد مسار ملفات PDF ومعلوماتها
            _pdfBasePath = _configuration["ChatSettings:PdfBasePath"] ??
                          Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfFiles");

            _pdfInfoPath = _configuration["ChatSettings:PdfInfoPath"] ??
                          Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PdfInfo");

            // التأكد من وجود المسارات
            Directory.CreateDirectory(_pdfBasePath);
            Directory.CreateDirectory(_pdfInfoPath);
        }

        /// <summary>
        /// الحصول على قائمة ملفات PDF المتاحة
        /// </summary>
        public async Task<BaseResponse<PdfFilesResponse>> GetAvailablePdfFilesAsync(string language)
        {
            try
            {
                var files = Directory.GetFiles(_pdfBasePath, "*.pdf");
                var pdfFiles = new List<PdfDocumentDTO>();

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var fileInfo = await GetPdfFileInfoInternalAsync(fileName);
                    pdfFiles.Add(fileInfo);
                }

                var response = new PdfFilesResponse
                {
                    Files = pdfFiles,
                    TotalCount = pdfFiles.Count
                };

                return BaseResponse<PdfFilesResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على قائمة ملفات PDF");
                var errorMessage = _localizationService.GetMessage("PdfFilesRetrievalError", "Errors", language);
                return BaseResponse<PdfFilesResponse>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على معلومات ملف PDF
        /// </summary>
        public async Task<BaseResponse<PdfDocumentDTO>> GetPdfFileInfoAsync(string fileName, string language)
        {
            try
            {
                var fileInfo = await GetPdfFileInfoInternalAsync(fileName);

                if (fileInfo == null)
                {
                    var errorMessage = _localizationService.GetMessage("PdfFileNotFound", "Errors", language);
                    return BaseResponse<PdfDocumentDTO>.FailureResponse(string.Format(errorMessage, fileName), 404);
                }

                return BaseResponse<PdfDocumentDTO>.SuccessResponse(fileInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على معلومات ملف PDF {fileName}", fileName);
                var errorMessage = _localizationService.GetMessage("PdfFileInfoRetrievalError", "Errors", language);
                return BaseResponse<PdfDocumentDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// تحديث معلومات ملف PDF
        /// </summary>
        public async Task<BaseResponse<PdfDocumentDTO>> UpdatePdfInfoAsync(string fileName, string title, string description, List<string> keywords, string userId, string language)
        {
            try
            {
                var fileInfo = await GetPdfFileInfoInternalAsync(fileName);

                if (fileInfo == null)
                {
                    var errorMessage = _localizationService.GetMessage("PdfFileNotFound", "Errors", language);
                    return BaseResponse<PdfDocumentDTO>.FailureResponse(string.Format(errorMessage, fileName), 404);
                }

                // تحديث المعلومات
                fileInfo.Title = title;
                fileInfo.Description = description;
                fileInfo.Keywords = keywords;

                // حفظ المعلومات
                SavePdfFileInfo(fileInfo);

                var successMessage = _localizationService.GetMessage("PdfFileInfoUpdated", "Messages", language);
                return BaseResponse<PdfDocumentDTO>.SuccessResponse(fileInfo, successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث معلومات ملف PDF {fileName}", fileName);
                var errorMessage = _localizationService.GetMessage("PdfFileInfoUpdateError", "Errors", language);
                return BaseResponse<PdfDocumentDTO>.FailureResponse(errorMessage, 500);
            }
        }

        /// <summary>
        /// الحصول على معلومات ملف PDF داخليًا
        /// </summary>
        private async Task<PdfDocumentDTO?> GetPdfFileInfoInternalAsync(string fileName)
        {
            var pdfFilePath = Path.Combine(_pdfBasePath, fileName);
            if (!File.Exists(pdfFilePath))
            {
                return null;
            }

            var infoFilePath = Path.Combine(_pdfInfoPath, $"{fileName}.json");

            // إذا كان ملف المعلومات موجودًا، نقرأه
            if (File.Exists(infoFilePath))
            {
                var json = await File.ReadAllTextAsync(infoFilePath);
                var fileInfo = JsonSerializer.Deserialize<PdfDocumentDTO>(json);

                if (fileInfo != null)
                {
                    return fileInfo;
                }
            }

            // إذا لم يكن ملف المعلومات موجودًا، ننشئه
            var fileSize = new FileInfo(pdfFilePath).Length;
            var fileInfo2 = new PdfDocumentDTO
            {
                FileName = fileName,
                Title = Path.GetFileNameWithoutExtension(fileName),
                Description = "",
                UploadDate = File.GetCreationTime(pdfFilePath),
                UploadedBy = "",
                Size = fileSize,
                Keywords = new List<string>()
            };

            SavePdfFileInfo(fileInfo2);
            return fileInfo2;
        }

        /// <summary>
        /// حفظ معلومات ملف PDF
        /// </summary>
        private void SavePdfFileInfo(PdfDocumentDTO fileInfo)
        {
            try
            {
                lock (_fileLock)
                {
                    var infoFilePath = Path.Combine(_pdfInfoPath, $"{fileInfo.FileName}.json");
                    var json = JsonSerializer.Serialize(fileInfo, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(infoFilePath, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حفظ معلومات ملف PDF {fileName}", fileInfo.FileName);
            }
        }

        /// <summary>
        /// استخراج النص من ملف PDF
        /// </summary>
        public async Task<string> ExtractTextFromPdfAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("ملف PDF غير موجود: {filePath}", filePath);
                    return string.Empty;
                }

                return await Task.Run(() =>
                {
                    var text = new System.Text.StringBuilder();
                    using (var reader = new PdfReader(filePath))
                    {
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            var strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                            string pageText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                            text.AppendLine(pageText);
                        }
                    }
                    return text.ToString();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء استخراج النص من ملف PDF {filePath}", filePath);
                return string.Empty;
            }
        }

        /// <summary>
        /// الحصول على عدد صفحات ملف PDF
        /// </summary>
        public async Task<long> GetPdfPageCountAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("ملف PDF غير موجود: {filePath}", filePath);
                    return 0;
                }

                return await Task.Run(() =>
                {
                    using (var reader = new PdfReader(filePath))
                    {
                        return reader.NumberOfPages;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء الحصول على عدد صفحات ملف PDF {filePath}", filePath);
                return 0;
            }
        }
    }
}