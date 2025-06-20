namespace ToDoApp.Application.Dtos.ExamQuestionModel
{
    public class ExamQuestionViewModel
    {
        public int QuestionBankId { get; set; }
        public string QuestionText { get; set; }

        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }

        public string CorrectAnswer { get; set; }
    }
}
