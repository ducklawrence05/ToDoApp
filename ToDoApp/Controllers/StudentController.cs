using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.Service.Services;

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

        [HttpGet]
        public async Task<IEnumerable<StudentViewModel>> GetStudentsAsync(
            string? searchProperty, string? searchValue,
            string? sortBy, bool isAscending,
            int pageIndex, int pageSize
        )
        {
            return await _studentService.GetStudentsAsync(searchProperty, searchValue, sortBy, isAscending, pageIndex, pageSize);
        }

        [HttpGet("all")]
        public async Task<IEnumerable<StudentViewModel>> GetAllStudentsAsync(){
            return await _studentService.GetAllStudentsAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostStudent([FromBody] StudentCreateModel student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _studentService.PostAsync(student);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> PutStudent([FromBody] StudentUpdateModel student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _studentService.PutAsync(student);
            return Ok(result);
        }

        [HttpDelete("{studentId}")]
        public async Task<IActionResult> DeleteStudent(int studentId)
        {
            var success = await _studentService.DeleteAsync(studentId);

            if (success == -1)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
