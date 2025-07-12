using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ToDoApp.DataAccess.Entities;

namespace ToDoApp.Infrastructures.Interceptors
{
    public class AudiLogInterceptor : SaveChangesInterceptor
    {
        private List<EntityEntry> addedEntities = new List<EntityEntry>();
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context as ApplicationDBContext;
            var auditLogs = new List<AuditLog>();
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if(entry.Entity is AuditLog)
                {
                    continue;
                }

                var log = new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    CreatedAt = DateTime.Now,
                    Action = entry.State.ToString(),
                };

                if (entry.State == EntityState.Added)
                {
                    addedEntities.Add(entry);
                    //log.NewValue = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                }
                if (entry.State == EntityState.Modified)
                {
                    log.OldValue = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                    log.NewValue = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                    auditLogs.Add(log);
                }
                if (entry.State == EntityState.Deleted)
                {
                    log.OldValue = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                    auditLogs.Add(log);
                }
            }

            if(auditLogs.Any())
            {
                context.AuditLogs.AddRange(auditLogs); //add 1 lần nhiều cái
            }
            return base.SavingChanges(eventData, result);
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            var context = eventData.Context as ApplicationDBContext;
            if (addedEntities.Any())
            {
                var auditLogs = addedEntities.Select(entity => new AuditLog
                {
                    EntityName = entity.Entity.GetType().Name,
                    CreatedAt = DateTime.Now,
                    Action = EntityState.Added.ToString(),
                    NewValue = JsonSerializer.Serialize(entity.CurrentValues.ToObject())
                });
                context.AuditLogs.AddRange(auditLogs);
                addedEntities.Clear();
                context.SaveChanges();
            }

            return base.SavedChanges(eventData, result);
        }
    }
}
