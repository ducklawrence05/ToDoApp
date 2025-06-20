namespace ToDoApp.Domains.Entities
{
    public class StudentExamAnswer
    {
        public int StudentExamId { get; set; }
        public StudentExam StudentExam { get; set; }

        public int ExamQuestionId { get; set; }
        public ExamQuestion ExamQuestion { get; set; }

        public string SelectedAnswer { get; set; }
    }
}
