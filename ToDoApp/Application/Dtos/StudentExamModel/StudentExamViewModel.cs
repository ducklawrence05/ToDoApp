using ToDoApp.Application.Dtos.StudentExamAnswerModel;

namespace ToDoApp.Application.Dtos.StudentExamModel
{
    public class StudentExamViewModel
    {
        public int StudentExamId { get; set; }
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public double Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<StudentExamAnswerViewModel> StudentExamAnswers { get; set; }
    }
}
