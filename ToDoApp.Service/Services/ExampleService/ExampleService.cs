using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.DataAccess.Entities;

namespace ToDoApp.Service.Services.ExampleService
{
    public class ExampleService
    {
        // Virtual để mock trong test
        protected virtual DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        public string CreateCourseWithCurrentTime(string courseName)
        {
            var currentTime = GetCurrentTime();

            var course = new Course
            {
                Name = courseName,
                StartDate = currentTime,
                CreatedAt = currentTime,
            };

            return $"Course '{courseName}' created at {currentTime:yyyy-MM-dd HH:mm:ss:fffffff}";
        }
    }
}
