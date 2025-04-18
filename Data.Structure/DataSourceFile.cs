﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Data.Structure;

public partial class DataSourceFile
{
    public long Id { get; set; }

    public long UploadedBy { get; set; }

    public bool IsPublic { get; set; }

    public bool IsActive { get; set; }

    public string FileName { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string FilePath { get; set; }

    /// <summary>
    /// bytes
    /// </summary>
    public long Size { get; set; }

    public bool IsKnowledgeBase { get; set; }

    public string FileType { get; set; }

    public long PageCount { get; set; }

    public byte[] Content { get; set; }

    public string ContentType { get; set; }

    public long CreatedByUserId { get; set; }

    public DateTime CreateDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ConversationPdfReference> ConversationPdfReferences { get; set; } = new List<ConversationPdfReference>();

    public virtual ICollection<DataSourceFileKeyword> DataSourceFileKeywords { get; set; } = new List<DataSourceFileKeyword>();

    public virtual User UploadedByNavigation { get; set; }
}