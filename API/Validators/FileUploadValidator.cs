using FluentValidation;
using Microsoft.AspNetCore.Http;
using Services;

namespace API.Validators
{
    /// <summary>
    /// فلتر التحقق من صحة رفع الملفات
    /// </summary>
    public class FileUploadValidator : AbstractValidator<IFormFile>
    {
        private readonly ILocalizationService _localizationService;
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] _allowedPdfExtensions = { ".pdf" };
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        
        public FileUploadValidator(ILocalizationService localizationService, bool pdfOnly = false)
        {
            _localizationService = localizationService;
            
            RuleFor(x => x.Length)
                .NotNull().WithMessage(x => _localizationService.GetMessage("FileRequired", "Validation", "ar"))
                .LessThanOrEqualTo(_maxFileSize).WithMessage(x => _localizationService.GetMessage("FileTooLarge", "Validation", "ar"));
                
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("FilenameRequired", "Validation", "ar"))
                .Must(fileName => 
                {
                    var extension = Path.GetExtension(fileName).ToLower();
                    if (pdfOnly)
                    {
                        return _allowedPdfExtensions.Contains(extension);
                    }
                    return _allowedPdfExtensions.Contains(extension) || _allowedImageExtensions.Contains(extension);
                })
                .WithMessage(x => pdfOnly 
                    ? _localizationService.GetMessage("OnlyPdfAllowed", "Validation", "ar")
                    : _localizationService.GetMessage("OnlyPdfAndImagesAllowed", "Validation", "ar"));
                
            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("ContentTypeRequired", "Validation", "ar"))
                .Must(contentType => 
                {
                    if (pdfOnly)
                    {
                        return contentType.ToLower() == "application/pdf";
                    }
                    return contentType.ToLower() == "application/pdf" || 
                           contentType.ToLower().StartsWith("image/");
                })
                .WithMessage(x => pdfOnly 
                    ? _localizationService.GetMessage("OnlyPdfContentTypeAllowed", "Validation", "ar")
                    : _localizationService.GetMessage("OnlyPdfAndImageContentTypesAllowed", "Validation", "ar"));
        }
    }

    /// <summary>
    /// نموذج طلب تحميل ملف
    /// </summary>
    public class FileUploadRequestDTO
    {
        /// <summary>
        /// الملف المراد تحميله
        /// </summary>
        public IFormFile File { get; set; }
        
        /// <summary>
        /// عنوان الملف
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// وصف الملف
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// الكلمات المفتاحية المرتبطة بالملف
        /// </summary>
        public List<string> Keywords { get; set; } = new List<string>();
    }
    
    /// <summary>
    /// التحقق من صحة نموذج تحميل الملف
    /// </summary>
    public class FileUploadRequestDTOValidator : AbstractValidator<FileUploadRequestDTO>
    {
        private readonly ILocalizationService _localizationService;
        
        public FileUploadRequestDTOValidator(ILocalizationService localizationService, bool pdfOnly = false)
        {
            _localizationService = localizationService;
            
            RuleFor(x => x.File)
                .NotNull().WithMessage(x => _localizationService.GetMessage("FileRequired", "Validation", "ar"))
                .SetValidator(new FileUploadValidator(localizationService, pdfOnly));
                
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("TitleRequired", "Validation", "ar"))
                .MaximumLength(100).WithMessage(x => _localizationService.GetMessage("TitleTooLong", "Validation", "ar"));
                
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage(x => _localizationService.GetMessage("DescriptionTooLong", "Validation", "ar"));
                
            RuleFor(x => x.Keywords)
                .Must(keywords => keywords == null || keywords.Count <= 20)
                .WithMessage(x => _localizationService.GetMessage("TooManyKeywords", "Validation", "ar"));
                
            RuleForEach(x => x.Keywords)
                .NotEmpty().WithMessage(x => _localizationService.GetMessage("EmptyKeyword", "Validation", "ar"))
                .MaximumLength(50).WithMessage(x => _localizationService.GetMessage("KeywordTooLong", "Validation", "ar"));
        }
    }
} 