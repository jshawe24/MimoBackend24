using MimoBackend_Oct24.API.Repositories.Interfaces;
using MimoBackend_Oct24.API.Services;
using Moq;
using MimoBackend_Oct24.API.Models;

namespace MimoBackend_Oct24.Tests.ServiceTests.AchievementServiceTests
{
    [TestFixture]
    public class GetAchievementAsync_Tests
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
        public async Task GetAchievementsForUserAsync_ReturnsNull_WhenUserHasNoAchievements()
        {
            // Arrange
            int userId = 1;
            _mockUserAchievementRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<UserAchievement>()); // No achievements available for the user

            // Act
            var result = await _achievementService.GetAchievementsForUserAsync(userId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAchievementsForUserAsync_ReturnsAchievementDtos_WhenUserHasAchievements()
        {
            // Arrange
            int userId = 1;
            var userAchievements = new List<UserAchievement>
            {
                new() { UserId = userId, AchievementId = 1, Progress = 5, IsCompleted = true },
                new() { UserId = userId, AchievementId = 2, Progress = 10, IsCompleted = false }
            };

            var achievements = new List<Achievement>
            {
                new() { AchievementId = 1, Title = "Complete 5 Lessons", Target = 5 },
                new() { AchievementId = 2, Title = "Complete 25 Lessons", Target = 25 }
            };

            _mockUserAchievementRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(userAchievements);

            _mockAchievementRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(achievements[0]);

            _mockAchievementRepository.Setup(repo => repo.GetByIdAsync(2))
                .ReturnsAsync(achievements[1]);

            // Act
            var result = await _achievementService.GetAchievementsForUserAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.IsTrue(result.Any(a => a.AchievementId == 1 && a.IsCompleted && a.Progress == 5));
            Assert.IsTrue(result.Any(a => a.AchievementId == 2 && !a.IsCompleted && a.Progress == 10));
        }
    }
}
