using System.ComponentModel.DataAnnotations.Schema;
using ToDoApp.DataAccess.Interface;

namespace ToDoApp.DataAccess.Entities
{
    public class Course : IEntity, ICreatedBy, ICreatedAt, IDeletedBy, IDeletedAt
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<CourseStudent> CourseStudents { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public ICollection<QuestionBank> QuestionBanks { get; set; }
    }
}
