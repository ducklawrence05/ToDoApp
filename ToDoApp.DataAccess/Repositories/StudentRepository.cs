using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Infrastructures;
using ToDoApp.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ToDoApp.DataAccess.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IApplicationDBContext _dbContext;

        public StudentRepository(IApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync(
            Expression<Func<Student, object>>? include)
        {
            var query = _dbContext.Students.AsQueryable();

            if (include != null)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }
    }
}
