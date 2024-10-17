using Microsoft.EntityFrameworkCore;
using MimoBackend_Oct24.API.Data;
using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Repositories;

namespace MimoBackend_Oct24.Tests.RepositoryTests
{
    [TestFixture]
    public class ChapterRepositoryTests : IDisposable
    {
        private AppDbContext _context;
        private ChapterRepository _chapterRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // Use unique database name
                .Options;

            _context = new AppDbContext(options);
            _chapterRepository = new ChapterRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
        
        [Test]
        public async Task GetChapterIdsForCourseAsync_ReturnsCorrectIds()
        {
            // Arrange
            var courseId = 1;
            var chapter1 = new Chapter { ChapterId = 1, Title = "Chapter 1", CourseId = courseId };
            var chapter2 = new Chapter { ChapterId = 2, Title = "Chapter 2", CourseId = courseId };
            var chapter3 = new Chapter { ChapterId = 3, Title = "Chapter 3", CourseId = 999 }; // Different course

            _context.Chapters.AddRange(chapter1, chapter2, chapter3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _chapterRepository.GetChapterIdsForCourseAsync(courseId);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.Contains(1, result);
            Assert.Contains(2, result);
            Assert.IsFalse(result.Contains(3));
        }
        
        [Test]
        public async Task IsChapterCompletedByUserAsync_ReturnsTrue_WhenAllLessonsCompleted()
        {
            // Arrange 
            var chapterId = 1;
            var userId = 1;

            var chapter = new Chapter { ChapterId = chapterId, Title = "Chapter 1" };
            var lesson1 = new Lesson { LessonId = 1, Title = "Lesson 1", ChapterId = chapterId };
            var lesson2 = new Lesson { LessonId = 2, Title = "Lesson 2", ChapterId = chapterId };

            _context.Chapters.Add(chapter);
            _context.Lessons.AddRange(lesson1, lesson2);
            _context.LessonProgresses.AddRange(
                new LessonProgress { LessonId = lesson1.LessonId, UserId = userId },
                new LessonProgress { LessonId = lesson2.LessonId, UserId = userId }
            );

            await _context.SaveChangesAsync();

            // Act
            var result = await _chapterRepository.IsChapterCompletedByUserAsync(chapterId, userId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsChapterCompletedByUserAsync_ReturnsFalse_WhenNotAllLessonsCompleted()
        {
            // Arrange
            var chapterId = 1;
            var userId = 1;

            var chapter = new Chapter { ChapterId = chapterId, Title = "Chapter 1" };
            var lesson1 = new Lesson { LessonId = 1, Title = "Lesson 1", ChapterId = chapterId };
            var lesson2 = new Lesson { LessonId = 2, Title = "Lesson 2", ChapterId = chapterId };

            _context.Chapters.Add(chapter);
            _context.Lessons.AddRange(lesson1, lesson2);
            _context.LessonProgresses.Add(new LessonProgress { LessonId = 1, UserId = userId }); // Only one lesson completed

            await _context.SaveChangesAsync();

            // Act
            var result = await _chapterRepository.IsChapterCompletedByUserAsync(chapterId, userId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllWithLessonsAsync_ReturnsChaptersWithLessons()
        {
            // Arrange
            var chapter1 = new Chapter 
            { 
                ChapterId = 1, 
                Title = "Chapter 1",
                Lessons = new List<Lesson> 
                { 
                    new()
                    { 
                        LessonId = 1, 
                        Title = "Lesson 1", 
                        ChapterId = 1 
                    } 
                } 
            };
            var chapter2 = new Chapter 
            { 
                ChapterId = 2, 
                Title = "Chapter 2",
                Lessons = new List<Lesson> 
                { 
                    new()
                    { 
                        LessonId = 2, 
                        Title = "Lesson 2", 
                        ChapterId = 2 
                    },
                    new()
                    { 
                        LessonId = 3, 
                        Title = "Lesson 3", 
                        ChapterId = 2 
                    } 
                } 
            };

            _context.Chapters.AddRange(chapter1, chapter2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _chapterRepository.GetAllWithLessonsAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Lessons.Count, Is.EqualTo(1));
            Assert.That(result[1].Lessons.Count, Is.EqualTo(2));

            // Check lesson IDs from the second chapter
            var lessonIds = result[1].Lessons.Select(l => l.LessonId).ToList();
            Assert.Contains(2, lessonIds);
            Assert.Contains(3, lessonIds);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
