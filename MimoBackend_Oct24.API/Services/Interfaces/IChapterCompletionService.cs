namespace MimoBackend_Oct24.API.Services;

public interface IChapterCompletionService
{
    Task<List<int>> GetCompletedChapterIdsForUser(int userId);
}