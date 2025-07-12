using AutoMapper;
using Moq;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Services;
using ToDoApp.DataAccess.Entities;
using ToDoApp.DataAccess.Repositories;

namespace ToDoApp.Service.Tests
{
    public class CourseServiceTests
    {
        private Mock<ICourseRepository> _courseRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private CourseService _courseService;

        public CourseServiceTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _mapperMock = new Mock<IMapper>();
            _courseService = new CourseService(_mapperMock.Object, _courseRepositoryMock.Object);
        }

        // naming convention: methodName_condition_expectedBehavior
        [Fact]
        public async Task PostCourseAsync_WithNonExistentCourse_ReturnsCreatedIdAsync()
        {
            // Arrange
            var inputCourse = new CourseCreateModel
            {
                CourseName = "New Course",
                StartDate = DateTime.Now,
            };

            var expectedCourse = new Course
            {
                Id = 999,
                Name = inputCourse.CourseName,
                StartDate = inputCourse.StartDate,
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Course?) null);
            // or .ReturnsAsync(() => null);

            _mapperMock.Setup(mapper => mapper.Map<Course>(inputCourse))
                .Returns(expectedCourse);

            _courseRepositoryMock.Setup(repo => repo.AddAsync(expectedCourse))
                .ReturnsAsync(expectedCourse.Id);

            // Act
            var result = await _courseService.PostCourseAsync(inputCourse);

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task PostCourseAsync_WithExistentCourse_ThrowsInvalidOperationException()
        {
            const string courseName = "Existing Course";
            // Arrange
            var inputCourse = new CourseCreateModel
            {
                CourseName = courseName,
                StartDate = DateTime.Now,
            };

            var existingCourse = new Course
            {
                Id = 999,
                Name = courseName,
                StartDate = inputCourse.StartDate,
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseByNameAsync(courseName))
                .ReturnsAsync(existingCourse);

            _mapperMock.Setup(mapper => mapper.Map<Course>(inputCourse))
                .Returns(existingCourse);

            _courseRepositoryMock.Setup(repo => repo.AddAsync(existingCourse))
                .ReturnsAsync(existingCourse.Id);

            // Act
            var result = async () => await _courseService.PostCourseAsync(inputCourse);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(result);

            Assert.Equal("Course name is existed", exception.Message);

            _mapperMock.Verify(mapper => mapper.Map<Course>(inputCourse), Times.Never);

            _courseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task PutCourseAsync_WithValidInput_ReturnsUpdatedCourseId()
        {
            // Arrange
            var inputCourse = new CourseUpdateModel
            {
                CourseId = 999,
                CourseName = "Existing Course",
                StartDate = DateTime.Now,
            };

            var expectedCourse = new Course
            {
                Id = 999,
                Name = "Updated Name",
                StartDate = (DateTime) inputCourse.StartDate,
                DeletedBy = null
            };

            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(inputCourse.CourseId))
                .ReturnsAsync(expectedCourse);

            _mapperMock.Setup(mapper => mapper.Map(inputCourse, expectedCourse))
                .Verifiable();

            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(expectedCourse))
                .Verifiable();

            // Act
            var id = await _courseService.PutCourseAsync(inputCourse);

            // Assert
            Assert.Equal(expectedCourse.Id, id);

            _mapperMock.Verify(mapper => mapper.Map(inputCourse, expectedCourse), Times.Once);

            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(expectedCourse), Times.Once);
        }

        [Theory]
        [MemberData(nameof(NonExistentCourses))]
        public async Task PutCourseAsync_WithNonExistentOrDeletedCourse_ThrowsInvalidOperationException(Course? courseFromDb)
        {
            // Arrange
            var inputCourse = new CourseUpdateModel
            {
                CourseId = 999,
                CourseName = "NonExistent Course",
                StartDate = DateTime.Now,
            };

            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(inputCourse.CourseId))
                .ReturnsAsync(courseFromDb);

            _mapperMock.Setup(mapper => mapper.Map(inputCourse, courseFromDb));

            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(courseFromDb!));

            // Act, Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _courseService.PutCourseAsync(inputCourse));

            Assert.Equal("Course not found or has been deleted.", exception.Message);

            _mapperMock.Verify(mapper => mapper.Map(inputCourse, courseFromDb), Times.Never);

            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(courseFromDb!), Times.Never);
        }

        public static IEnumerable<object[]> NonExistentCourses = new List<object[]>
        {
            new object[] { null },
            new object[] 
            { 
                new Course 
                { 
                    Id = 999, 
                    Name = "NonExistent Course", 
                    DeletedBy = 1,
                } 
            }
        };

        [Fact]
        public async Task DeleteCourseAsync_WithExistentCourse_ReturnsDeletedCourseId()
        {
            // Arrange
            var existingCourseId = 999;
            var existingCourse = new Course
            {
                Id = existingCourseId,
                Name = "Existing Course",
                StartDate = DateTime.Now,
            };

            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(existingCourseId))
                .ReturnsAsync(existingCourse);

            _courseRepositoryMock.Setup(repo => repo.DeleteAsync(existingCourseId))
                .ReturnsAsync(existingCourseId);

            // Act
            var result = await _courseService.DeleteCourseAsync(existingCourseId);

            // Assert
            Assert.Equal(existingCourseId, result);
        }
    }
}