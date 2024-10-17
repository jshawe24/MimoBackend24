using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Models.DTOs;
using MimoBackend_Oct24.API.Repositories.Interfaces;

namespace MimoBackend_Oct24.API.Services
{
    public class LessonProgressService : ILessonProgressService
    {
        private readonly IRepository<User> _userRepository;
        private readonly ILessonRepository _lessonRepository;  // Use custom repository
        private readonly ILessonProgressRepository _lessonProgressRepository;  // Use custom repository

        public LessonProgressService(
            IRepository<User> userRepository,
            ILessonRepository lessonRepository,
            ILessonProgressRepository lessonProgressRepository           
        )
        {
            _userRepository = userRepository;
            _lessonRepository = lessonRepository;
            _lessonProgressRepository = lessonProgressRepository;
                   
        }

        public async Task<ValidationResult> ValidateLessonProgressAsync(LessonProgressDto lessonProgressDto)
        {
            // Check if LessonId exists using the Lesson repository
            var lesson = await _lessonRepository.GetByIdAsync(lessonProgressDto.LessonId);
            if (lesson == null)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = "Lesson not found." };
            }

            // Check if UserId exists using the User repository
            var user = await _userRepository.GetByIdAsync(lessonProgressDto.UserId);
            if (user == null)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = "User not found." };
            }

            // Check if startTime is before completionTime
            if (lessonProgressDto.StartTime >= lessonProgressDto.EndTime)
            {
                return new ValidationResult { IsValid = false, ErrorMessage = "Start time must be before completion time." };
            }

            return new ValidationResult { IsValid = true };
        }
        public async Task<LessonProgressResponseDto> CompleteLessonAsync(LessonProgressDto lessonProgressDto)
        {
            // Validate the lesson progress first
            var validationResult = await ValidateLessonProgressAsync(lessonProgressDto);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // Create the LessonProgress entity
            var lessonProgress = new LessonProgress
            {
                LessonId = lessonProgressDto.LessonId,
                UserId = lessonProgressDto.UserId,
                StartTime = lessonProgressDto.StartTime,
                EndTime = lessonProgressDto.EndTime
            };

            // Retrieve the lesson along with Chapter and Course details
            var lesson = await _lessonRepository.GetLessonWithDetailsAsync(lessonProgress.LessonId);
            if (lesson == null || lesson.Chapter == null)
            {
                throw new Exception("Lesson or Chapter not found.");
            }

            // Set the CourseId and ChapterId based on the lesson details
            lessonProgress.CourseId = lesson.Chapter.CourseId;
            lessonProgress.ChapterId = lesson.ChapterId;

            // Save the lesson progress
            await _lessonProgressRepository.AddAsync(lessonProgress);
            await _lessonProgressRepository.SaveChangesAsync();

            // Return a response DTO
            return new LessonProgressResponseDto
            {
                LessonProgressId = lessonProgress.LessonProgressId, // Assuming this property exists
                LessonId = lessonProgress.LessonId,
                UserId = lessonProgress.UserId,
                StartTime = lessonProgress.StartTime,
                EndTime = lessonProgress.EndTime
            };
        }
        public async Task<int> CountDistinctLessonsCompletedByUser(int userId)
        {
            return (await _lessonProgressRepository
                    .GetAllAsync(lp => lp.UserId == userId))
                .Select(lp => lp.LessonId)
                .Distinct()
                .Count();
        }

    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
