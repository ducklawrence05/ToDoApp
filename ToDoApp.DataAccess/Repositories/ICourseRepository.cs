using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Domains.Entities;

namespace ToDoApp.DataAccess.Repositories
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetCoursesAsync();

        Task<Course?> GetCourseByIdAsync(int courseId);

        Task<Course?> GetCourseByNameAsync(string courseName);
        Task<int> AddAsync(Course course);
        Task<int> UpdateAsync(Course course);
        Task<int> DeleteAsync(int courseId);
    }
}
