using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.ActionFilters;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Services;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(LogFilter), Arguments = [LogLevel.Warning])]
    [AuditFilter]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(ICourseService courseService, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [TypeFilter(typeof(CacheFilter), Arguments = [5])]
        [HttpGet("{id}")]
        public CourseStudentViewModel GetCourseDetail(int id)
        {
            _logger.LogInformation("Get Course id: " + id);
            if(id == 10)
            {
                _logger.LogWarning("Warning: " + id);
            }
            if(id <= 0)
            {
                _logger.LogError("Id can't be less than or equal to 0");
                throw new Exception("Course id can't be less than or equal to 0");
            }
            return _courseService.GetCourseDetail(id);
        }

        [HttpGet]
        public IEnumerable<CourseViewModel> GetCourse()
        {
            return _courseService.GetCourses();
        }

        [HttpPost]
        public async Task<CourseViewModel> PostCourse(CourseCreateModel course)
        {
            return await _courseService.PostCourse(course);
        }

        [HttpPut]
        public CourseViewModel PutCourse(CourseUpdateModel course)
        {
            return _courseService.PutCourse(course);
        }

        [HttpDelete]
        public void DeleteCourse(int courseId)
        {
            _courseService.DeleteCourse(courseId);
        }
    }
}
