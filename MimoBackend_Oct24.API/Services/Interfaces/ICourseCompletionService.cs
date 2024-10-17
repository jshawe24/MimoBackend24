namespace MimoBackend_Oct24.API.Services;

public interface ICourseCompletionService
{
    Task<int> CheckCompletedCourseByLessons(int userId, int courseId);
}