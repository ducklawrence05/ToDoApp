using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;
using ToDoApp.DataAccess.Entities;
using ToDoApp.DataAccess.Repositories;
using ToDoApp.Infrastructures;
using ToDoApp.Service.MapperProfiles;
using AutoMapper;
using ToDoApp.Service.Services;

namespace ToDoApp.IntergrationTests
{
    public class StudentServiceTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly StudentService _studentService;

        public StudentServiceTests(TestDatabaseFixture fixture)
        {
            _dbContext = fixture.CreateContext();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TodoProfile());
            });

            _studentService = new StudentService
            (
                _dbContext, 
                mapperConfig.CreateMapper(), 
                new StudentRepository(_dbContext),
                new SchoolRepository(_dbContext)
            );

            // ko commit nên sẽ tự rollback lại hết
            _dbContext.Database.BeginTransaction();
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
        public async Task GetStudents_SearchWithFirstName_ReturnsStudent()
        {
            var students = SetupStudents();

            var searchProperty = "FirstName";
            var searchValue = "John";

            var result = await _studentService.GetStudentsAsync(
                searchProperty, searchValue,
                null, true, 1, 1);

            Assert.NotNull(result);

            Assert.Collection(result,
                student => Assert.Contains("John", student.FullName));
        }

        [Fact]
        public async Task GetStudents_SearchWithSchoolName_ReturnsBothStudents()
        {
            var students = SetupStudents();

            var searchProperty = "School.Name";
            var searchValue = "Test";

            var result = await _studentService.GetStudentsAsync(
                searchProperty, searchValue,
                null, true, 1, 10);

            Assert.NotNull(result);

            Assert.Collection(result,
                student => Assert.Contains("TestSchool", student.SchoolName),
                student => Assert.Contains("TestSchool", student.SchoolName)
                );
        }
    }
}
