namespace ToDoApp.Application.Dtos.StudentExamAnswerModel
{
    public class StudentExamAnswerViewModel
    {
        public int ExamQuestionId { get; set; }
        public int QuestionBankId { get; set; }
        public string QuestionText { get; set; }
        public string SelectedAnswer { get; set; }
    }
}
