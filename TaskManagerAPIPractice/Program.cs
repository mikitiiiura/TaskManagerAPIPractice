using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text.Json.Serialization;
using TaskManagerAPIPractice.Application.Handlers.TasksHandler;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.DataAccess;
using TaskManagerAPIPractice.DataAccess.abstruct;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.Repositories;
using TaskManagerAPIPractice.Extensions;
using TaskManagerAPIPractice.Persistence;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//// Налаштування Serilog
//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    //.MinimumLevel.Information() // Встановіть мінімальний рівень логування
//    .MinimumLevel.Error()
//    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // Логи в файл
//    .CreateLogger();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Записувати лише логи рівня Information і вище
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Приховати логи ASP.NET Core нижче рівня Warning
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning) // Приховати логи EF Core нижче рівня Warning
    .WriteTo.Console()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog(); // Підключення Serilog

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaskAPIDbContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("WebAppDbContext"));     //For MSSQL
    });

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddApiAuthentification(builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>());

builder.Services.AddScoped<INotificationsRepository, NotificationsRepository>();
builder.Services.AddScoped<IProjectsRepository, ProjectsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectsServices, ProjectsServices>();
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173") // Дозволяє твоєму React-додатку робити запити Важливо без /
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()); // Якщо використовуєш аутентифікацію через cookies
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAllTasksHandler>());


builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
