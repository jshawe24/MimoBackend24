using MimoBackend_Oct24.API.Models;

namespace MimoBackend_Oct24.API.Repositories.Interfaces
{
    public interface IChapterRepository : IRepository<Chapter>
    {
        Task<List<int>> GetChapterIdsForCourseAsync(int courseId);
        
        Task<bool> IsChapterCompletedByUserAsync(int chapterId, int userId);
        Task<List<Chapter>> GetAllWithLessonsAsync();
    }
}
