using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;
using ToDoApp.Domains.Entities;

namespace ToDoApp.Infrastructures
{
    public interface IApplicationDBContext
    {
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<School> School { get; set; }
        public int SaveChanges();
    }
}
