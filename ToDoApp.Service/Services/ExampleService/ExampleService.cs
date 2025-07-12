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
        public string CreateCourseWithCurrentTime(string courseName)
        {
            // DateTime.Now is static and cannot be mocked
            var currentTime = DateTime.Now;
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
