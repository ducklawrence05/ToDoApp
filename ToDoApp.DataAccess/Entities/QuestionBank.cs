namespace ToDoApp.Domains.Entities
{
    public class QuestionBank
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }

        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }

        public string CorrectAnswer { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}
