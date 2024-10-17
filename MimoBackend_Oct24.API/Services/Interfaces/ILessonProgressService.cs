using MimoBackend_Oct24.API.Models;
using System.Threading.Tasks;
using MimoBackend_Oct24.API.Models.DTOs;

namespace MimoBackend_Oct24.API.Services
{
    public interface ILessonProgressService
    {
        Task<ValidationResult> ValidateLessonProgressAsync(LessonProgressDto lessonProgressDto);
        Task<LessonProgressResponseDto> CompleteLessonAsync(LessonProgressDto lessonProgressDto);
        Task<int> CountDistinctLessonsCompletedByUser(int userId);

    }
}
