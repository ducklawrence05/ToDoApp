using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.DataAccess.Entities;

namespace ToDoApp.DataAccess.Repositories
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
    }
}
