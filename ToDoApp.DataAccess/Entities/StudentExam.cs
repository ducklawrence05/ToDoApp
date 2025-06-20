using ToDoApp.Domains.Interface;

namespace ToDoApp.Domains.Entities
{
    public class StudentExam : ICreatedAt
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public double Score { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<StudentExamAnswer> Answers { get; set; } = [];
    }
}
