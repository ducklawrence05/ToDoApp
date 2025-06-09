using ToDoApp.Application.Dtos;
using ToDoApp.Application.Middlewares;
using ToDoApp.Application.Services.CacheService;
using ToDoApp.Application.Services;
using ToDoApp.Application.Services.GoogleCredentialService;

namespace ToDoApp.Infrastructures.Extentions
{
    public static class AddDependencyInjection
    {
        public static void AddServices(this IServiceCollection serviceCollection)
        {
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
