using Microsoft.EntityFrameworkCore;
using MimoBackend_Oct24.API.Data;
using MimoBackend_Oct24.API.Models;
using System.Linq.Expressions;
using MimoBackend_Oct24.API.Repositories.Interfaces;

namespace MimoBackend_Oct24.API.Repositories
{
    public class LessonProgressRepository : Repository<LessonProgress>, ILessonProgressRepository
    {
        private readonly AppDbContext _context;

        public LessonProgressRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // New method to count how many lessons have been completed by a user
        public async Task<int> CountCompletedLessonsAsync(int userId)
        {
            return await _context.LessonProgresses
                .CountAsync(lp => lp.UserId == userId);
        }

        // Method to get lesson progress based on a filter (predicate)
        public async Task<IEnumerable<LessonProgress>> GetAllAsync(Expression<Func<LessonProgress, bool>> predicate)
        {
            return await _context.LessonProgresses
                .Where(predicate) // Apply the predicate filter
                .ToListAsync();   // Return as a list asynchronously
        }
        
    }
}
