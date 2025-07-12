using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using ToDoApp.DataAccess.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.DataAccess.Repositories
{
    public class CachedRepository<T> : IGenericRepository<T>
        where T : IEntity
    {
        private readonly IGenericRepository<T> _decoratee;
        private readonly IMemoryCache _cache;

        public CachedRepository(IGenericRepository<T> decoratee, IMemoryCache cache)
        {
            _decoratee = decoratee;
            _cache = cache;
        }

        public async Task<int> AddAsync(T entity)
        {
            _cache.Set(GetCacheKey(entity.Id), entity, TimeSpan.FromSeconds(30));

            return await _decoratee.AddAsync(entity);
        }

        private static string GetCacheKey(int id)
        {
            return $"{typeof(T).FullName}_{id}";
        }

        public async Task<int> DeleteAsync(int id)
        {
            _cache.Remove(GetCacheKey(id));

            return await _decoratee.DeleteAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>>? include = null)
        {
            var cacheKey = $"{typeof(T).FullName}_All";

            return await _cache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(30);

                return await _decoratee.GetAllAsync(include);
            }) ?? Enumerable.Empty<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var cacheKey = GetCacheKey(id);

            return await _cache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(30);

                return await _decoratee.GetByIdAsync(id);
            });
        }

        public async Task<int> UpdateAsync(T entity)
        {
            _cache.Remove(GetCacheKey(entity.Id));
            _cache.Set(GetCacheKey(entity.Id), entity, TimeSpan.FromSeconds(30));

            return await _decoratee.UpdateAsync(entity);
        }
    }
}
