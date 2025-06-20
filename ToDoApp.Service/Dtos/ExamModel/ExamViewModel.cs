using ToDoApp.Application.Dtos.ExamQuestionModel;

namespace ToDoApp.Application.Dtos.ExamModel
{
    public class ExamViewModel
    {
        public int ExamId { get; set; }
        public string Title { get; set; }
        public int CourseId { get; set; }
        public IEnumerable<ExamQuestionViewModel> ExamQuestions { get; set; }
    }
}
