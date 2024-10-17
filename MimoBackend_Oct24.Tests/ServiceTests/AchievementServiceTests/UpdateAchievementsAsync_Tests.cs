using MimoBackend_Oct24.API.Models;
using MimoBackend_Oct24.API.Repositories.Interfaces;
using MimoBackend_Oct24.API.Services;
using Moq;

namespace MimoBackend_Oct24.Tests.ServiceTests.AchievementServiceTests
{
    [TestFixture]
    public class UpdateAchievementsAsync_Tests
    {
        private Mock<IRepository<UserAchievement>> _mockUserAchievementRepository;
        private Mock<IRepository<Achievement>> _mockAchievementRepository;
        private Mock<ILessonProgressService> _mockLessonProgressService;
        private Mock<IChapterCompletionService> _mockChapterCompletionService;
        private Mock<ICourseCompletionService> _mockCourseCompletionService;
        private AchievementService _achievementService;

        [SetUp]
        public void Setup()
        {
            _mockUserAchievementRepository = new Mock<IRepository<UserAchievement>>();
            _mockAchievementRepository = new Mock<IRepository<Achievement>>();
            _mockLessonProgressService = new Mock<ILessonProgressService>();
            _mockChapterCompletionService = new Mock<IChapterCompletionService>();
            _mockCourseCompletionService = new Mock<ICourseCompletionService>();
            
            _achievementService = new AchievementService(
                _mockUserAchievementRepository.Object,
                _mockAchievementRepository.Object,
                _mockLessonProgressService.Object,
                _mockChapterCompletionService.Object,
                _mockCourseCompletionService.Object);
        }

        [Test]
        public async Task UpdateAchievementsAsync_CreatesUserAchievements_WhenNoneExist()
        {
            // Arrange
            int userId = 1;
            var achievements = new List<Achievement>
            {
                new() { AchievementId = 1, Title = "Complete 5 Lessons", Target = 5 },
                new() { AchievementId = 2, Title = "Complete 25 Lessons", Target = 25 },
                new() { AchievementId = 3, Title = "Complete 50 Lessons", Target = 50 },
                new() { AchievementId = 4, Title = "Complete 1 Chapter", Target = 1 },
                new() { AchievementId = 5, Title = "Complete 5 Chapters", Target = 5 },
                new() { AchievementId = 6, Title = "Complete Swift Course", Target = 1 },  
                new() { AchievementId = 7, Title = "Complete Javascript Course", Target = 1 },  
                new() { AchievementId = 8, Title = "Complete C# Course", Target = 1 }  
            };
            var lessonProgressCount = 12; // Mocking completed lessons
            var completedChapterIds = new List<int> { 1, 2 }; // Mocking completed chapters

            _mockLessonProgressService.Setup(s => s.CountDistinctLessonsCompletedByUser(userId))
                .ReturnsAsync(lessonProgressCount);

            _mockChapterCompletionService.Setup(s => s.GetCompletedChapterIdsForUser(userId))
                .ReturnsAsync(completedChapterIds);

            _mockAchievementRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(achievements);

            _mockUserAchievementRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<UserAchievement>()); // No achievements exist

            // Act
            await _achievementService.UpdateAchievementsAsync(userId);

            // Assert
            _mockUserAchievementRepository.Verify(repo => repo.AddAsync(It.IsAny<UserAchievement>()), Times.Exactly(achievements.Count));
            _mockUserAchievementRepository.Verify(repo => repo.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Test]
        public async Task UpdateAchievementsAsync_UpdatesUserAchievements_WhenTheyExist()
        {
            // Arrange
            int userId = 1;
            var userAchievements = new List<UserAchievement>
            {
                new() { UserId = userId, AchievementId = 1, Progress = 0, IsCompleted = false },
                new() { UserId = userId, AchievementId = 2, Progress = 0, IsCompleted = false },
                new() { UserId = userId, AchievementId = 3, Progress = 0, IsCompleted = false },
                new() { UserId = userId, AchievementId = 4, Progress = 0, IsCompleted = false },
                new() { UserId = userId, AchievementId = 5, Progress = 0, IsCompleted = false },
                new() { UserId = userId, AchievementId = 6, Progress = 0, IsCompleted = false },
                new() { UserId = userId, AchievementId = 7, Progress = 0, IsCompleted = false },
                new() { UserId = userId, AchievementId = 8, Progress = 0, IsCompleted = false },
            };

            var achievements = new List<Achievement>
            {
                new() { AchievementId = 1, Title = "Complete 5 Lessons", Target = 5 },
                new() { AchievementId = 2, Title = "Complete 25 Lessons", Target = 25 },
                new() { AchievementId = 3, Title = "Complete 50 Lessons", Target = 50 },
                new() { AchievementId = 4, Title = "Complete 1 Chapter", Target = 1 },
                new() { AchievementId = 5, Title = "Complete 5 Chapters", Target = 5 },
                new() { AchievementId = 6, Title = "Complete Swift Course", Target = 1 },  
                new() { AchievementId = 7, Title = "Complete Javascript Course", Target = 1 },  
                new() { AchievementId = 8, Title = "Complete C# Course", Target = 1 }  
            };

            var lessonProgressCount = 12; // Mocking completed lessons
            var completedChapterIds = new List<int> { 1, 2, 3, 4, 5 ,6}; // Mocking completed chapters

            _mockLessonProgressService.Setup(s => s.CountDistinctLessonsCompletedByUser(userId))
                .ReturnsAsync(lessonProgressCount);

            _mockChapterCompletionService.Setup(s => s.GetCompletedChapterIdsForUser(userId))
                .ReturnsAsync(completedChapterIds);

            _mockAchievementRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(achievements);

            _mockUserAchievementRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(userAchievements);
            
            // Mocking course completion checks
            _mockCourseCompletionService.Setup(s => s.CheckCompletedCourseByLessons(userId, 1))
                .ReturnsAsync(1); // Swift course is completed

            _mockCourseCompletionService.Setup(s => s.CheckCompletedCourseByLessons(userId, 2))
                .ReturnsAsync(1); // Javascript course is completed

            _mockCourseCompletionService.Setup(s => s.CheckCompletedCourseByLessons(userId, 3))
                .ReturnsAsync(1); // C# course is completed

            // Act
            await _achievementService.UpdateAchievementsAsync(userId);

            // Assert
            // Lesson completion asserts  
            Assert.That(userAchievements.First(ua => ua.AchievementId == 1).Progress, Is.EqualTo(5));
            Assert.IsTrue(userAchievements.First(ua => ua.AchievementId == 1).IsCompleted);
            Assert.That(userAchievements.First(ua => ua.AchievementId == 2).Progress, Is.EqualTo(12));
            Assert.IsFalse(userAchievements.First(ua => ua.AchievementId == 2).IsCompleted);
            Assert.That(userAchievements.First(ua => ua.AchievementId == 3).Progress, Is.EqualTo(12));
            Assert.IsFalse(userAchievements.First(ua => ua.AchievementId == 3).IsCompleted);
            
            // Chapter completion asserts  
            Assert.That(userAchievements.First(ua => ua.AchievementId == 4).Progress, Is.EqualTo(1));
            Assert.IsTrue(userAchievements.First(ua => ua.AchievementId == 4).IsCompleted);
            Assert.That(userAchievements.First(ua => ua.AchievementId == 5).Progress, Is.EqualTo(5));
            Assert.IsTrue(userAchievements.First(ua => ua.AchievementId == 5).IsCompleted);
            
            // Course completion asserts  
            Assert.That(userAchievements.First(ua => ua.AchievementId == 6).Progress, Is.EqualTo(1));
            Assert.IsTrue(userAchievements.First(ua => ua.AchievementId == 6).IsCompleted);
            Assert.That(userAchievements.First(ua => ua.AchievementId == 7).Progress, Is.EqualTo(1));
            Assert.IsTrue(userAchievements.First(ua => ua.AchievementId == 7).IsCompleted);
            Assert.That(userAchievements.First(ua => ua.AchievementId == 8).Progress, Is.EqualTo(1));
            Assert.IsTrue(userAchievements.First(ua => ua.AchievementId == 8).IsCompleted);

            // Verify that Update was called for each achievement
            _mockUserAchievementRepository.Verify(repo => repo.Update(It.IsAny<UserAchievement>()), Times.Exactly(userAchievements.Count));
            _mockUserAchievementRepository.Verify(repo => repo.SaveChangesAsync(), Times.AtLeastOnce);
        }
    }
}
