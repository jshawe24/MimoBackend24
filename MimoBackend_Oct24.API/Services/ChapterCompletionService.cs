using MimoBackend_Oct24.API.Repositories.Interfaces;

namespace MimoBackend_Oct24.API.Services;

public class ChapterCompletionService : IChapterCompletionService
{
    private readonly IChapterRepository _chapterRepository;
    private readonly ILessonProgressRepository _lessonProgressRepository;

    public ChapterCompletionService(IChapterRepository chapterRepository, ILessonProgressRepository lessonProgressRepository)
    {
        _chapterRepository = chapterRepository;
        _lessonProgressRepository = lessonProgressRepository;
    }

    public async Task<List<int>> GetCompletedChapterIdsForUser(int userId)
    {
        var allChapters = await _chapterRepository.GetAllWithLessonsAsync();
        var completedChapterIds = new List<int>();

        foreach (var chapter in allChapters)
        {
            var lessonIdsInChapter = chapter.Lessons.Select(l => l.LessonId).ToList();
            var userCompletedLessons = await _lessonProgressRepository.GetAllAsync(lp => lp.UserId == userId && lessonIdsInChapter.Contains(lp.LessonId));

            if (userCompletedLessons.Select(lp => lp.LessonId).Distinct().Count() == lessonIdsInChapter.Count)
            {
                completedChapterIds.Add(chapter.ChapterId);
            }
        }

        return completedChapterIds;
    }
}