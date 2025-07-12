using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Infrastructures;
using System.Linq.Expressions;

namespace ToDoApp.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> 
        where T : class, IEntity
    {
        protected readonly IApplicationDBContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(IApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>>? include = null)
        {
            var query = _dbSet.AsQueryable();
            if (include != null)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> UpdateAsync(T entity)
        {
            var entityFromDb = await GetByIdAsync(entity.Id);
            if (entityFromDb == null)
            {
                throw new Exception($"{typeof(T).Name} not found");
            }

            _dbContext.Entry(entityFromDb).CurrentValues.SetValues(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int entityId)
        {
            var entityFromDb = await GetByIdAsync(entityId);
            if (entityFromDb == null)
            {
                throw new Exception($"{typeof(T).Name} not found");
            }

            _dbSet.Remove(entityFromDb);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
