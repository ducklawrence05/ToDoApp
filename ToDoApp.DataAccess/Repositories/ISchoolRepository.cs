using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.DataAccess.Entities;

namespace ToDoApp.DataAccess.Repositories
{
    public interface ISchoolRepository : IGenericRepository<School>
    {
        Task<School?> GetSchoolByNameAsync(string schoolName);
    }
}
