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
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(IApplicationDBContext dbContext) : base(dbContext)
        {
        }

        public async Task<Course?> GetCourseByNameAsync(string courseName)
        {
            return await _dbContext.Courses.SingleOrDefaultAsync(c => c.Name == courseName);
        }
    }
}
