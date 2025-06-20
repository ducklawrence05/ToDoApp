using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ToDoApp.Domains.Interface;

namespace ToDoApp.Infrastructures.Interceptors
{
    public class AddedAndModifiedInterceptor : SaveChangesInterceptor
    {
        //btvn
        //add thêm 2 prop DeletedBy và DeletedAt
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context as ApplicationDBContext;
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if(entry.State == EntityState.Added)
                {
                    if (entry.Entity is ICreatedAt createdAtEntity)
                    {
                        createdAtEntity.CreatedAt = DateTime.Now;
                    }
                    if (entry.Entity is ICreatedBy createdByEntity)
                    {
                        createdByEntity.CreatedBy = 1;
                    }
                }
                if(entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IUpdatedAt updatedAtEntity)
                    {
                        updatedAtEntity.UpdatedAt = DateTime.Now;
                    }
                    if (entry.Entity is IUpdatedBy updatedByEntity)
                    {
                        updatedByEntity.UpdatedBy = 1;
                    }
                }
            }
            return base.SavingChanges(eventData, result);
        }
    }
}
