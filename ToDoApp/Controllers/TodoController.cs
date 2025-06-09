using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Dtos.ToDoModel;
using ToDoApp.Application.Services;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Controllers
{
    // GET, POST, PUT, DELETE
    // Idempotency: GET, PUT, DELETE
    // Non-Idempotency: POST

    // Example
    // POST request: create 1 row Todo 
    // When calling { Description: "Study", IsCompleted: false } 10 times => Create 10 rows in database

    // PUT request
    // Update 1 row Todo
    // When calling { Id = 1, Description: "Study", IsCompleted: false } 10 times

    // Client ---> Server ---> Database: OK; Server ---> Client: OK

    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly IApplicationDBContext _dbContext;
        private readonly IToDoService _todoService;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ISingletonGenerator _singletonGenerator;
        private readonly GuidData _guidData;

        public TodoController(
            IApplicationDBContext dbContext, 
            IToDoService todoService,
            IGuidGenerator guidGenerator,
            ISingletonGenerator singletonGenerator,
            GuidData guidData)
        { 
            _dbContext = dbContext;
            _todoService = todoService;
            _guidGenerator = guidGenerator;
            _singletonGenerator = singletonGenerator;
            _guidData = guidData;
        }

        [HttpGet("guid")]
        public Guid[] GetGuid()
        {
            _guidData.guidGenerator = _guidGenerator;

            return
                new Guid[]
                {
                    _guidData.GetGuid(),
                    _singletonGenerator.Generate()
                };
        }

        [HttpGet]
        public IEnumerable<ToDoViewModel> Get(bool isCompleted) 
        {
            var data = _dbContext.ToDos
                .Where(x => x.IsCompleted == isCompleted)
                .Select(x => 
                    new ToDoViewModel
                    {
                        Description = x.Description,
                        IsCompleted = x.IsCompleted,
                    })
                .ToList();
            return data;

            //SELECT * FROM ToDos WHERE IsCompleted = isCompleted
            //return _dbContext.ToDos
            //    .Where(x => x.IsCompleted == isCompleted)
            //    .ToList();
        }

        [HttpPost]
        public int Post(ToDoCreateModel todo)
        {
            return _todoService.Post(todo);
        }

        [HttpPut]
        public int Put(ToDoUpdateModel todo)
        {
            var data = _dbContext.ToDos.Find(todo.Id);
            if (data == null) return -1;

            data.Description = todo.Description;
            data.IsCompleted = todo.IsCompleted;

            _dbContext.SaveChanges();

            return todo.Id;
        }

        [HttpDelete]
        public void Delete(int id)
        {
            var data = _dbContext.ToDos.Find(id);
            if (data == null) return;

            _dbContext.ToDos.Remove(data);
            _dbContext.SaveChanges();
        }
    }
}
