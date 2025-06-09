using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ToDoApp.Domain.Entities;
using ToDoApp.Domains.Entities;

namespace ToDoApp.Infrastructures
{
    public interface IApplicationDBContext
    {
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<QuestionBank> QuestionBanks { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<StudentExamAnswer> StudentExamAnswers { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public EntityEntry<T> Entry<T>(T entity) where T : class;
        public int SaveChanges();
        public Task<int> SaveChangesAsync();
    }
}
