using Data.Structure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace Data.Structure
{
    public partial class MuhamiContext : DbContext
    {
        private const int InitialVersion = 1;

        private static readonly string CreatedByUserIdProp = nameof(IBaseAuditableEntity.CreatedByUserId);
        private static readonly string CreatedDateProp = nameof(IBaseAuditableEntity.CreatedDate);
        private static readonly string VersionProp = nameof(IBaseAuditableEntity.Version);
        private static readonly string ModifiedByUserIdProp = nameof(IBaseAuditableEntity.ModifiedByUserId);
        private static readonly string ModifiedDateProp = nameof(IBaseAuditableEntity.ModifiedDate);

        private readonly IHttpContextAccessor _httpContextAccessor;

        public MuhamiContext(DbContextOptions<MuhamiContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
        }


        public override int SaveChanges()
        {
            var userId = GetUserIdFromClaims();
            UpdateAuditFields(userId);
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = GetUserIdFromClaims();
            UpdateAuditFields(userId);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields(long userId)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                var entity = entry.Entity;
                var entityType = entity.GetType();

                if (entry.State == EntityState.Added)
                {
                    SetPropertyValue(entity, entityType, CreatedByUserIdProp, userId);
                    SetPropertyValue(entity, entityType, CreatedDateProp, DateTime.Now);
                    SetPropertyValue(entity, entityType, VersionProp, InitialVersion);
                }
                else if (entry.State == EntityState.Modified)
                {
                    SetPropertyValue(entity, entityType, ModifiedByUserIdProp, userId);
                    SetPropertyValue(entity, entityType, ModifiedDateProp, DateTime.Now);

                    if (entityType.GetProperty(VersionProp) is PropertyInfo versionProperty)
                    {
                        var currentVersion = (long)versionProperty.GetValue(entity);
                        versionProperty.SetValue(entity, currentVersion + 1);
                    }
                }
            }
        }

        private void SetPropertyValue(object entity, Type entityType, string propertyName, object value)
        {
            if (entityType.GetProperty(propertyName) is PropertyInfo property)
            {
                property.SetValue(entity, value);
            }
        }

        private long GetUserIdFromClaims()
        {
            string? userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out long userId))
            {
                return userId;
            }
            else
            {
                return 10012; // default user
            }
        }


    }
}
