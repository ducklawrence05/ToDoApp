using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Dtos.ToDoModel;
using ToDoApp.DataAccess.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface IToDoService
    {
        int Post(ToDoCreateModel todo);

        Guid Generate();
    }

    public class ToDoService : IToDoService
    {
        private readonly IApplicationDBContext _dbContext;
        private readonly IGuidGenerator _guidGenerator;

        public ToDoService(IApplicationDBContext dbContext, IGuidGenerator guidGenerator)
        {
            _dbContext = dbContext;
            _guidGenerator = guidGenerator;
        }

        public Guid Generate()
        {
            return _guidGenerator.Generate();
        }

        public int Post(ToDoCreateModel todo)
        {
            var data = new ToDo
            {
                Description = todo.Description
            };
            _dbContext.ToDos.Add(data);

            _dbContext.SaveChanges();

            return data.Id;
        }

    }
}
