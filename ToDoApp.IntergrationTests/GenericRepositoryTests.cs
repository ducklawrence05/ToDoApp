using Microsoft.EntityFrameworkCore;
using ToDoApp.DataAccess.Entities;
using ToDoApp.DataAccess.Repositories;
using ToDoApp.Infrastructures;

namespace ToDoApp.IntergrationTests
{
    public class GenericRepositoryTests : IClassFixture<TestDatabaseFixture>
    {
        private ApplicationDBContext _dbContext;
        private GenericRepository<Student> _repository;

        public GenericRepositoryTests(TestDatabaseFixture fixture)
        {
            _dbContext = fixture.CreateContext();

            _repository = new GenericRepository<Student>
                (_dbContext);

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
        public async Task GetAllAsync_WithNoInclude_ReturnsAllWithNoIncludeEntity()
        {
            // Arrange
            var students = SetupStudents();

            // Act
            var result = await _repository.GetAllAsync(null);

            // Assert
            Assert.NotNull(result);
            Assert.Collection
            (
                result,
                student => Assert.Equal("John", student.FirstName),
                student => Assert.Equal("Jane", student.FirstName)
            );
            Assert.All(result, student => Assert.Null(student.School));
        }

        [Fact]
        public async Task GetAllAsync_WithInclude_ReturnsAllWithIncludeEntity()
        {
            // Arrange
            var students = SetupStudents();

            // Act
            var result = await _repository.GetAllAsync(s => s.School);

            // Assert
            Assert.NotNull(result);
            Assert.All(students, student => Assert.NotNull(student.School));
        }

        [Fact]
        public async Task GetById_WithExistingId_ReturnsEntity()
        {
            // Arrange
            var students = SetupStudents();
             
            // Act
            var student = await _repository.GetByIdAsync(students.First().Id);

            // Assert
            Assert.NotNull(student);
            Assert.Equal("John", student.FirstName);
        }

        [Fact]
        public async Task GetById_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            var students = SetupStudents();

            // Act
            var student = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(student);
        }

        [Fact]
        public async Task AddAsync_ReturnsCreatedEntityId()
        {
            var id = await _repository.AddAsync(new Student
            {
                FirstName = "Alice",
                Address1 = "456",
                School = new School
                {
                    Name = "New School",
                    Address = "456"
                }
            });

            Assert.True(id > 0);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingEntity_UpdatesEntity()
        {
            var students = SetupStudents();

            var studentToUpdate = students.First();

            studentToUpdate.FirstName = "Updated Name";

            var updatedId = await _repository.UpdateAsync(studentToUpdate);

            Assert.Equal(studentToUpdate.Id, updatedId);

            var student = await _dbContext.Students
                .FirstOrDefaultAsync(s => s.Id 
                == studentToUpdate.Id);

            Assert.NotNull(student);
            
            Assert.Equal("Updated Name", student.FirstName);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingEntity_ThrowsException()
        {
            var studentToUpdate = new Student()
            {
                FirstName = "Updated Name",
                Address1 = "456",
                School = new School()
                {
                    Name = "New School",
                    Address = "123"
                }
            };

            var result = await _dbContext.Students.FirstOrDefaultAsync(s => 
                s.FirstName == "Updated Name");

            Assert.Null(result);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _repository.UpdateAsync(studentToUpdate));

            Assert.Equal("Student not found", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingEntity_DeletesEntity()
        {
            // Arrange
            var students = SetupStudents();
            var existingStudent = students.First();

            var studentInDb = await _dbContext.Students.FindAsync(existingStudent.Id);
            Assert.NotNull(studentInDb);

            // Act
            var deletedId = await _repository.DeleteAsync(existingStudent.Id);

            // Assert
            Assert.True(deletedId > 0);

            var result = await _dbContext.Students.FindAsync(existingStudent.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingEntity_ThrowsException()
        {
            // Arrange
            int nonExistingId = 99999;

            var result = await _dbContext.Students
                .FirstOrDefaultAsync(s => s.Id == nonExistingId);

            Assert.Null(result);

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _repository.DeleteAsync(nonExistingId));

            // Assert
            Assert.Equal("Student not found", ex.Message);
        }
    }
}
