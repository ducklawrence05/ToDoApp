using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.DataAccess.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.IntergrationTests
{
    public class StudentControllerTests : IClassFixture<CustomWebApplicationFactory>, 
        IClassFixture<TestDatabaseFixture>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly ApplicationDBContext _dbContext;

        public StudentControllerTests(CustomWebApplicationFactory factory,
            TestDatabaseFixture fixture)
        {
            _factory = factory;
            _dbContext = fixture.CreateContext();
        }

        private List<Student> SetupStudents()
        {
            var school = new School()
            {
                Name = "TestSchool",
                Address = "123"
            };

            var student = new Student()
            {
                FirstName = "John",
                LastName = "Doe",
                School = school,
                Address1 = "123"
            };

            var student2 = new Student()
            {
                FirstName = "Jane",
                LastName = "Doe",
                School = school,
                Address1 = "123"
            };

            _dbContext.Students.AddRange(student, student2);
            _dbContext.SaveChanges();

            _dbContext.ChangeTracker.Clear();

            return [student, student2];
        }

        [Fact]
        public async Task GetAllStudentsAsync_ReturnsStudents()
        {
            var students = SetupStudents();

            var client = _factory.CreateClient();

            var response = await client.GetFromJsonAsync<IEnumerable<StudentViewModel>>($"/Student/all");
              
            Assert.NotNull( response );
            Assert.Collection(response,
                    student => Assert.Contains(students[0].FirstName, student.FullName),
                    student => Assert.Contains(students[1].FirstName, student.FullName)
                );
        }
    }
}
