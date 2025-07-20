using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Infrastructures;

namespace ToDoApp.IntergrationTests
{
    public class TestDatabaseFixture
    {
        private static object _lock = new object();
        private static bool _isDatabaseCreated = false;
        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_isDatabaseCreated)
                {
                    using ApplicationDBContext context = CreateContext();

                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    _isDatabaseCreated = true;
                }
            }
        }

        public ApplicationDBContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                            .UseSqlServer("Server=MSI\\SQLEXPRESS;Database=IntergrationTestDb;Trusted_Connection=True;TrustServerCertificate=True")
                            .Options;

            var context = new ApplicationDBContext(options);
            return context;
        }
    }
}
