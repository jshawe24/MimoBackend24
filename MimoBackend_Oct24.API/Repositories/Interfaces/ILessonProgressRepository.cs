using System.Linq.Expressions;
using MimoBackend_Oct24.API.Models;

namespace MimoBackend_Oct24.API.Repositories.Interfaces;

public interface ILessonProgressRepository : IRepository<LessonProgress>
{
    Task<int> CountCompletedLessonsAsync(int userId);
    Task<IEnumerable<LessonProgress>> GetAllAsync(Expression<Func<LessonProgress, bool>> predicate);
}