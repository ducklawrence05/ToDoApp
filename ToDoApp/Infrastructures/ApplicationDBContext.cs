using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ToDoApp.Domain.Entities;
using ToDoApp.Domains.Entities;
using ToDoApp.Infrastructures.DatabaseMapping;
using ToDoApp.Infrastructures.Interceptors;

namespace ToDoApp.Infrastructures
{
    public class ApplicationDBContext : DbContext, IApplicationDBContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=MSI\\SQLEXPRESS;Database=ToDo;Trusted_Connection=True;TrustServerCertificate=True");
            //optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.AddInterceptors(
                new SqlQueryLoggingInterceptor(), 
                new AudiLogInterceptor(),
                new AddedAndModifiedInterceptor(),
                new SoftDeleteInterceptor());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");

                entity.Property(x => x.Age)
                    .HasComputedColumnSql("DATEDIFF(YEAR, DateOfBirth, GETDATE())");

                entity.HasMany(student => student.CourseStudents)
                    .WithOne(courseStudent => courseStudent.Student)
                    .HasForeignKey(courseStudent => courseStudent.StudentId);

                entity.HasMany(student => student.StudentExams)
                    .WithOne(studentExam => studentExam.Student)
                    .HasForeignKey(studentExam => studentExam.StudentId);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");

                entity.HasMany(course => course.CourseStudents)
                    .WithOne(courseStudent => courseStudent.Course)
                    .HasForeignKey(courseStudent => courseStudent.CourseId);

                entity.HasMany(course => course.QuestionBanks)
                    .WithOne(questionBank => questionBank.Course)
                    .HasForeignKey(questionBank => questionBank.CourseId);

                entity.HasMany(course => course.Exams)
                    .WithOne(exam => exam.Course)
                    .HasForeignKey(exam => exam.CourseId);
            });

            modelBuilder.Entity<CourseStudent>(entity =>
            {
                entity.ToTable("CourseStudents");

                entity.HasKey(courseStudent => new { courseStudent.CourseId, courseStudent.StudentId });
            });

            modelBuilder.Entity<QuestionBank>(entity =>
            {
                entity.ToTable("QuestionBanks");

                entity.Property(x => x.CorrectAnswer).HasMaxLength(1);

                entity.HasMany(questionBank => questionBank.ExamQuestions)
                    .WithOne(examQuestion => examQuestion.QuestionBank)
                    .HasForeignKey(examQuestion => examQuestion.QuestionBankId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.ToTable("Exams");

                entity.HasMany(exam => exam.ExamQuestions)
                    .WithOne(examQuestion => examQuestion.Exam)
                    .HasForeignKey(examQuestion => examQuestion.ExamId);

                entity.HasMany(exam => exam.StudentExams)
                    .WithOne(studentExam => studentExam.Exam)
                    .HasForeignKey(studentExam => studentExam.ExamId);
            });

            modelBuilder.Entity<ExamQuestion>(entity =>
            {
                entity.ToTable("ExamQuestions");

                entity.HasIndex(e => new { e.ExamId, e.QuestionBankId }).IsUnique();

                entity.HasMany(examQuestion => examQuestion.StudentExamAnswers)
                    .WithOne(studentExamAnswer => studentExamAnswer.ExamQuestion)
                    .HasForeignKey(studentExamAnswer => studentExamAnswer.ExamQuestionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StudentExam>(entity =>
            {
                entity.ToTable("StudentExams");

                entity.HasMany(e => e.Answers)
                    .WithOne(x => x.StudentExam)
                    .HasForeignKey(x => x.StudentExamId);
            });

            modelBuilder.Entity<StudentExamAnswer>(entity =>
            {
                entity.ToTable("StudentExamAnswers");

                entity.HasKey(e => new { e.StudentExamId, e.ExamQuestionId });

                entity.Property(x => x.SelectedAnswer).HasMaxLength(1);
            });

            modelBuilder.Entity<School>().ToTable("Schools");

            modelBuilder.ApplyConfiguration(new CourseMapping());

            modelBuilder.ApplyConfiguration(new UserMapping());

            modelBuilder.Entity<RefreshToken>()
                .ToTable("RefreshToken");

            base.OnModelCreating(modelBuilder);
        }

        public int SaveChanges()
        {
            //var auditLogs = new List<AuditLog>();
            //foreach(var entity in ChangeTracker.Entries())
            //{
            //    var log = new AuditLog
            //    {
            //        EntityName = entity.Entity.GetType().Name,
            //        CreatedAt = DateTime.Now,
            //        Action = entity.State.ToString(),
            //    };

            //    if(entity.State == EntityState.Added)
            //    {
            //        log.NewValue = JsonSerializer.Serialize(entity.CurrentValues.ToObject());
            //    }
            //    if(entity.State == EntityState.Modified)
            //    {
            //        log.OldValue = JsonSerializer.Serialize(entity.OriginalValues.ToObject());
            //        log.NewValue = JsonSerializer.Serialize(entity.CurrentValues.ToObject());
            //    }
            //    if(entity.State == EntityState.Deleted)
            //    {
            //        log.OldValue = JsonSerializer.Serialize(entity.OriginalValues.ToObject());
            //    }
            //    auditLogs.Add(log);
            //}
            //AuditLog.AddRange(auditLogs); //add 1 lần nhiều cái
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public EntityEntry<T> Entry<T>(T entity) where T : class
        {
            return base.Entry(entity);
        }
    }
}
