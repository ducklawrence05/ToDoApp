using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Dtos.QuestionBankModel;
using ToDoApp.Application.Services;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionBankController : ControllerBase
    {
        private readonly IQuestionBankService _questionBankService;

        public QuestionBankController(IQuestionBankService questionBankService)
        {
            _questionBankService = questionBankService;
        }

        [HttpGet]
        public IEnumerable<QuestionBankViewModel> GetQuestions(int? courseId)
        {
            return _questionBankService.GetQuestions(courseId);
        }

        [HttpPost]
        public QuestionBankViewModel PostQuestion(QuestionBankCreateModel question)
        {
            return _questionBankService.PostQuestion(question);
        }

        [HttpPut]
        public QuestionBankViewModel PutQuestion(QuestionBankUpdateModel question)
        {
            return _questionBankService.PutQuestion(question);
        }

        [HttpDelete]
        public void DeleteQuestion(int questionBankId)
        {
            _questionBankService.DeleteQuestion(questionBankId);
        }
    }
}
