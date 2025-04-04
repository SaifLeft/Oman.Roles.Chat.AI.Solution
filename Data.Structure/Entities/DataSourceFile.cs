using Data.Structure.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Structure.Entities
{
    /// <summary>
    /// يمثل ملف مصدر بيانات في النظام
    /// </summary>
    public class DataSourceFile : IBaseAuditableEntity
    {
        /// <summary>
        /// المعرف
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// معرف فريد خارجي (GUID) للملف
        /// </summary>
        [Required, MaxLength(50)]
        public string ExternalId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// الاسم الأصلي للملف
        /// </summary>
        [Required, MaxLength(255)]
        public string OriginalName { get; set; } = string.Empty;

        /// <summary>
        /// اسم الملف في النظام
        /// </summary>
        [Required, MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// نوع الملف
        /// </summary>
        [Required, MaxLength(50)]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// حجم الملف بالبايت
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// مسار الملف
        /// </summary>
        [Required, MaxLength(512)]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// معرف المستخدم
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// المستخدم المالك للملف
        /// </summary>
        [ForeignKey("UserId")]
        public User? User { get; set; }

        /// <summary>
        /// وصف الملف
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// عدد مرات تنزيل الملف
        /// </summary>
        public int DownloadCount { get; set; } = 0;

        /// <summary>
        /// المحتوى النصي المستخرج من الملف (إن كان PDF)
        /// </summary>
        public string? ExtractedText { get; set; }

        /// <summary>
        /// عدد صفحات الملف (إن كان PDF)
        /// </summary>
        public int? PageCount { get; set; }

        /// <summary>
        /// علامات الملف (مخزنة كنص مفصول بفواصل)
        /// </summary>
        [MaxLength(500)]
        public string? Tags { get; set; }

        /// <summary>
        /// هل الملف نشط
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// هل تم حذف الملف
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        #region Audit Fields

        /// <summary>
        /// معرف المستخدم الذي أنشأ السجل
        /// </summary>
        public long CreatedByUserId { get; set; }

        /// <summary>
        /// تاريخ إنشاء السجل
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// معرف المستخدم الذي عدل السجل
        /// </summary>
        public long? ModifiedByUserId { get; set; }

        /// <summary>
        /// تاريخ تعديل السجل
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// رقم النسخة
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// تعيين معلومات إنشاء السجل
        /// </summary>
        public void SetCreatedAuditInfo()
        {
            CreatedByUserId = UserId;
            CreatedDate = DateTime.Now;
        }

        /// <summary>
        /// تعيين معلومات تعديل السجل
        /// </summary>
        public void SetModifiedAuditInfo()
        {
            ModifiedByUserId = UserId;
            ModifiedDate = DateTime.Now;
            Version++;
        }

        #endregion
    }
} 