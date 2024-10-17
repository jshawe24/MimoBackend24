using Microsoft.EntityFrameworkCore;
using MimoBackend_Oct24.API.Data;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Repositories;
using System.Linq.Expressions;

namespace MimoBackend_Oct24.Tests.RepositoryTests
{
    [TestFixture]
    public class LessonProgressRepositoryTests : IDisposable
    {
        private AppDbContext _context;
        private LessonProgressRepository _lessonProgressRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _lessonProgressRepository = new LessonProgressRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CountCompletedLessonsAsync_ReturnsCorrectCount_WhenLessonsExist()
        {
            // Arrange
            var userId = 1;
            _context.LessonProgresses.AddRange(
                new LessonProgress { LessonId = 1, UserId = userId },
                new LessonProgress { LessonId = 2, UserId = userId },
                new LessonProgress { LessonId = 3, UserId = 2 } // Different user
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _lessonProgressRepository.CountCompletedLessonsAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(2)); // There are 2 completed lessons for userId 1
        }
        
        [Test]
        public async Task CountCompletedLessonsAsync_ReturnsZero_WhenNoLessonsExist()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await _lessonProgressRepository.CountCompletedLessonsAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(0)); // No lessons should be counted
        }

        [Test]
        public async Task GetAllAsync_ReturnsFilteredProgress_WhenPredicateIsMet()
        {
            // Arrange
            var userId = 1;
            _context.LessonProgresses.AddRange(
                new LessonProgress { LessonId = 1, UserId = userId },
                new LessonProgress { LessonId = 2, UserId = userId },
                new LessonProgress { LessonId = 3, UserId = 2 } // Different user
            );
            await _context.SaveChangesAsync();

            // Define a predicate that filters by UserId
            Expression<Func<LessonProgress, bool>> predicate = lp => lp.UserId == userId;

            // Act
            var result = await _lessonProgressRepository.GetAllAsync(predicate);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2)); // There should be 2 lessons for userId 1
        }

        [Test]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoMatches()
        {
            // Arrange
            var userId = 999;

            // Define a predicate that filters by a non-existent UserId
            Expression<Func<LessonProgress, bool>> predicate = lp => lp.UserId == userId; // Non-existent user

            // Act
            var result = await _lessonProgressRepository.GetAllAsync(predicate);

            // Assert
            Assert.IsEmpty(result); // Should return an empty list
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
