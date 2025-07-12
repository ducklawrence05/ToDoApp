namespace ToDoApp.DataAccess.Entities
{
    public class ExamQuestion
    {
        public int Id { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public int QuestionBankId { get; set; }
        public QuestionBank QuestionBank { get; set; }

        public ICollection<StudentExamAnswer> StudentExamAnswers { get; set; }
    }
}
