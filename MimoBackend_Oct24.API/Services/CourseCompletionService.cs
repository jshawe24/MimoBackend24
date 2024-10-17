using MimoBackend_Oct24.API.Repositories.Interfaces;

namespace MimoBackend_Oct24.API.Services;

public class CourseCompletionService : ICourseCompletionService
{
    private readonly IChapterRepository _chapterRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonProgressRepository _lessonProgressRepository;

    public CourseCompletionService(IChapterRepository chapterRepository, ILessonRepository lessonRepository, ILessonProgressRepository lessonProgressRepository)
    {
        _chapterRepository = chapterRepository;
        _lessonRepository = lessonRepository;
        _lessonProgressRepository = lessonProgressRepository;
    }

    public async Task<int> CheckCompletedCourseByLessons(int userId, int courseId)
    {
        var chaptersInCourse = await _chapterRepository.GetAllAsync();
        var chapterIdsInCourse = chaptersInCourse.Where(c => c.CourseId == courseId).Select(c => c.ChapterId).ToList();

        var lessonsInCourse = await _lessonRepository.GetAllAsync();
        var lessonIdsInCourse = lessonsInCourse.Where(l => chapterIdsInCourse.Contains(l.ChapterId)).Select(l => l.LessonId).ToList();

        var completedLessonsByUser = await _lessonProgressRepository.GetAllAsync(lp => lp.UserId == userId && lessonIdsInCourse.Contains(lp.LessonId));
        var completedLessonIds = completedLessonsByUser.Select(lp => lp.LessonId).ToList();

        return lessonIdsInCourse.All(lessonId => completedLessonIds.Contains(lessonId)) ? 1 : 0;
    }
}
