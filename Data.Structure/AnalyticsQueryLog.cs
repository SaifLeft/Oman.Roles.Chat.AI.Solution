﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Data.Structure;

public partial class AnalyticsQueryLog
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string QueryText { get; set; }

    public DateTime QueryDate { get; set; }

    public string Topic { get; set; }

    public string SessionId { get; set; }

    public long CreatedByUserId { get; set; }

    public DateTime CreateDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User User { get; set; }
}