using FluentValidation;
using System.Text.Json.Serialization;

namespace Models.DTOs.Files
{
    /// <summary>
    /// طلب رفع ملف
    /// </summary>
    public class UploadFileRequestDTO
    {
        /// <summary>
        /// محتوى الملف كمصفوفة بايتات
        /// </summary>
        [JsonPropertyName("fileContent")]
        public byte[] FileContent { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// اسم الملف
        /// </summary>
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// نوع محتوى الملف
        /// </summary>
        [JsonPropertyName("contentType")]
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// وصف الملف (اختياري)
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// علامات الملف (اختياري)
        /// </summary>
        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }
    }

    /// <summary>
    /// مدقق صحة طلب رفع الملف
    /// </summary>
    public class UploadFileRequestDTOValidator : AbstractValidator<UploadFileRequestDTO>
    {
        public UploadFileRequestDTOValidator()
        {
            RuleFor(x => x.FileContent)
                .NotEmpty().WithMessage("يجب تحديد محتوى الملف المراد رفعه");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("يجب تحديد اسم الملف")
                .MaximumLength(255).WithMessage("يجب ألا يتجاوز اسم الملف 255 حرفًا");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("يجب تحديد نوع محتوى الملف");

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(500).WithMessage("يجب ألا يتجاوز وصف الملف 500 حرف");
            });
        }
    }
} 