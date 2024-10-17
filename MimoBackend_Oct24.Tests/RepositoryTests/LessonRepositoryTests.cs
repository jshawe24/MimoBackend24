using Microsoft.EntityFrameworkCore;
using MimoBackend_Oct24.API.Data;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Repositories;

namespace MimoBackend_Oct24.Tests.RepositoryTests
{
    [TestFixture]
    public class LessonRepositoryTests : IDisposable
    {
        private AppDbContext _context;
        private LessonRepository _lessonRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _lessonRepository = new LessonRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetLessonWithDetailsAsync_ReturnsLessonWithChapterAndCourse()
        {
            // Arrange
            var course = new Course { CourseId = 1, Title = "Course 1" };
            var chapter = new Chapter { ChapterId = 1, Title = "Chapter 1", CourseId = course.CourseId, Course = course };
            var lesson = new Lesson { LessonId = 1, Title = "Lesson 1", ChapterId = chapter.ChapterId, Chapter = chapter };

            _context.Courses.Add(course);
            _context.Chapters.Add(chapter);
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            // Act
            var result = await _lessonRepository.GetLessonWithDetailsAsync(lesson.LessonId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.LessonId, Is.EqualTo(lesson.LessonId));
            Assert.That(result.Chapter, Is.Not.Null);
            Assert.That(result.Chapter.Course, Is.Not.Null);
            Assert.That(result.Chapter.Course.Title, Is.EqualTo(course.Title));
        }

        [Test]
        public async Task GetLessonWithDetailsAsync_ReturnsNull_WhenLessonDoesNotExist()
        {
            // Act
            var result = await _lessonRepository.GetLessonWithDetailsAsync(999); // Non-existent ID

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetLessonWithDetailsAsync_ReturnsNull_WhenChapterDoesNotExist()
        {
            // Arrange
            var lesson = new Lesson { LessonId = 1, Title = "Lesson 1", ChapterId = 999 }; // Non-existent ChapterId

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            // Act
            var result = await _lessonRepository.GetLessonWithDetailsAsync(lesson.LessonId);

            // Assert
            Assert.IsNull(result);
        }
        

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
