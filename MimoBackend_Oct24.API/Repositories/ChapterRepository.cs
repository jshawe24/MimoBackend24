using Microsoft.EntityFrameworkCore;
using MimoBackend_Oct24.API.Data;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Repositories.Interfaces;

namespace MimoBackend_Oct24.API.Repositories
{
    public class ChapterRepository : Repository<Chapter>, IChapterRepository
    {
        private readonly AppDbContext _context;

        public ChapterRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Method to get all chapter IDs for a specific course
        public async Task<List<int>> GetChapterIdsForCourseAsync(int courseId)
        {
            return await _context.Chapters
                .Where(c => c.CourseId == courseId)
                .Select(c => c.ChapterId)
                .ToListAsync();
        }

        // Method to check if a user has completed all lessons in a specific chapter
        public async Task<bool> IsChapterCompletedByUserAsync(int chapterId, int userId)
        {
            // Get all lesson IDs for the given chapter
            var lessonIds = await _context.Lessons
                .Where(l => l.ChapterId == chapterId)
                .Select(l => l.LessonId)
                .ToListAsync();

            // Check if the user has completed all of those lessons
            var completedLessonsCount = await _context.LessonProgresses
                .Where(lp => lp.UserId == userId && lessonIds.Contains(lp.LessonId))
                .Select(lp => lp.LessonId)
                .Distinct()
                .CountAsync();

            // Compare number of completed lessons with total lessons in the chapter
            return completedLessonsCount == lessonIds.Count;
        }

        // Method to get all chapters with their associated lessons
        public async Task<List<Chapter>> GetAllWithLessonsAsync()
        {
            // Fetch all chapters and include associated lessons
            return await _context.Chapters
                .Include(c => c.Lessons) // Include the lessons for each chapter
                .ToListAsync();
        }
    
    }
}
