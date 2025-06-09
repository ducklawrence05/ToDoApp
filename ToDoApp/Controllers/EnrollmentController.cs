using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Dtos.EnrollmentModel;
using ToDoApp.Application.Services;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _courseStudentService;

        public EnrollmentController(IEnrollmentService courseStudentService)
        {
            _courseStudentService = courseStudentService;
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
