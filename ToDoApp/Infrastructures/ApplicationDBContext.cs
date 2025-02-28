using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;
using ToDoApp.Domains.Entities;

namespace ToDoApp.Infrastructures
{
    public class ApplicationDBContext : DbContext, IApplicationDBContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<School> School { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=MSI\\SQLEXPRESS;Database=ToDo;Trusted_Connection=True;TrustServerCertificate=True");
        }

        public int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
