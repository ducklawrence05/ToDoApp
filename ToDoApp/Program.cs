using ToDoApp.Application.Dtos;
using ToDoApp.Application.Services;
using ToDoApp.Infrastructures;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IApplicationDBContext, ApplicationDBContext>();
builder.Services.AddScoped<IToDoService, ToDoService>();
builder.Services.AddTransient<IGuidGenerator, GuidGenerator>();
//builder.Services.AddSingleton<IGuidGenerator, GuidGenerator>();
//builder.Services.AddScoped<IGuidGenerator, GuidGenerator>();
builder.Services.AddSingleton<ISingletonGenerator, SingletonGenerator>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddTransient<GuidData>();
/* Các loại add services
 * AddTransient: inject là tạo mới (life time ngắn nhất)
 * AddScoped: mỗi request là tạo mới
 * AddSingleton: mỗi start app là tạo mới (life time dài nhất)
 * Bonus: AddDbContext là AddScoped
 * 
 * Thằng có life time ngắn hơn ko bỏ vào thằng dài hơn
 *  do thằng dài nó sẽ giữ của thằng ngắn cho đến hết life time của thằng dài
 */

// DI Containers, IServiceProvider (1 Singleton)

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
