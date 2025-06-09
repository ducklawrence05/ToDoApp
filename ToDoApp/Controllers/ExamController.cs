using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Dtos.ExamModel;
using ToDoApp.Application.Dtos.ExamQuestionModel;
using ToDoApp.Application.Services;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpPost]
        public ExamViewModel PostExam(ExamCreateModel model)
        {
            return _examService.PostExam(model);
        }

        [HttpPost("{examId}")]
        public ExamViewModel PostExamQuestions(int examId, IEnumerable<ExamQuestionCreateModel> questionBankIds)
        {
            return _examService.PostExamQuestions(examId, questionBankIds);
        }

        [HttpPut("{examId}")]
        public ExamViewModel PutExamQuestions(int examId, IEnumerable<ExamQuestionCreateModel> questionBankIds)
        {
            return _examService.PutExamQuestions(examId, questionBankIds);
        }
    }
}
