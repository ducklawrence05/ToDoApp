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
        public void CreateCourseWithCurrentTime_ShouldReturnCorrectMessage()
        {
            // Arrange
            var courseName = "Test Course Name";
            var fixedTime = new DateTime(2025, 7, 19, 20, 0, 0); // Giả lập thời gian cố định

            var service = new TestableExampleService(fixedTime);

            var expectedMessage = $"Course '{courseName}' created at {fixedTime:yyyy-MM-dd HH:mm:ss:fffffff}";

            // Act
            var result = service.CreateCourseWithCurrentTime(courseName);

            // Assert
            Assert.Equal(expectedMessage, result);
        }
    }
}
