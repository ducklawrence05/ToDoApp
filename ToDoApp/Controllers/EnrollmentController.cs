using Microsoft.AspNetCore.Mvc;
using ToDoApp.ActionFilters;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Dtos.EnrollmentModel;
using ToDoApp.Application.Services;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _courseStudentService;
        private readonly ILogger<CourseController> _logger;

        public EnrollmentController(IEnrollmentService courseStudentService, ILogger<CourseController> logger)
        {
            _courseStudentService = courseStudentService;
            _logger = logger;
        }

        [TypeFilter(typeof(CacheFilter), Arguments = [5])]
        [HttpGet("{id}")]
        public CourseStudentViewModel GetCourseDetail(int id)
        {
            _logger.LogInformation("Get Course id: " + id);
            if (id == 10)
            {
                _logger.LogWarning("Warning: " + id);
            }
            if (id <= 0)
            {
                _logger.LogError("Id can't be less than or equal to 0");
                throw new Exception("Course id can't be less than or equal to 0");
            }
            return _courseStudentService.GetCourseDetail(id);
        }

        [HttpPost]
        public EnrollmentViewModel PostEnrollment(EnrollmentCreateModel enrollment)
        {
            return _courseStudentService.PostEnrollment(enrollment);
        }

        [HttpPut]
        public EnrollmentViewModel PutEnrollment(EnrollmentCreateModel enrollment)
        {
            return _courseStudentService.PutEnrollment(enrollment);
        }
    }
}
