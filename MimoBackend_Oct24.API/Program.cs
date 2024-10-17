using Microsoft.EntityFrameworkCore;
using MimoBackend_Oct24.API.Services;
using MimoBackend_Oct24.API.Data;
using MimoBackend_Oct24.API.Repositories;
using MimoBackend_Oct24.API.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers to the service collection
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Register repositories and services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();

builder.Services.AddScoped<ILessonProgressService, LessonProgressService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IChapterCompletionService, ChapterCompletionService>();
builder.Services.AddScoped<ICourseCompletionService, CourseCompletionService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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