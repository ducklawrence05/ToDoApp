using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.DataAccess.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.DataAccess.Repositories
{
    public class SchoolRepository : GenericRepository<School>, ISchoolRepository
    {
        public SchoolRepository(IApplicationDBContext dbContext) : base(dbContext)
        {
        }

        public async Task<School?> GetSchoolByNameAsync(string schoolName)
        {
            return await _dbContext.Schools.SingleOrDefaultAsync(c => c.Name == schoolName);
        }
    }
}
