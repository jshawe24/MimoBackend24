using Microsoft.EntityFrameworkCore;
using MimoBackend_Oct24.API.Data;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Repositories.Interfaces;

namespace MimoBackend_Oct24.API.Repositories
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        private readonly AppDbContext _context;

        public LessonRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Existing method to get lesson with full details
        public async Task<Lesson> GetLessonWithDetailsAsync(int lessonId)
        {
            
            var record = await _context.Lessons
                .Include(l => l.Chapter)        // Include Chapter
                .ThenInclude(c => c.Course)     // Include Course inside Chapter
                .FirstOrDefaultAsync(l => l.LessonId == lessonId); 

            return record;
            
        }
    }
}
