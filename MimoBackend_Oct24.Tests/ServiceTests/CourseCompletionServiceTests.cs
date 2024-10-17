using MimoBackend_Oct24.API.Models; // Adjust based on where your models are located
using MimoBackend_Oct24.API.Repositories.Interfaces;
using MimoBackend_Oct24.API.Services;
using Moq;
using System.Linq.Expressions;

namespace MimoBackend_Oct24.Tests.ServiceTests
{
    [TestFixture]
    public class CourseCompletionServiceTests
    {
        private Mock<IChapterRepository> _mockChapterRepository;
        private Mock<ILessonRepository> _mockLessonRepository;
        private Mock<ILessonProgressRepository> _mockLessonProgressRepository;
        private CourseCompletionService _courseCompletionService;

        [SetUp]
        public void Setup()
        {
            _mockChapterRepository = new Mock<IChapterRepository>();
            _mockLessonRepository = new Mock<ILessonRepository>();
            _mockLessonProgressRepository = new Mock<ILessonProgressRepository>();

            _courseCompletionService = new CourseCompletionService(
                _mockChapterRepository.Object,
                _mockLessonRepository.Object,
                _mockLessonProgressRepository.Object);
        }

        [Test]
        public async Task CheckCompletedCourseByLessons_ReturnsOne_WhenAllLessonsCompleted()
        {
            // Arrange
            int userId = 1;
            int courseId = 1;

            var chapters = new List<Chapter>
            {
                new() { ChapterId = 1, CourseId = courseId },
                new() { ChapterId = 2, CourseId = courseId }
            };

            var lessons = new List<Lesson>
            {
                new() { LessonId = 1, ChapterId = 1 },
                new() { LessonId = 2, ChapterId = 1 },
                new() { LessonId = 3, ChapterId = 2 },
                new() { LessonId = 4, ChapterId = 2 }
            };

            var lessonProgress = new List<LessonProgress>
            {
                new() { UserId = userId, LessonId = 1 },
                new() { UserId = userId, LessonId = 2 },
                new() { UserId = userId, LessonId = 3 },
                new() { UserId = userId, LessonId = 4 }
            };

            _mockChapterRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(chapters);

            _mockLessonRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(lessons);

            _mockLessonProgressRepository
                .Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<LessonProgress, bool>>>()))
                .ReturnsAsync(lessonProgress);

            // Act
            var result = await _courseCompletionService.CheckCompletedCourseByLessons(userId, courseId);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task CheckCompletedCourseByLessons_ReturnsZero_WhenNotAllLessonsCompleted()
        {
            // Arrange
            int userId = 1;
            int courseId = 1;

            var chapters = new List<Chapter>
            {
                new() { ChapterId = 1, CourseId = courseId },
                new() { ChapterId = 2, CourseId = courseId }
            };

            var lessons = new List<Lesson>
            {
                new() { LessonId = 1, ChapterId = 1 },
                new() { LessonId = 2, ChapterId = 1 },
                new() { LessonId = 3, ChapterId = 2 },
                new() { LessonId = 4, ChapterId = 2 }
            };

            var lessonProgress = new List<LessonProgress>
            {
                new() { UserId = userId, LessonId = 1 },
                new() { UserId = userId, LessonId = 2 },
                // Lesson 3 and 4 are not completed
            };

            _mockChapterRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(chapters);

            _mockLessonRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(lessons);

            _mockLessonProgressRepository
                .Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<LessonProgress, bool>>>()))
                .ReturnsAsync(lessonProgress);

            // Act
            var result = await _courseCompletionService.CheckCompletedCourseByLessons(userId, courseId);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }
        
    }
}
