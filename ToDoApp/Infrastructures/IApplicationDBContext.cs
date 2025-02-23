using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Infrastructures
{
    public interface IApplicationDBContext
    {
        public DbSet<ToDo> ToDos { get; set; }

        public int SaveChanges();
    }
}
