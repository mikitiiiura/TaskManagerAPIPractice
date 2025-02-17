using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Додаємо MediatR
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(Program)));

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

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.WithOrigins("http://localhost:5173/");
//        policy.AllowAnyHeader();
//        policy.AllowAnyMethod();
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173") // Дозволяє твоєму React-додатку робити запити Важливо без /
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()); // Якщо використовуєш аутентифікацію через cookies
});

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GetAllTasksHandler).Assembly));

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
