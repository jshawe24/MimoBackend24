using Moq;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Models.DTOs;
using MimoBackend_Oct24.API.Repositories.Interfaces;
using MimoBackend_Oct24.API.Services;
using Xunit;
using Assert = Xunit.Assert;

namespace MimoBackend_Oct24.Tests.ServiceTests.LessonProgressServiceTests
{
    public class CompleteLessonAsyncTests
    {
        private readonly Mock<IRepository<User>> _userRepository;
        private readonly Mock<ILessonRepository> _lessonRepository;
        private readonly Mock<ILessonProgressRepository> _lessonProgressRepository;
        private readonly LessonProgressService _lessonProgressService;

        public CompleteLessonAsyncTests()
        {
            _userRepository = new Mock<IRepository<User>>();
            _lessonRepository = new Mock<ILessonRepository>();
            _lessonProgressRepository = new Mock<ILessonProgressRepository>();

            _lessonProgressService = new LessonProgressService(
                _userRepository.Object,
                _lessonRepository.Object,
                _lessonProgressRepository.Object
            );
        }

        [Fact]
        public async Task CompleteLessonAsync_AddsLessonProgress()
        {
            // Arrange
            var lessonId = 1;
            var userId = 1;

            var lessonProgressDto = new LessonProgressDto
            {
                LessonId = lessonId,
                UserId = userId,
                StartTime = DateTime.Now.AddMinutes(-30),
                EndTime = DateTime.Now
            };

            var lesson = new Lesson
            {
                LessonId = lessonId,
                Chapter = new Chapter { CourseId = 1, ChapterId = 1 } // Provide course and chapter info
            };

            var user = new User { UserId = userId };  // Mocked user

            // Set up mocks
            _lessonRepository.Setup(repo => repo.GetByIdAsync(lessonId)).ReturnsAsync(lesson);  // Mock the correct method and value
            _lessonRepository.Setup(repo => repo.GetLessonWithDetailsAsync(lessonId)).ReturnsAsync(lesson);  // Mock the correct method and value
            _userRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);  // Return a mocked user

            // Act
            var result = await _lessonProgressService.CompleteLessonAsync(lessonProgressDto);

            // Assert
            _lessonProgressRepository.Verify(repo => repo.AddAsync(It.IsAny<LessonProgress>()), Times.Once);
            _lessonProgressRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);

            // Optionally verify the returned result
            Assert.NotNull(result);
            Assert.Equal(lessonId, result.LessonId);
            Assert.Equal(userId, result.UserId);
            Assert.True(result.StartTime < result.EndTime); // Check if StartTime is before EndTime
        }

        [Fact]
        public async Task CompleteLessonAsync_InvalidLesson_ThrowsException()
        {
            // Arrange
            var lessonProgressDto = new LessonProgressDto
            {
                LessonId = 1,
                UserId = 1,
                StartTime = DateTime.Now.AddMinutes(-30),
                EndTime = DateTime.Now
            };

            // Mock the repository to return null for the lesson
            _lessonRepository.Setup(repo => repo.GetLessonWithDetailsAsync(lessonProgressDto.LessonId))!
                .ReturnsAsync((Lesson)null!);

            // Act & Assert - Expecting an ArgumentException due to lesson not being found
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _lessonProgressService.CompleteLessonAsync(lessonProgressDto));

            // Optionally verify the exception message if specific exception text is important
            Assert.Equal("Lesson not found.", exception.Message);
        }

        [Fact]
        public async Task CompleteLessonAsync_InvalidUser_ThrowsException()
        {
            // Arrange
            var lessonId = 1;
            var userId = 1;
            var lessonProgressDto = new LessonProgressDto
            {
                LessonId = lessonId,
                UserId = userId,
                StartTime = DateTime.Now.AddMinutes(-30),
                EndTime = DateTime.Now
            };

            var lesson = new Lesson
            {
                LessonId = lessonId,
                Chapter = new Chapter { CourseId = 1, ChapterId = 1 }
            };

            _lessonRepository.Setup(repo => repo.GetLessonWithDetailsAsync(lessonId)).ReturnsAsync(lesson);
            _userRepository.Setup(repo => repo.GetByIdAsync(userId))!.ReturnsAsync((User)null!); // Return null user

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _lessonProgressService.CompleteLessonAsync(lessonProgressDto));
        }

        [Fact]
        public async Task CompleteLessonAsync_InvalidTimeFrame_ThrowsException()
        {
            // Arrange
            var lessonId = 1;
            var userId = 1;
            var lessonProgressDto = new LessonProgressDto
            {
                LessonId = lessonId,
                UserId = userId,
                StartTime = DateTime.Now.AddMinutes(30), // Start time after End time
                EndTime = DateTime.Now
            };

            var lesson = new Lesson
            {
                LessonId = lessonId,
                Chapter = new Chapter { CourseId = 1, ChapterId = 1 }
            };

            _lessonRepository.Setup(repo => repo.GetByIdAsync(lessonId)).ReturnsAsync(lesson);  // Mock the correct method and value
            _userRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(new User());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _lessonProgressService.CompleteLessonAsync(lessonProgressDto));
            Assert.Equal("Start time must be before completion time.", exception.Message);
        }
    }
}
