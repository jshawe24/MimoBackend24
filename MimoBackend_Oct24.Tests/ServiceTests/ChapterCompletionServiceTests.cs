using MimoBackend_Oct24.API.Models; // Adjust based on where your Chapter model is located
using MimoBackend_Oct24.API.Repositories.Interfaces;
using MimoBackend_Oct24.API.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MimoBackend_Oct24.Tests.ServiceTests
{
    [TestFixture]
    public class ChapterCompletionServiceTests
    {
        private Mock<IChapterRepository> _mockChapterRepository;
        private Mock<ILessonProgressRepository> _mockLessonProgressRepository;
        private ChapterCompletionService _chapterCompletionService;

        [SetUp]
        public void Setup()
        {
            _mockChapterRepository = new Mock<IChapterRepository>();
            _mockLessonProgressRepository = new Mock<ILessonProgressRepository>();

            _chapterCompletionService = new ChapterCompletionService(
                _mockChapterRepository.Object,
                _mockLessonProgressRepository.Object);
        }

        [Test]
        public async Task GetCompletedChapterIdsForUser_ReturnsEmptyList_WhenNoChaptersExist()
        {
            // Arrange
            int userId = 1;
            _mockChapterRepository.Setup(repo => repo.GetAllWithLessonsAsync())
                .ReturnsAsync(new List<Chapter>()); // No chapters available

            // Act
            var result = await _chapterCompletionService.GetCompletedChapterIdsForUser(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetCompletedChapterIdsForUser_ReturnsCompletedChapterIds()
        {
            // Arrange
            int userId = 1;
            var chapters = new List<Chapter>
            {
                new() { ChapterId = 1, Lessons = new List<Lesson>
                    {
                        new() { LessonId = 1 },
                        new() { LessonId = 2 }
                    }
                },
                new() { ChapterId = 2, Lessons = new List<Lesson>
                    {
                        new() { LessonId = 3 }
                    }
                }
            };

            var lessonProgress = new List<LessonProgress>
            {
                new() { UserId = userId, LessonId = 1 },
                new() { UserId = userId, LessonId = 2 },
                // User did not complete Lesson 3
            };

            _mockChapterRepository.Setup(repo => repo.GetAllWithLessonsAsync())
                .ReturnsAsync(chapters);

            _mockLessonProgressRepository
                .Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<LessonProgress, bool>>>()))
                .ReturnsAsync(lessonProgress);

            // Act
            var result = await _chapterCompletionService.GetCompletedChapterIdsForUser(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First(), Is.EqualTo(1)); // Chapter 1 is completed
        }

        [Test]
        public async Task GetCompletedChapterIdsForUser_ReturnsMultipleCompletedChapterIds()
        {
            // Arrange
            int userId = 1;
            var chapters = new List<Chapter>
            {
                new() { ChapterId = 1, Lessons = new List<Lesson>
                    {
                        new() { LessonId = 1 },
                        new() { LessonId = 2 }
                    }
                },
                new() { ChapterId = 2, Lessons = new List<Lesson>
                    {
                        new() { LessonId = 3 },
                        new() { LessonId = 4 }
                    }
                },
                new() { ChapterId = 3, Lessons = new List<Lesson>
                    {
                        new() { LessonId = 5 }
                    }
                }
            };

            var lessonProgress = new List<LessonProgress>
            {
                new() { UserId = userId, LessonId = 1 },
                new() { UserId = userId, LessonId = 2 },
                new() { UserId = userId, LessonId = 3 },
                new() { UserId = userId, LessonId = 4 }
                // Note: Lesson 5 is not completed
            };

            _mockChapterRepository.Setup(repo => repo.GetAllWithLessonsAsync())
                .ReturnsAsync(chapters);

            // Change the setup to mimic the filtering for userId in the tests
            _mockLessonProgressRepository
                .Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<LessonProgress, bool>>>()))
                .ReturnsAsync((Expression<Func<LessonProgress, bool>> predicate) =>
                    lessonProgress.Where(predicate.Compile()).ToList()); // Use the predicate to filter lessonProgress

            // Act
            var result = await _chapterCompletionService.GetCompletedChapterIdsForUser(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(2)); // Chapters 1 and 2 should be completed
            Assert.IsTrue(result.Contains(1));
            Assert.IsTrue(result.Contains(2));
        }
    }
}
