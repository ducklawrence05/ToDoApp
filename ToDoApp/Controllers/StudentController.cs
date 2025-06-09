using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Dtos.StudentModel;
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

        [HttpGet("{id}/scores")]
        public StudentCourseViewModel GetStudentDetail(int id)
        {
            return _studentService.GetStudentDetail(id);
        }

        [HttpGet("{id}/average")]
        public StudentAverageScoreViewModel GetStudentAverageScore(int id)
        {
            return _studentService.GetStudentAverageScore(id);
        }

        //[HttpGet]
        //public IEnumerable<StudentViewModel> GetStudents(
        //    string? searchProperty, string? searchValue,
        //    string? sortBy, bool isAscending,
        //    int pageIndex, int pageSize
        //)
        //{
        //    return _studentService.GetStudents(searchProperty, searchValue, sortBy, isAscending, pageIndex, pageSize);
        //}

        [HttpGet]
        public IEnumerable<StudentViewModel> GetStudents(){
            return _studentService.GetStudents();
        }

        [HttpPost]
        public IActionResult PostStudent(StudentCreateModel student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_studentService.PostStudent(student));
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
