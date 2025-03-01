using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Data.Structure.Common;
public interface IBaseAuditableEntity
{
    long CreatedByUserId { get; set; }
    DateTime CreatedDate { get; set; }
    long? ModifiedByUserId { get; set; }
    DateTime? ModifiedDate { get; set; }
    long Version { get; set; }
    void SetCreatedAuditInfo();
    void SetModifiedAuditInfo();
}

public partial class BaseAuditableEntity : IBaseAuditableEntity
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private long userId = 10000012;

    public BaseAuditableEntity(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        userId = long.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        if (userId == 0)
        {
            userId = 10000012;
        }
    }

    public long CreatedByUserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public long? ModifiedByUserId { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public long Version { get; set; }

    public void SetCreatedAuditInfo()
    {
        CreatedByUserId = userId;
        CreatedDate = DateTime.Now;
    }

    public void SetModifiedAuditInfo()
    {
        ModifiedByUserId = userId;
        ModifiedDate = DateTime.Now;
    }
}

