using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.DataAccess.Entities;

namespace ToDoApp.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : IEntity
    {
        Task<int> AddAsync(T entity);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>>? include = null);
        Task<T?> GetByIdAsync(int id);
        Task<int> UpdateAsync(T entity);
    }
}
