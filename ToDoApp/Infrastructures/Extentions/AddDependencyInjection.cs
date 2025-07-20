using ToDoApp.Application.Dtos;
using ToDoApp.Application.Services.CacheService;
using ToDoApp.Application.Services;
using ToDoApp.Application.Services.GoogleCredentialService;
using ToDoApp.DataAccess.Repositories;
using ToDoApp.DataAccess.Entities;
using Microsoft.Extensions.Caching.Memory;
using ToDoApp.Middlewares;
using ToDoApp.Service.Services;

namespace ToDoApp.Infrastructures.Extentions
{
    public static class AddDependencyInjection
    {
        public static void AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICourseRepository, CourseRepository>();
            serviceCollection.AddScoped<IStudentRepository, StudentRepository>();
            serviceCollection.AddScoped<ISchoolRepository, SchoolRepository>();
            serviceCollection.AddScoped<IGenericRepository<Student>, GenericRepository<Student>>();

            serviceCollection.AddScoped<IGenericRepository<Student>>(serviceProvider =>
                new CachedRepository<Student>(
                    serviceProvider.GetRequiredService<IGenericRepository<Student>>(),
                    serviceProvider.GetRequiredService<IMemoryCache>()    
                )
            );

            serviceCollection.AddScoped<IToDoService, ToDoService>();
            serviceCollection.AddTransient<IGuidGenerator, GuidGenerator>();
            serviceCollection.AddTransient<GuidData>();
            //serviceCollection.AddSingleton<IGuidGenerator, GuidGenerator>();
            //serviceCollection.AddScoped<IGuidGenerator, GuidGenerator>();
            serviceCollection.AddSingleton<ISingletonGenerator, SingletonGenerator>();
            serviceCollection.AddScoped<IStudentService, StudentService>();
            serviceCollection.AddScoped<ISchoolService, SchoolService>();
            serviceCollection.AddScoped<ICourseService, CourseService>();
            serviceCollection.AddScoped<IEnrollmentService, EnrollmentService>();
            serviceCollection.AddScoped<IQuestionBankService, QuestionBankService>();
            serviceCollection.AddScoped<IExamService, ExamService>();
            serviceCollection.AddScoped<IStudentExamService, StudentExamService>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddSingleton<LogMiddleware>();
            serviceCollection.AddSingleton<RateLimitMiddleware>();
            //serviceCollection.AddSingleton<LogFilter>();
            serviceCollection.AddSingleton<ICacheService, CacheService>();
            serviceCollection.AddSingleton<IGoogleCredentialService, GoogleCredentialService>();
        }
    }
}
