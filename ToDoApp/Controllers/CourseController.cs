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
        
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourse()
        {
            return Ok(await _courseService.GetCourses());
        }

        [HttpPost]
        public async Task<int> PostCourse(CourseCreateModel course)
        {
            return await _courseService.PostCourse(course);
        }

        [HttpPut]
        public async Task<int> PutCourse(CourseUpdateModel course)
        {
            return await _courseService.PutCourse(course);
        }

        [HttpDelete]
        public async Task<int> DeleteCourse(int courseId)
        {
            return await _courseService.DeleteCourse(courseId);
        }
    }
}
