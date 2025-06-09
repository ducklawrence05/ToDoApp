namespace ToDoApp.Domains.Entities
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public ICollection<ExamQuestion> ExamQuestions { get; set; }
        public ICollection<StudentExam> StudentExams { get; set; }
    }
}
