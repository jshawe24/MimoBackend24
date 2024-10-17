using MimoBackend_Oct24.API.Models;

namespace MimoBackend_Oct24.API.Repositories.Interfaces;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<Lesson> GetLessonWithDetailsAsync(int lessonId);
}