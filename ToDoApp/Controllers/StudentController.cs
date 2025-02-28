using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Services;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public IEnumerable<StudentViewModel> GetStudents(int? schoolId)
        {
            return _studentService.GetStudents(schoolId);
        }

        [HttpPost]
        public int PostStudent(StudentCreateModel student)
        {
            return _studentService.PostStudent(student);
        }

        [HttpPut]
        public int PutStudent(StudentUpdateModel student)
        {
            return _studentService.PutStudent(student);
        }

        [HttpDelete]
        public void DeleteStudent(int studentId)
        {
            _studentService.DeleteStudent(studentId);
        }
    }
}
