using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Dtos.StudentExamAnswerModel;
using ToDoApp.Application.Dtos.StudentExamModel;
using ToDoApp.Application.Services;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentExamController : ControllerBase
    {
        private readonly IStudentExamService _studentExamService;

        public StudentExamController(IStudentExamService studentExamService)
        {
            _studentExamService = studentExamService;
        }

        [HttpPost]
        public StudentExamViewModel PostStudentExam(StudentExamCreateModel model)
        {
            return _studentExamService.PostStudentExam(model);
        }

        [HttpPost("{studentExamId}")]
        public StudentExamViewModel PostStudentExamAnswers(int studentExamId, IEnumerable<StudentExamAnswerCreateModel> studentExamAnswers)
        {
            return _studentExamService.PostStudentExamAnswers(studentExamId, studentExamAnswers);
        }
    }
}
