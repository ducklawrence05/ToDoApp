using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToDoApp.Infrastructures;

namespace ToDoApp.IntergrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(serviceCollection =>
            {
                var optionType = serviceCollection.FirstOrDefault(s =>
                        s.ServiceType == typeof
                        (DbContextOptions<ApplicationDBContext>));

                if (optionType != null)
                {
                    serviceCollection.Remove(optionType);
                }

                var dbContextDescriptor = serviceCollection.FirstOrDefault(s =>
                    s.ServiceType == typeof(ApplicationDBContext));

                if (dbContextDescriptor != null)
                {
                    serviceCollection.Remove(dbContextDescriptor);
                }
                  
                serviceCollection.AddDbContext<ApplicationDBContext>(options =>
                {
                    options.UseSqlServer("Server=MSI\\SQLEXPRESS;Database=IntergrationTestDb;Trusted_Connection=True;TrustServerCertificate=True");
                });
            });
        }
    }
}
