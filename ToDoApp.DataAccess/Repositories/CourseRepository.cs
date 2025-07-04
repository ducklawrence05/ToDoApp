using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Domains.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.DataAccess.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly IApplicationDBContext _dbContext;

        public CourseRepository(IApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            var query = _dbContext.Courses;

            return await query.ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            return await _dbContext.Courses.FindAsync(courseId);
        }

        public async Task<Course?> GetCourseByNameAsync(string courseName)
        {
            return await _dbContext.Courses.SingleOrDefaultAsync(c => c.Name == courseName);
        }

        public async Task<int> AddAsync(Course course)
        {
            await _dbContext.Courses.AddAsync(course);
            await _dbContext.SaveChangesAsync();

            return course.Id;
        }

        public async Task<int> UpdateAsync(Course course)
        {
            var courseFromDb = await GetCourseByIdAsync(course.Id);

            if (courseFromDb == null)
            {
                throw new Exception("Course not found");
            }

            _dbContext.Entry(courseFromDb).CurrentValues.SetValues(course);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int courseId)
        {
            var course = await GetCourseByIdAsync(courseId);

            if (course == null)
            {
                throw new Exception("Course not found");
            }

            _dbContext.Courses.Remove(course);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
