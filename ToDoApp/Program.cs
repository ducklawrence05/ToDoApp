using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using ToDoApp.Application.ActionFilters;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.MapperProfiles;
using ToDoApp.Application.Middlewares;
using ToDoApp.Application.Services;
using ToDoApp.Application.Services.CacheService;
using ToDoApp.Infrastructures;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Domains.AppSettingsConfigurations;
using Microsoft.OpenApi.Models;
using ToDoApp.Infrastructures.Extentions;
using OfficeOpenXml;
using Hangfire;
using ToDoApp.Application.BackgroundJobs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(option =>
{
    option.Filters.Add<TestFilter>();
    //option.Filters.Add<TokenBlackListFilter>();
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDoApp",
        Version = "v1",
        Description = "ToDoApp API"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    option.AddSecurityDefinition("Bearer", securityScheme);

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] {} }
    });
});

builder.Services.AddDbContext<IApplicationDBContext, ApplicationDBContext>();
builder.Services.AddServices();
builder.Services.AddMemoryCache();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30); // Session timeout setting
    options.Cookie.HttpOnly = true;                 // ko cho phép js đọc được cookie
    options.Cookie.IsEssential = true;              // tự động lưu cookie vào browser
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var googleAuthSettings = builder.Configuration.GetSection("GoogleAuthentication");
builder.Services.Configure<GoogleAuthSettings>(googleAuthSettings);

var fileInformation = builder.Configuration.GetSection("FileInformation");
builder.Services.Configure<FileInformation>(fileInformation);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"]
        };
    });

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IApplicationDBContext, ApplicationDBContext>(options =>
{
    options.UseSqlServer(connectionString);
});

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//    {
//        options.Cookie.HttpOnly = true;
//        options.ExpireTimeSpan = TimeSpan.FromSeconds(30);
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//        options.SlidingExpiration = true;

//        options.Events = new CookieAuthenticationEvents
//        {
//            OnRedirectToLogin = context =>
//            {
//                context.Response.StatusCode = 401;
//                return Task.CompletedTask;
//            },
//            OnRedirectToAccessDenied = context =>
//            {
//                context.Response.StatusCode = 403;
//                return Task.CompletedTask;
//            }
//        };
//    });

builder.Services.AddAutoMapper(typeof(TodoProfile));

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

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs\\log.txt"), 
        rollingInterval: RollingInterval.Minute)
    .CreateLogger();

builder.Host.UseSerilog();

ExcelPackage.License.SetNonCommercialPersonal("Duck");

builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseExceptionHandler("/Error");

app.UseHttpsRedirection();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseCors(option =>
{
    option.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader();
});

app.Use(async (context, next) =>
{
    Console.WriteLine("Request to middleware 1");

    await next(context);

    Console.WriteLine("Response to middleware 1");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("Request to middleware 2");

    await next(context);

    Console.WriteLine("Response to middleware 2");
});

app.UseMiddleware<LogMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<GenerateSchoolReportJob>("RecurringJob",
    job => job.ExecuteAsync(), Cron.Minutely);

app.Run();