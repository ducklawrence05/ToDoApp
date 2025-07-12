using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Service.Services.ExampleService;

namespace ToDoApp.Service.Tests
{
    public class ExampleServiceTests
    {
        [Fact]
        public async Task CreateCourseWithCurrentTime_ShouldReturnCorrectMessage()
        {
            // Arrange
            var exampleService = new ExampleService();

            var courseName = "Test Course Name";

            var currentTime = DateTime.Now;

            // Act
            var result = exampleService.CreateCourseWithCurrentTime(courseName);

            // Assert
            Assert.Equal($"Course '{courseName}' created at {currentTime:yyyy-MM-dd HH:mm:ss:fffffff}", result);
        }
    }
}
