using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;
using System.Text.Json;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        private static string Serialize(PropertyValues values)
        {
            var dict = new Dictionary<string, object?>();

            foreach (var prop in values.Properties)
            {
                dict[prop.Name] = values[prop];
            }

            return JsonSerializer.Serialize(dict);
        }

        // ================= CURRENT USER =================
        private int GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userId, out var id) ? id : 0;
        }

        // ================= DELTA SERIALIZER =================
        private static (string? OldValues, string? NewValues) GetDelta(EntityEntry entry)
        {
            var oldValues = new Dictionary<string, object?>();
            var newValues = new Dictionary<string, object?>();

            foreach (var prop in entry.Properties)
            {
                if (prop.IsTemporary) continue;

                var original = entry.OriginalValues[prop.Metadata.Name];
                var current = entry.CurrentValues[prop.Metadata.Name];

                if (!Equals(original, current))
                {
                    oldValues[prop.Metadata.Name] = original;
                    newValues[prop.Metadata.Name] = current;
                }
            }

            if (!oldValues.Any())
                return (null, null);

            return (
                JsonSerializer.Serialize(oldValues),
                JsonSerializer.Serialize(newValues)
            );
        }

        // ================= AUDIT CORE =================
        //        public override async Task<int> SaveChangesAsync(
        //       CancellationToken cancellationToken = default)
        //        {
        //            var auditLogs = new List<AuditLog>();

        //            foreach (var entry in ChangeTracker.Entries<TaskItem>())
        //            {
        //                if (entry.State != EntityState.Added &&
        //                    entry.State != EntityState.Modified)
        //                    continue;

        //                string action;
        //                string? oldValues = null;
        //                string? newValues = null;

        //                if (entry.State == EntityState.Added)
        //                {
        //                    action = "Created";
        //                    newValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
        //                }
        //                else
        //                {
        //                    action = "Updated";

        //                    var delta = GetDelta(entry);
        //                    oldValues = delta.OldValues;
        //                    newValues = delta.NewValues;

        //                    if (oldValues == null && newValues == null)
        //                        continue;
        //                }

        //                auditLogs.Add(new AuditLog
        //                {
        //                    UserId = GetCurrentUserId(),
        //                    EntityName = nameof(TaskItem),
        //                    Action = action,
        //                    OldValues = oldValues,
        //                    NewValues = newValues,
        //                    CreatedAt = DateTime.UtcNow
        //                });
        //            }

        //            var result = await base.SaveChangesAsync(cancellationToken);

        //            if (auditLogs.Any())
        //            {
        //                foreach (var log in auditLogs)
        //                {
        //                    var taskEntry = ChangeTracker.Entries<TaskItem>()
        //                        .FirstOrDefault(e => e.State == EntityState.Unchanged);

        //                    if (taskEntry != null)
        //                        log.EntityId = taskEntry.Entity.Id;
        //                }

        //                AuditLogs.AddRange(auditLogs);
        //                await base.SaveChangesAsync(cancellationToken);
        //            }

        //            return result;
        //        }
        //    }
        //}
    }
}