using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ToDoApp.Domains.Interface;

namespace ToDoApp.Infrastructures.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context as ApplicationDBContext;

            var entries = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).ToList();
            foreach (var entry in entries)
            {
                bool isModified = false;

                if (entry.Entity is IDeletedAt deletedAtEntity)
                {
                    if(deletedAtEntity.DeletedAt == null)
                    {
                        deletedAtEntity.DeletedAt = DateTime.Now;
                        isModified = true;
                    }
                }
                if (entry.Entity is IDeletedBy deletedByEntity)
                {
                    if(deletedByEntity.DeletedBy == null)
                    {
                        deletedByEntity.DeletedBy = 1;
                        isModified = true;
                    }
                }

                if (entry.Entity is IDeletedAt || entry.Entity is IDeletedBy)
                {
                    entry.State = isModified
                        ? EntityState.Modified
                        : EntityState.Unchanged;
                }
            }

            return base.SavingChanges(eventData, result);
        }
    }
}
